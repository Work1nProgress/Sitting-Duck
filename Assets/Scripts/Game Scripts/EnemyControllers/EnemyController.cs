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

    [SerializeField]
    float chanceToDropHeart;



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

    protected void HandleDeath(EntityStats stats)
    {
        _entityStats.Heal(1000);
        _entityStats._canHealthChange = false;
        if (Random.value < chanceToDropHeart)
        {
            HeartSource heart = PoolManager.Spawn<HeartSource>("HeartOrb", null, transform.position);
            heart.Initialize(ControllerGame.Instance.OrbDuration);
        }
        else
        {
            XPSource xpOrb = PoolManager.Spawn<XPSource>("XPOrb", null, transform.position);
            xpOrb.Initialize(ControllerGame.Instance.OrbDuration);
            xpOrb.SetExp(stats.GetExperienceValue);

        }
       
        ChangeState(_deathState);

    }

    protected void DespawnSelf()
    {
        _entityStats._canHealthChange = true;
        PoolManager.Despawn(this);
    }
}