using System;
using System.Collections.Generic;

using UnityEngine.Audio;
using UnityEngine;

public class SoundManager : GenericSingleton<SoundManager>
{

    [SerializeField]
    AudioMixerGroup Mixer;

    [SerializeField] private List<Sound> sounds;

    AudioSource[] clips;
    [SerializeField]
    float minPitch, maxPitch;

    protected override void Awake()
    {

        base.Awake();
        int nOfClips = 11;

        clips = new AudioSource[nOfClips];
        for (int i = 0; i < nOfClips; i++)
        {
            clips[i] = new GameObject().AddComponent<AudioSource>();
            clips[i].gameObject.transform.SetParent(transform);
            clips[i].outputAudioMixerGroup = Mixer;
            if (i > 0)
            {
                clips[i].pitch = (maxPitch - minPitch) * ((i - 1f) / (nOfClips - 1f)) + minPitch + 1;
            }
            clips[i].name = $"pitch_{clips[i].pitch}";
        }

    }


    public void Play(string name)
    {

        var idx = sounds.FindIndex(x => x.SoundName == name);
        if (idx == -1)
        {
            Debug.LogError($"Sound {name} not found");
            return;
        }
        else
        {

            var soundItem = sounds[idx].SoundItem;
            if (soundItem.RandomPitchAmplitude)
            {
                clips[UnityEngine.Random.Range(1, clips.Length)].PlayOneShot(soundItem.AudioClip, soundItem.Volume);
            }
            clips[0].PlayOneShot(soundItem.AudioClip, soundItem.Volume);
        }
    }
}

   
