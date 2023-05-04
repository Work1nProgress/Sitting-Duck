using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimationPlayer : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float _timeToNextFrame;
    [SerializeField] private bool loopOn = true;
    private float _countDownTimer;
    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = _sprites[0];
        _countDownTimer = _timeToNextFrame;
    }

    private int _index = 0;
    
    void Update()
    {
        _countDownTimer -= Time.deltaTime;
        if (!(_countDownTimer <= 0)) return;

        if (_index < _sprites.Length - 1)
        {
            _index++;
        }
        else
        {
            if (loopOn)
            {
                _index = 0;
            }
            else
            {
                return;
            }
        }

        _image.sprite = _sprites[_index];
        _countDownTimer = _timeToNextFrame;
    }
}
