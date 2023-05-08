using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : PoolObject
{
    private bool _despawn = false;

    [SerializeField]
    float MaxMagnetDistance;

    private CountdownTimer _despawnTimer;

    public void Initialize(float lifespan)
    {
        _despawn = false;

        _despawnTimer = new CountdownTimer(lifespan, false, false);
        _despawnTimer.OnTimerExpired += LifetimeExpired;
    }

    public void PickupObject()
    {
        if (_despawn)
            return;

        _despawn = true;

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
            var move = (ControllerGame.Instance.PlayerController.transform.position - transform.position);
            move = new Vector3(move.x, move.y, 0);
            if (distance > 0)
            {
                transform.position += move.normalized * Mathf.Min(Time.deltaTime * ControllerGame.Instance.MagnetStrength / distance, distance);
            }

        }


        _despawnTimer.Update(Time.deltaTime);
    }
}
