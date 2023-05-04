using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjectTimed : PoolObject
{

    [SerializeField]
    bool autoKill;
    [SerializeField]
    float duration;
    CountdownTimer timer;
    public override void Reuse()
    {
        if (autoKill)
        {
            StartTicking();
        }
    }

    public void StartTicking()
    {
        timer = new CountdownTimer(duration, false, false);
        timer.OnTimerExpired += SelfDestruct;
        base.Reuse();
    }


    void SelfDestruct()
    {
        PoolManager.Despawn(this);
    }
}
