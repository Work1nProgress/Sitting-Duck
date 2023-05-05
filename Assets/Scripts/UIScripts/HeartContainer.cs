﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    [SerializeField] private GameObject heart;
    private List<GameObject> _hearts = new List<GameObject>();
    private EntityStats _playerStats;
    private int _maxHealth;
    private int _currentHealth;

    private void Awake()
    {
        GameManager.Instance.OnSceneLoaded += Wakey;
    }

    private void Wakey()
    {
        GameManager.Instance.OnSceneLoaded -= Wakey;
        _playerStats = ControllerGame.Instance.Player.GetComponent<EntityStats>();
        _maxHealth = _playerStats.MaxHealth;
        _currentHealth = _playerStats.Health;
        _playerStats.OnHealthChanged += HealthChanged;
    }

    private void HealthChanged(int oldhealth, int newhealth, int maxhealth)
    {
        for (int i = _hearts.Count-1; i >= 0; i--)
        {
            if (i >= newhealth)
            {
                Destroy(_hearts[i]);
                _hearts.RemoveAt(i);
            }
        }
    }

    private void Start()
    {
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        for (var i = 0; i < _maxHealth; i++)
        {
            var newHeart = Instantiate(heart, gameObject.transform);
            _hearts.Add(newHeart);
        }
    }
}