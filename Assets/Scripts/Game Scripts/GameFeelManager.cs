using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameFeelManager : GenericSingleton<GameFeelManager>
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float _shakeTimer;

    protected override void Awake()
    {
        base.Awake();
        noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0)
            {
                noise.m_AmplitudeGain = 0;
                noise.m_FrequencyGain = 0;
            }
        }
        
    }

    public void PlayerTookDamageShakeTheCameraFor(float seconds)
    {
        noise.m_AmplitudeGain = 3;
        noise.m_FrequencyGain = 3;
        _shakeTimer = seconds;
    }
    
}
