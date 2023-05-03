using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPSource : PoolObject
{
    private int _experience;
    private bool _despawn = false;

    private CountdownTimer _despawnTimer;

    public void Initialize(int xp, float lifespan)
    {
        _despawn = false;
        _experience = xp;

        _despawnTimer = new CountdownTimer(lifespan, false, false);
        _despawnTimer.OnTimerExpired += LifetimeExpired;
}

    public int Pickup()
    {
        if (_despawn)
            return 0;
        
        _despawn = true;
        return _experience;
    }

    private void LifetimeExpired()
    {
        _despawn = true;
    }

    void Update()
    {
        if (_despawn)
            PoolManager.Despawn(this);

        _despawnTimer.Update(Time.deltaTime);
    }
}
