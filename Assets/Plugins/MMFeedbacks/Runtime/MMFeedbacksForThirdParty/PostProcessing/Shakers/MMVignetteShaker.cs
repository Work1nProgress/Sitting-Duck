﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MoreMountains.Feedbacks;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a vignette post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(Volume))]
    public class MMVignetteShaker : MonoBehaviour
    {
        public int Channel = 0;
        public bool RelativeIntensity = false;
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        public float ShakeDuration = 0.2f;
        public float ShakeAmplitude = 1.0f;

        [MMFReadOnly]
        public bool Shaking = false;

        [MMFInspectorButton("StartShaking")]
        public bool TestShakeButton;

        protected Vignette _vignette;
        protected Volume _volume;
        protected float _shakeStartedTimestamp;
        protected float _remappedTimeSinceStart;
        protected ClampedFloatParameter _initialIntensity;

        protected virtual void Awake()
        {
            _volume = this.gameObject.GetComponent<Volume>();
            _volume.profile.TryGet(out _vignette);
            _initialIntensity = _vignette.intensity;
            Shaking = false;
        }

        public virtual void StartShaking()
        {
            if (Shaking)
            {
                return;
            }
            else
            {
                _shakeStartedTimestamp = Time.time;
                Shaking = true;
            }
        }

        protected virtual void Update()
        {
            if (Shaking)
            {
                Shake();
            }

            if (Shaking && (Time.time - _shakeStartedTimestamp > ShakeDuration))
            {
                Shaking = false;
                _vignette.intensity.Override(_initialIntensity.value);
            }
        }

        protected virtual void Shake()
        {
            _remappedTimeSinceStart = MMFeedbacksHelpers.Remap(Time.time - _shakeStartedTimestamp, 0f, ShakeDuration, 0f, 1f);
            _vignette.intensity.Override(ShakeIntensity.Evaluate(_remappedTimeSinceStart) * ShakeAmplitude);
            if (RelativeIntensity) { _vignette.intensity.Override(_vignette.intensity.value + _initialIntensity.value); }
        }


        public virtual void OnVignetteShakeEvent(AnimationCurve intensity, float duration, float amplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }
            ShakeDuration = duration;
            ShakeIntensity = intensity;
            ShakeAmplitude = amplitude * attenuation;
            RelativeIntensity = relativeIntensity;
            this.StartShaking();
        }

        protected virtual void OnEnable()
        {
            MMVignetteShakeEvent.Register(OnVignetteShakeEvent);
        }

        protected virtual void OnDisable()
        {
            MMVignetteShakeEvent.Unregister(OnVignetteShakeEvent);
        }
    }

    public struct MMVignetteShakeEvent
    {
        public delegate void Delegate(AnimationCurve intensity, float duration, float amplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(AnimationCurve intensity, float duration, float amplitude, bool relativeIntensity = false, float attenuation = 1.0f, int channel = 0)
        {
            OnEvent?.Invoke(intensity, duration, amplitude, relativeIntensity, attenuation, channel);
        }
    }
}
