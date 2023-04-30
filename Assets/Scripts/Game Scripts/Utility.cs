using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityHealth
{
    public void Heal(int ammount);
    public void Damage(int ammount);
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
            if (OnTimerExpired != null)
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