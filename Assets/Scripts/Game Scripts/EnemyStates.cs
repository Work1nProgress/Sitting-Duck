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

    public virtual void InitializeState() { _stateTimer.OnTimerExpired += TimeExpired; }

    public virtual void EnterState() { _stateTimer.Reset(); }

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
        public float TimeInState;
        public string stateName;

        public EnemyStateData(
        Transform target,
        Rigidbody2D rigidBody,
        BulletSpawner bulletSpawner,
        EnemyState[] transitionStates,
        float TimeInState,
        string stateName)
        {
            this.target = target;
            this.rigidBody = rigidBody;
            this.bulletSpawner = bulletSpawner;
            this.transitionStates = transitionStates;
            this.TimeInState = TimeInState;
            this.stateName = stateName;
        }
    }
}

public class ChargingEnemyState : EnemyState
{
    public ChargingEnemyState(EnemyStateData data)
    {
        _target = data.target;
        _rigidBody = data.rigidBody;
        _bulletSpawner = data.bulletSpawner;
        _transitionStates = data.transitionStates;
        _stateTimer = new CountdownTimer(data.TimeInState, false, false);
        _stateName = data.stateName;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void TimeExpired()
    {
        Debug.Log("Change state from: " + _stateName);
        InvokeStateChangeRequest(_transitionStates[0]);
    }
}

public class CountdownTimer
{
    private float _timeLeft;
    private float _time;
    private bool _timerPaused;
    private bool _restartOnExpiration;

    public delegate void TimerExpiredSignature();
    public event TimerExpiredSignature OnTimerExpired;

    public CountdownTimer(float timerSeconds, bool shouldStartPaused, bool shouldLoop)
    {
        _timeLeft = timerSeconds;
        _time = timerSeconds;
        _timerPaused = shouldStartPaused;
        _restartOnExpiration = shouldLoop;
    }

    public void Update(float deltaTime)
    {
        if (!_timerPaused)
            _timeLeft -= deltaTime;

        if (_timeLeft <= 0)
        {
            if(OnTimerExpired != null)
            OnTimerExpired.Invoke();

            Reset();

            if (!_restartOnExpiration)
                Pause();
        }
    }
    public void Pause() { _timerPaused = true; }
    public void Resume() { _timerPaused = false; }
    public void Reset() { _timeLeft = _time; }
    public void SetShouldLoop(bool loop) { _restartOnExpiration = loop; }
    public void SetNewTime(float time)
    {
        _time = time;
        _timeLeft = time;
    }
    public void StepTime(float step)
    {
        float newTime = _timeLeft + step;

        if (newTime > _time)
        {
            Reset();
            return;
        }
        else if (newTime <= 0)
        {
            Update(0);
            return;
        }
        else
            _timeLeft = newTime;
    }
}
