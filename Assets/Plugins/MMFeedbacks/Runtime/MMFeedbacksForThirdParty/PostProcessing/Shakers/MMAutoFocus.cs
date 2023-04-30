﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MoreMountains.Feedbacks;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This class will set the depth of field to focus on the set of targets specified in its inspector.
    /// </summary>
    [RequireComponent(typeof(Volume))]
    public class MMAutoFocus : MonoBehaviour
    {
        // Array of targets
        public Transform[] FocusTargets;

        // Current target
        public float FocusTargetID;

        // Cache profile
        Volume _volume;
        VolumeProfile _profile;
        DepthOfField _depthOfField;

        [Range(0.1f, 20f)] public float Aperture;


        void Start()
        {
            _volume = GetComponent<Volume>();
            _profile = _volume.profile;
            _profile.TryGet(out _depthOfField);
        }

        void Update()
        {

            // Set variables
            // Get distance from camera and target
            float distance = Vector3.Distance(transform.position, FocusTargets[Mathf.FloorToInt(FocusTargetID)].position);
            _depthOfField.focusDistance.Override(distance);
            _depthOfField.aperture.Override(Aperture);
        }
    }
}
