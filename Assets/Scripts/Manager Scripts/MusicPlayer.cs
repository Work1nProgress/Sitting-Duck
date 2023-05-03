using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    AudioSource _source;

    [SerializeField] bool _shuffle;
    [SerializeField] private int _startIndex;
    [SerializeField] private AudioClip[] _musicClips;
    int _clipIndex = 0;

    private void Awake()
    {
        _clipIndex = Mathf.Clamp(_startIndex, 0, _musicClips.Length - 1);
        _source = GetComponent<AudioSource>();
    }

    void Start()
    {
        _source.clip = _musicClips[_clipIndex];
        _source.Play();
    }

    void Update()
    {
        if (!_source.isPlaying)
        {
            if (!_shuffle)
                ChangeMusic(GetNextIndex());
            else
                ChangeMusic(GetRandomIndex());

        }
    }

    private int GetNextIndex()
    {
        _clipIndex++;
        if (_clipIndex >= _musicClips.Length)
            _clipIndex = 0;

        return _clipIndex;
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, _musicClips.Length);
    }

    private void ChangeMusic(int index)
    {
        index = Mathf.Clamp(index, 0, _musicClips.Length - 1);

        _source.Stop();
        _source.clip = _musicClips[index];
        _source.Play();
        Debug.Log("Play new music");
    }
}
