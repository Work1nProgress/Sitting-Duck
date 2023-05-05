using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingDamageNumber : PoolObject
{
    [SerializeField] private float lifeTime;
    [SerializeField] private TMP_Text text;
    private float _lifeCounter;
    private float _damageAmount;

    public void Init(float damageAmount)
    {
        _damageAmount = damageAmount;
        text.text = _damageAmount.ToString(CultureInfo.InvariantCulture);
        gameObject.transform.SetParent(null, true);
        transform.rotation = Quaternion.identity;
        transform.DOScale(new Vector3(2, 2, 2), .5f);
        transform.DOShakePosition(.3f, .5f);
    }

    private void OnEnable()
    {
        _lifeCounter = lifeTime;
        text.text = _damageAmount.ToString(CultureInfo.InvariantCulture);
        text.alpha = 1;
    }

    private void Update()
    {
        text.alpha -= Time.deltaTime * 2.5f;
        _lifeCounter -= Time.deltaTime;
        if (_lifeCounter <= 0)
        {
            gameObject.SetActive(false);
        }
    }

}
