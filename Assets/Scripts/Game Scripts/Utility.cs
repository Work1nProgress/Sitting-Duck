using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityHealth
{
    public void Heal(int ammount);
    public void Damage(int ammount);
}

public interface IExperience
{
    public int GetExperienceValue();
    public void ChangeExperienceValue(int change);
    public int GetLevel();
    public void IncreaseLevel();
}

public class CountdownTimer
{
    private float _timeLeft;
    private float _time;
    private bool _timerPaused;
    private bool _restartOnExpiration;

    public float TimeLeft => _timeLeft;
    public bool TimerPaused => _timerPaused;

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
        {
            _timeLeft -= deltaTime;

        }
        else
        {
            return;
        }

        if (_timeLeft <= 0)
        {
            if (!_restartOnExpiration)
                Pause();

            if (OnTimerExpired != null)
                OnTimerExpired.Invoke();

            Reset();

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