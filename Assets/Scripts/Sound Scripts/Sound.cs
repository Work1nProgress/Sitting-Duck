using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string SoundName;

    public bool Randomize;
    [SerializeField]private SoundItem[] soundItems;

    public SoundItem SoundItem
    {
        get
        {
            if (Randomize)
            {
                return soundItems[Random.Range(0, soundItems.Length)];
            }
            else
            {
                return soundItems[0];
            }
        }

    }

  

   

 
}

[System.Serializable]
    public class SoundItem{
    public AudioClip AudioClip;

    [Range(-0.5f, 0.5f)]
    public float Volume = 0.5f;
    public bool RandomPitchAmplitude;


}
