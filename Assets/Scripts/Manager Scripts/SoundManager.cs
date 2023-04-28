using System;
using System.Collections.Generic;

using UnityEngine.Audio;
using UnityEngine;

public class SoundManager : GenericSingleton<SoundManager>
{

    
    [SerializeField]private Sound[] sounds;
    private Dictionary<SoundEnums,Sound> soundDictionary;

    //private Queue<Sound> soundProcessQueue;
    protected override void Awake() {
        
        base.Awake();
        //DontDestroyOnLoad(gameObject);
        
        soundDictionary = new Dictionary<SoundEnums, Sound>();
        //soundProcessQueue = new Queue<Sound>();        
        foreach(Sound sound in sounds)
        {
            soundDictionary.Add(sound.SoundType,sound);
        }
    }


    public void Play(SoundEnums soundType,AudioSource source)
    {
        if(soundDictionary.TryGetValue(soundType,out Sound sound))
        {
            source.clip = sound.Clip;
            source.Play();
        }        
    }

    public void Stop(AudioSource source)
    {
        source.Stop();
    }

    public void PlayInstantly(SoundEnums soundType,AudioSource source)
    {
        if(soundDictionary.TryGetValue(soundType,out Sound sound))
        {
            source.PlayOneShot(sound.Clip);
        }
        else
        {
            Debug.Log("Sound not found.");
        }
    }
}
