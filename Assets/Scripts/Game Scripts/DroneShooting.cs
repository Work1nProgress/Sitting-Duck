using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShooting : DroneBase
{

    [SerializeField]
    float ShootSpeed;
    [SerializeField]
    protected float BulletLifetime;
    [SerializeField]
    float AnimateBeforeShotTime;
    protected CountdownTimer ShootingTimer;
    protected CountdownTimer AnimationTimer;



    public override void Init(Vector3 offset, Quaternion rotationOffset)
    {
        base.Init(offset, rotationOffset);
        ShootingTimer = new CountdownTimer(ShootSpeed, false, true);
        AnimationTimer = new CountdownTimer(ShootSpeed-AnimateBeforeShotTime, false, false);

        ShootingTimer.OnTimerExpired += OnShoot;
        AnimationTimer.OnTimerExpired += OnBeforeFire;
    }

    protected virtual void OnShoot() {

        ShootBullet();
        AnimationTimer.Resume();
    }

    protected override void Update()
    {
        base.Update();
        ShootingTimer.Update(Time.deltaTime);
        AnimationTimer.Update(Time.deltaTime);
    }

    protected virtual void ShootBullet()
    {
        BulletManager.Instance.RequestBullet(BulletType.Player, transform.position, transform.up, transform.localEulerAngles.z, BulletLifetime);
    }


    void OnBeforeFire()
    {
        Animator.SetTrigger("Shoot");
    }

    public override void Destroy()
    {
        ShootingTimer.OnTimerExpired -= OnShoot;
        AnimationTimer.OnTimerExpired -= OnBeforeFire;
        base.Destroy();
    }

}
