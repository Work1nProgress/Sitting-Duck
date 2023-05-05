using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRifle : DroneShooting
{

    [SerializeField]
    float burstDelay;

    [SerializeField]
    int burstAmount;

    CountdownTimer BurstTimer;

    int burstCounter;

    public override void Init(Vector3 offset, Quaternion rotationOffset)
    {
        base.Init(offset, rotationOffset);
        BurstTimer = new CountdownTimer(burstDelay, true, false);
        BurstTimer.OnTimerExpired += OnBurstShoot;


    }

    protected override void OnShoot()
    {

        burstCounter = 0;
        BurstTimer.Resume();
        AnimationTimer.Resume();
    }

    protected override void Update()
    {

        BurstTimer.Update(Time.deltaTime);
        base.Update();
    }

    void OnBurstShoot()
    {
        ShootBullet();
        burstCounter++;
        if (burstCounter >= burstAmount - 1)
        {
            BurstTimer.Pause();
        }
        else
        {
            BurstTimer.Resume();
        }
    }

    public override void Destroy()
    {
        BurstTimer.OnTimerExpired -= OnBurstShoot;
        base.Destroy();
    }

}
