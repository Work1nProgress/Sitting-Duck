using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityStats))]
public class EnemyController : PoolObject
{

    protected EntityStats _entityStats;

    protected EnemyState _activeState;

    protected DeathState _deathState;

    protected Animator _animator;



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
        _entityStats = GetComponent<EntityStats>();
        _animator = GetComponent<Animator>();
    }

    protected void HandleDeath()
    {
        _entityStats.Heal(1000);
        _entityStats._canHealthChange = false;
        XPSource xpOrb = PoolManager.Spawn<XPSource>("XPOrb", null, transform.position);
        xpOrb.Initialize(_entityStats.GetExperienceValue(), 5);
        ChangeState(_deathState);
    }

    protected void DespawnSelf()
    {
        _entityStats._canHealthChange = true;
        PoolManager.Despawn(this);
    }
}