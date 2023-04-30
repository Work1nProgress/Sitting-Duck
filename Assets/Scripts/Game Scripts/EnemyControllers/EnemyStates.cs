using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    protected Transform _target;
    protected Rigidbody2D _rigidBody;
    protected BulletSpawner _bulletSpawner;
    protected EnemyState[] _transitionStates;
    protected CountdownTimer _stateTimer;
    protected string _stateName;

    public delegate void StateChangeSignature(EnemyState state);
    public event StateChangeSignature OnRequestStateChange;
    protected void InvokeStateChangeRequest(EnemyState newState) { OnRequestStateChange.Invoke(newState); }

    public virtual void InitializeState(EnemyStateData data)
    {
        _target = data.target;
        _rigidBody = data.rigidBody;
        _bulletSpawner = data.bulletSpawner;
        _transitionStates = (EnemyState[]) data.transitionStates.Clone();
        _stateTimer = new CountdownTimer(data.timeInState, data.timerStartsPaused, false);
        _stateName = data.stateName;

        _stateTimer.OnTimerExpired += TimeExpired;
    }

    public virtual void EnterState() { _stateTimer.Reset(); _stateTimer.Resume(); }

    public virtual void UpdateState() { _stateTimer.Update(Time.deltaTime); }

    public virtual void FixedUpdateState() { }

    public virtual void ExitState() { OnRequestStateChange = null; }

    public virtual void TimeExpired() { }

    public virtual void DecomissionState() { _stateTimer.OnTimerExpired -= TimeExpired; }

    public struct EnemyStateData
    {
        public Transform target;
        public Rigidbody2D rigidBody;
        public BulletSpawner bulletSpawner;
        public EnemyState[] transitionStates;
        public float timeInState;
        public bool timerStartsPaused;
        public string stateName;

        public EnemyStateData(
        Transform target,
        Rigidbody2D rigidBody,
        BulletSpawner bulletSpawner,
        EnemyState[] transitionStates,
        float timeInState,
        bool timerStartsPaused,
        string stateName)
        {
            this.target = target;
            this.rigidBody = rigidBody;
            this.bulletSpawner = bulletSpawner;
            this.transitionStates = (EnemyState[]) transitionStates.Clone();
            this.timeInState = timeInState;
            this.timerStartsPaused = timerStartsPaused;
            this.stateName = stateName;
        }
    }
}

public class ChargingEnemyState : EnemyState
{
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void TimeExpired()
    {
        InvokeStateChangeRequest(_transitionStates[0]);
    }
}

public class ApproachPlayerEnemyState : EnemyState
{
    private float _walkSpeed;
    private Vector3 _targetPosition;
    private float _attackPlayerRadius;

    private CountdownTimer _updateTargetPositionTimer;


    public ApproachPlayerEnemyState(float walkSpeed, float attackPlayerRadius)
    {
        _walkSpeed = walkSpeed;
        _attackPlayerRadius = attackPlayerRadius;
    }

    public override void InitializeState(EnemyStateData data)
    {
        base.InitializeState(data);

        UpdateTargetPosition();

        _updateTargetPositionTimer = new CountdownTimer(0.125f, false, true);
        _updateTargetPositionTimer.OnTimerExpired += UpdateTargetPosition;
        _updateTargetPositionTimer.OnTimerExpired += CheckForPlayer;
    }

    public override void EnterState()
    {
        base.EnterState();

        _bulletSpawner.StopFiring();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        _updateTargetPositionTimer.Update(Time.deltaTime);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if(_rigidBody != null)
        {
            _rigidBody.MovePosition(_rigidBody.position +
                ((Vector2)_targetPosition - _rigidBody.position).normalized *
                _walkSpeed * 0.1f);

            Quaternion newRotation = Quaternion.LookRotation(_target.transform.position - (Vector3)_rigidBody.position, Vector3.back);

            _rigidBody.SetRotation(newRotation);
        }
    }

    public override void DecomissionState()
    {
        base.DecomissionState();
        _updateTargetPositionTimer.OnTimerExpired -= UpdateTargetPosition;
        _updateTargetPositionTimer.OnTimerExpired -= CheckForPlayer;
    }

    private void UpdateTargetPosition()
    {
        if (_target != null)
            _targetPosition = _target.position;
    }

    private void CheckForPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_rigidBody.position, _attackPlayerRadius);

        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject.name == _target.name)
            {
                if( _transitionStates[0] != null)
                    InvokeStateChangeRequest(_transitionStates[0]);
            }
        }
    }
}

public class MeleeAttackEnemyState : EnemyState
{
    private float _windUpTime;
    private float _attackBoxLength = 5;
    private float _attackBoxwidth = 1;
    private int _damage;

    private CountdownTimer _windUpTimer;
    private CountdownTimer _hitTimer;

    public MeleeAttackEnemyState(float windUpTime, int hitDamage)
    {
        _windUpTime = windUpTime;
        _damage = hitDamage;
    }

    public override void InitializeState(EnemyStateData data)
    {
        base.InitializeState(data);

        _windUpTimer = new CountdownTimer(_windUpTime, true, false);
        _hitTimer = new CountdownTimer(0.125f, true, false);
        _windUpTimer.OnTimerExpired += AttackHit;
        _hitTimer.OnTimerExpired += ChangeToNextState;
    }

    public override void EnterState()
    {
        base.EnterState();

        BeginAttack();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        _windUpTimer.Update(Time.deltaTime);
        _hitTimer.Update(Time.deltaTime);
    }

    private void BeginAttack()
    {
        ResetAttackTimers();
        Debug.DrawLine(_rigidBody.position, (Vector3)_rigidBody.position + (_attackBoxLength / 2 * _rigidBody.transform.up), Color.yellow, _windUpTime);
    }

    private void AttackHit()
    {
        Debug.DrawLine(_rigidBody.position, (Vector3)_rigidBody.position + (_rigidBody.transform.up * _attackBoxLength), Color.red, 0.125f);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            (Vector3)_rigidBody.position +
            (_attackBoxLength / 2 * _rigidBody.transform.up),
            new Vector2(_attackBoxwidth, _attackBoxLength),
            _rigidBody.rotation);

        foreach(Collider2D collider in colliders)
        {
            IEntityHealth healthComponent = collider.GetComponent<IEntityHealth>();
            if(healthComponent != null &&
                collider.gameObject != _rigidBody.gameObject)
            {
                healthComponent.Damage(_damage);
            }
        }
        
        _hitTimer.Resume();
    }


    private void ResetAttackTimers()
    {
        _windUpTimer.Reset();
        _hitTimer.Pause();
        _hitTimer.Reset();
        _windUpTimer.Resume();
    }

    private void ChangeToNextState()
    {
        if (_transitionStates[0] != null)
            InvokeStateChangeRequest(_transitionStates[0]);
    }
}


