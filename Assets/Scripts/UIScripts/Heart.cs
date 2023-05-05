using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private GameObject _beating;

    [SerializeField] private GameObject _bleeding;

    private bool _isBeating = true;

    public void ToggleHeart()
    {
        _isBeating = !_isBeating;
        _beating.SetActive(_isBeating);
        _bleeding.SetActive(!_isBeating);
    }
}
