using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController _controller;
    protected Transform _target;
    protected Transform _transform;
    protected Animator _animator;
    protected EnemyState[] _transitionStates;
    protected CountdownTimer _stateTimer;
    protected string _stateName;

    public delegate void StateChangeSignature(EnemyState state);
    public event StateChangeSignature OnRequestStateChange;
    protected void InvokeStateChangeRequest(EnemyState newState) { if(OnRequestStateChange != null)OnRequestStateChange.Invoke(newState); }

    public virtual void InitializeState(EnemyStateData data)
    {
        _controller = data.controller;
        _target = data.target;
        _transform = data.transform;
        _animator = data.animator;
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

    public virtual void LookAtPosition(Vector3 position)
    {
        Vector3 positionDifference = (position - _transform.position).normalized;
        float angle = Mathf.Atan2(positionDifference.y, positionDifference.x) * Mathf.Rad2Deg;
        _transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public virtual void MoveTowardsPosition(Vector3 position, float speed)
    {
        _transform.position = _transform.position +
                (position - _transform.position).normalized *
                speed * 0.1f;
    }

    public struct EnemyStateData
    {
        public EnemyController controller;
        public Transform target;
        public Transform transform;
        public Animator animator;
        public EnemyState[] transitionStates;
        public float timeInState;
        public bool timerStartsPaused;
        public string stateName;

        public EnemyStateData(
        EnemyController controller,
        Transform target,
        Transform transform,
        Animator animator,
        EnemyState[] transitionStates,
        float timeInState,
        bool timerStartsPaused,
        string stateName)
        {
            this.controller = controller;
            this.target = target;
            this.transform = transform;
            this.transitionStates = (EnemyState[]) transitionStates.Clone();
            this.timeInState = timeInState;
            this.timerStartsPaused = timerStartsPaused;
            this.stateName = stateName;
            this.animator = animator;
        }
    }
}

public class ChargingEnemyState : EnemyState
{
    float _delay;
    float _chargeSpeed;
    float _chargeTime;

    CountdownTimer _chargeDelay;
    CountdownTimer _chargeDurationTimer;
    Vector3 _chargePoint;

    Vector3 _positionLastFrame = Vector3.zero;

    bool _charging;

    public ChargingEnemyState(float delay, float speed, float duration)
    {
        _delay = delay;
        _chargeSpeed = speed;
        _chargeTime = duration;

        _chargeDelay = new CountdownTimer(delay, false, false);
        _chargeDelay.OnTimerExpired += BeginCharge;

        _chargeDurationTimer = new CountdownTimer(duration, true, false);
        _chargeDurationTimer.OnTimerExpired += EndCharge;
    }

    public override void EnterState()
    {
        base.EnterState();

        _chargePoint = _target.position;
        _chargeDurationTimer.Reset();
        Debug.Log("windupstate", _transform);
        _animator.SetBool("ChargeWindup", true);

        if (_delay > 0)
        {
            _chargeDelay.SetNewTime(_delay);
            _chargeDelay.Reset();
            _chargeDelay.Resume();
        }
        else BeginCharge();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        _chargeDelay.Update(Time.deltaTime);
        _chargeDurationTimer.Update(Time.deltaTime);

        if (_charging && (_chargePoint - _transform.position).magnitude < 0.5f)
            EndCharge();

        _positionLastFrame = _transform.position;
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (_charging)
            MoveTowardsPosition(_chargePoint, _chargeSpeed);
    }

    public override void TimeExpired()
    {
        InvokeStateChangeRequest(_transitionStates[0]);
    }

    private void BeginCharge()
    {
        _charging = true;
        _animator.SetBool("ChargeWindup", false);
        _animator.SetBool("Charge", true);
        _chargeDurationTimer.Resume();
    }

    private void EndCharge()
    {
        _charging = false;
        _chargeDurationTimer.Reset();
        _chargeDurationTimer.Pause();
        _animator.SetBool("ChargeWindup", false);
        _animator.SetBool("Charge", false);
        InvokeStateChangeRequest(_transitionStates[0]);
    }
}

public class SmashEnemyState : EnemyState
{
    private int _attackDamage;
    private float _hitRange = 7;
    CountdownTimer _hitDelayTimer;

    public SmashEnemyState(int attackDamage, float damageDelay)
    {
        _attackDamage = attackDamage;
        _hitDelayTimer = new CountdownTimer(damageDelay, false, false);
        _hitDelayTimer.OnTimerExpired += () =>
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _hitRange);
            foreach (Collider2D collider in colliders)
            {
                EntityStats entity = collider.GetComponent<EntityStats>();
                if (entity != null)
                    if (entity.GetEntityType() == EntityType.Player)
                        entity.Damage(_attackDamage);
            }
        };
    }

    public override void EnterState()
    {
        base.EnterState();
        _animator.SetBool("Smash", true);
        _hitDelayTimer.Resume();
    }

    public override void ExitState()
    {
        base.ExitState();
        _animator.SetBool("Smash", false);
        _hitDelayTimer.Reset();
        _hitDelayTimer.Pause();
    }

    public override void TimeExpired()
    {
        base.TimeExpired();
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

        _animator.SetBool("Walk", true);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        _updateTargetPositionTimer.Update(Time.deltaTime);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        MoveTowardsPosition(_targetPosition, _walkSpeed);
        LookAtPosition(_target.position);
    }

    public override void ExitState()
    {
        base.ExitState();

        _animator.SetBool("Walk", false);
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _attackPlayerRadius);

        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject.name == _target.name)
            {
                if (_transitionStates.Length > 1 &&
                    (collider.transform.position - _transform.position).magnitude <= 7)
                {
                    InvokeStateChangeRequest(_transitionStates[1]);
                }

                if( _transitionStates[0] != null)
                    InvokeStateChangeRequest(_transitionStates[0]);
            }
        }
    }
}

public class MeleeAttackEnemyState : EnemyState
{
    private float _windUpTime;
    private float _attackBoxLength = 4;
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

        _animator.SetBool("Attack", true);
        BeginAttack();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        _windUpTimer.Update(Time.deltaTime);
        _hitTimer.Update(Time.deltaTime);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        LookAtPosition(_target.position);
    }

    public override void ExitState()
    {
        base.ExitState();

        _animator.SetBool("Attack", false);
    }

    private void BeginAttack()
    {
        ResetAttackTimers();
        Debug.DrawLine(_transform.position, _transform.position + (_attackBoxLength * _transform.up), Color.yellow, _windUpTime);
    }
    int PlayerLayerMask = LayerMask.GetMask("EnemyBulletTarget");

    private void AttackHit()
    {
        Debug.DrawLine(_transform.position, _transform.position + (_transform.up * _attackBoxLength), Color.red, 0.125f);

        Vector3 something;
        float angle;
        _transform.rotation.ToAngleAxis(out angle, out something);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            _transform.position +
            (_attackBoxLength / 2 * _transform.up),
            new Vector2(_attackBoxwidth, _attackBoxLength),
            angle, PlayerLayerMask);

        foreach(Collider2D collider in colliders)
        {
            IEntityHealth healthComponent = collider.GetComponent<IEntityHealth>();
            if(healthComponent != null &&
                collider.gameObject != _transform.gameObject && collider.gameObject.layer == 12)
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

public class DeathState : EnemyState
{
    public delegate void DeathStateExitSignature();
    public event DeathStateExitSignature OnDeathStateExited;

    public override void EnterState()
    {
        base.EnterState();

        _animator.SetBool("Dead", true);
    }

    public override void TimeExpired()
    {
        InvokeStateChangeRequest(_transitionStates[0]);
        base.TimeExpired();
    }

    public override void ExitState()
    {
        _animator.SetBool("Dead", false);
        base.ExitState();
        if (OnDeathStateExited != null)
            OnDeathStateExited.Invoke();
    }
}