using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BulletSpawner))]
[RequireComponent(typeof(EntityStats))]
public class EnemyController : PoolObject
{

    protected Rigidbody2D _rigidbody;
    protected BulletSpawner _bulletSpawner;

    protected GameManager _gameManager;
    protected EntityStats _entityStats;

    protected EnemyState _activeState;

    protected virtual void Awake()
    {
        SetComponentReferences();
        _entityStats.OnDeath += HandleDeath;
    }

    protected virtual void Start()
    {
        if (_activeState != null)
        {
            _activeState.EnterState();
            _activeState.OnRequestStateChange += ChangeState;
        }

        _gameManager = _entityStats.GetGameManager();
    }

    protected virtual void Update()
    {
        if(_activeState != null)
        _activeState.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        if (_activeState != null)
            _activeState.FixedUpdateState();
    }

    protected void ChangeState(EnemyState newState)
    {
        if (_activeState != null)
            _activeState.ExitState();

        _activeState = newState;

        _activeState.EnterState();
        _activeState.OnRequestStateChange += ChangeState;
    }

    protected void SetComponentReferences()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _bulletSpawner = GetComponent<BulletSpawner>();
        _entityStats = GetComponent<EntityStats>();
    }

    protected void HandleDeath()
    {
        PoolManager.Despawn(this);
    }
}