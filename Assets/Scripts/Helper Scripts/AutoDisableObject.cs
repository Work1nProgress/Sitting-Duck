using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableObject : MonoBehaviour
{
    [SerializeField] private float disableAfter = 1f;
    private float _disableTimer;

    public void StartCountdown()
    {
        _disableTimer = disableAfter;
    }

    private void Start()
    {
        _disableTimer = disableAfter;
    }

    private void Update()
    {
        if (!(_disableTimer >= 0)) return;
        
        _disableTimer -= Time.deltaTime;
        if (_disableTimer < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
