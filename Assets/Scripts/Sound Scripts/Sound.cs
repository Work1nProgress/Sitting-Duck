using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    [SerializeField]private SoundEnums soundType;

    [SerializeField]private AudioClip _clip;

    public AudioClip Clip { get => _clip; }
    public SoundEnums SoundType { get => soundType;}

}
