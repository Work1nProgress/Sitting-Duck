using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip repeat1;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if(!audioSource.isPlaying) {
            audioSource.clip = repeat1;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
