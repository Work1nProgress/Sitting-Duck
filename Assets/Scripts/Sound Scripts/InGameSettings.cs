using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviour
{
    [SerializeField] SoundTypes soundTypes;
    SoundSettings soundSettings;
    Slider slider;

    private void Start() {
        soundSettings = FindObjectOfType<SoundSettings>();
        slider = GetComponent<Slider>();

        switch(soundTypes) {
            case SoundTypes.MUSIC:
                slider.onValueChanged.AddListener(delegate {ChangeMusicValue();});
                break;
            case SoundTypes.SFX:
                slider.onValueChanged.AddListener(delegate {ChangeSFXValue();});
                break;
        }
    }
    void ChangeMusicValue() {
        soundSettings.ChangeMusicValue(slider.value);
    }
    void ChangeSFXValue() {
        soundSettings.ChangeSFXValue(slider.value);
    }
}
