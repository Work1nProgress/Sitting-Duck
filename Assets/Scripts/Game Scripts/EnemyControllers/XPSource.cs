using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPSource : PoolObject
{
    private int _experience;
    private bool _despawn = false;

    [SerializeField]
    float MaxMagnetDistance, MagnetStrenght;

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

        var distance = Vector2.Distance(transform.position, ControllerGame.Instance.PlayerPosition);
        if (distance < MaxMagnetDistance)
        {
            var move = (ControllerGame.Instance.Player.transform.position - transform.position);
            move = new Vector3(move.x, move.y, 0);
            if (distance > 0)
            {
                move = move.normalized* Mathf.Min(Time.deltaTime * MagnetStrenght / distance, distance);
            }
            
        }


        _despawnTimer.Update(Time.deltaTime);
    }
}
