using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BulletSpawner))]
public class EnemyController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;
    protected BulletSpawner _bulletSpawner;

    protected EnemyState _activeState;

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
        _rigidbody = GetComponent<Rigidbody2D>();
        _bulletSpawner = GetComponent<BulletSpawner>();
    }
}