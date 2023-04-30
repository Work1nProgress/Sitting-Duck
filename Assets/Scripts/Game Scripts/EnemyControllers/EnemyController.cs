using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BulletSpawner))]
public class EnemyController : MonoBehaviour, IEntityHealth
{
    [SerializeField] protected int _maxHealth;
    protected int _health;

    protected Rigidbody2D _rigidbody;
    protected BulletSpawner _bulletSpawner;

    protected EnemyState _activeState;

    public delegate void EnemyHealthChangeSignature(int oldHealth, int newHealth, int maxHealth);
    public event EnemyHealthChangeSignature OnHealthChanged;

    public delegate void EntityDeathSignature();
    public event EntityDeathSignature OnDeath;

    protected virtual void Start()
    {
        _health = _maxHealth;

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

    public void Damage(int ammount)
    {
        int newHealth = _health - ammount;

        if(newHealth <= 0)
        {
            _health = 0;

            if (OnDeath != null)
                OnDeath.Invoke();
            
            return;
        }
        else if(newHealth >= _maxHealth)
        {
            if (OnHealthChanged != null)
                OnHealthChanged.Invoke(_health, newHealth, _maxHealth);

            _health = _maxHealth;

            return;
        }
        else
        {
            if (OnHealthChanged != null)
                OnHealthChanged.Invoke(_health, newHealth, _maxHealth);

            _health = newHealth;
        }

    }
    public void Heal(int ammount)
    {
        int newHealth = _health + ammount;

        if (newHealth <= 0)
        {
            _health = 0;
            if (OnDeath != null)
                OnDeath.Invoke();
            return;
        }
        else if (newHealth >= _maxHealth)
        {
            if (OnHealthChanged != null)
                OnHealthChanged.Invoke(_health, newHealth, _maxHealth);
            
            _health = _maxHealth;

            return;
        }
        else
        {
            if (OnHealthChanged != null)
                OnHealthChanged.Invoke(_health, newHealth, _maxHealth);

            _health = newHealth;
        }
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