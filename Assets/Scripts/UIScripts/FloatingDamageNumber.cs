using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDamageNumber : PoolObject
{
    [SerializeField] private float lifeTime;
    private float _lifeCounter;
    private float _damageAmount;

    private void Awake()
    {
        _lifeCounter = lifeTime;
    }

    private void Update()
    {
        _lifeCounter -= Time.deltaTime;
        if (_lifeCounter <= 0)
        {
            gameObject.SetActive(false);
        }
    }

}
