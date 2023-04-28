using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettings : MonoBehaviour
{
    float musicValue = .5f, SFXValue = .5f, masterValue = .5f;
    public static SoundSettings instance;
    private void Awake() {
        if(instance == null) instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    public void ChangeMusicValue(float value) {
        musicValue = value;
    }
    public void ChangeSFXValue(float value) {
        SFXValue = value;
    }
    void ChangeMasterValue(float value) {
        masterValue = value;
    }
    public float ReturnSFX() {
        return SFXValue;
    }
    public float ReturnMusic() {
        return musicValue;
    }
}
