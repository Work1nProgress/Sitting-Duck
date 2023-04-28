using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundValueFromSettings : MonoBehaviour
{
    SoundSettings soundSettings;
    [SerializeField] AudioSources audioSource;
    [SerializeField] SoundTypes soundTypes;

    void Start()
    {
        soundSettings = FindObjectOfType<SoundSettings>();

        switch(audioSource) {
            case AudioSources.AudioSource:
                if(soundTypes == SoundTypes.MUSIC) GetComponent<AudioSource>().volume = soundSettings.ReturnMusic();
                else if(soundTypes == SoundTypes.SFX) GetComponent<AudioSource>().volume = soundSettings.ReturnSFX();
                break;
            case AudioSources.MMFeedbackSound:
                if(soundTypes == SoundTypes.MUSIC) {
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MinVolume = soundSettings.ReturnMusic();
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MaxVolume = soundSettings.ReturnMusic();  
                } else if(soundTypes == SoundTypes.SFX) {
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MinVolume = soundSettings.ReturnSFX();
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MaxVolume = soundSettings.ReturnSFX(); 
                }
                break;
        }
    }
    void Update() {
        switch(audioSource) {
            case AudioSources.AudioSource:
                if(soundTypes == SoundTypes.MUSIC) GetComponent<AudioSource>().volume = soundSettings.ReturnMusic();
                else if(soundTypes == SoundTypes.SFX) GetComponent<AudioSource>().volume = soundSettings.ReturnSFX();
                break;
            case AudioSources.MMFeedbackSound:
                if(soundTypes == SoundTypes.MUSIC) {
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MinVolume = soundSettings.ReturnMusic();
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MaxVolume = soundSettings.ReturnMusic();  
                } else if(soundTypes == SoundTypes.SFX) {
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MinVolume = soundSettings.ReturnSFX();
                    GetComponent<MoreMountains.Feedbacks.MMFeedbackSound>().MaxVolume = soundSettings.ReturnSFX(); 
                }
                break;
        }
    }
}
