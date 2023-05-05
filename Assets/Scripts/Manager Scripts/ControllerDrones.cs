using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDrones : MonoBehaviour
{


    [SerializeField]
    Weapons[] Weapons;

    int currentWeaponIndex;

    List<DroneBase> drones = new List<DroneBase>();

    CountdownTimer ShootingTimer;
    CountdownTimer AnimationTimer;

    CountdownTimer BurstTimer;
    CountdownTimer[] timers;
    int burstCounter;


    

    public void Init()
    {
        currentWeaponIndex = 0;
      


        ShootingTimer = new CountdownTimer(0, true, false);
        AnimationTimer = new CountdownTimer(0, true, false);
        BurstTimer = new CountdownTimer(0, true, false);

        ShootingTimer.OnTimerExpired += StartShot;
        AnimationTimer.OnTimerExpired += Animate;
        BurstTimer.OnTimerExpired += BurstShot;
        timers = new CountdownTimer[]{
            ShootingTimer,
            AnimationTimer,
            BurstTimer

        };
        ChangeWeapon();

        ControllerInput.Instance.OnWeaponChange.AddListener(OnChangeWeaponCallback);
    }

    void ChangeWeapon()
    {

        if (drones.Count > 0)
        {
            for (int i = drones.Count-1; i >= 0; i--)
            {
                PoolManager.Despawn(drones[i]);
            }
            drones.Clear();
        }
        var weapon = Weapons[currentWeaponIndex];
        for (int i = 0; i < weapon.NumberOfDrones; i++)
        {
            var drone = PoolManager.Spawn<DroneBase>($"Drone{weapon.WeaponType}", null, ControllerGame.Instance.PlayerPosition);
            drone.Init(weapon.Drones[i].OffsetPosition, Quaternion.Euler(0,0,weapon.Drones[i].OffsetRotation));
            drones.Add(drone);
        }
        var settings = ControllerGame.Instance.DroneShootSettings[currentWeaponIndex];

        ShootingTimer.SetNewTime(settings.ShootSpeed);
        BurstTimer.SetNewTime(settings.BurstDelay);
        AnimationTimer.SetNewTime(settings.ShootSpeed - settings.AnimateBeforeShotTime);


        ShootingTimer.Resume();

        AnimationTimer.Resume();
        BurstTimer.Pause();
    }

    private void Update()
    {
        foreach (var t in timers)
        {
            if (t != null)
            {
                t.Update(Time.deltaTime);
            }
        }
    }


    #region Shooting
    public void StartShot()
    {
        burstCounter = 1;
        if (ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].BurstAmount > 1)
        {
          
            BurstTimer.Resume();
        }

        ShootingTimer.Resume();
        AnimationTimer.Reset();
        AnimationTimer.Resume();
        Shoot();

    }
    public void BurstShot()
    {
        Shoot();
        burstCounter++;
        if (burstCounter >= ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].BurstAmount)
        {
            BurstTimer.Pause();
        }
        else
        {
            BurstTimer.Resume();
        }
    }

    public void Shoot()
    {
        var settings = ControllerGame.Instance.DroneShootSettings[currentWeaponIndex];
        if (settings.NumberOfBullets == 0)
        {
            return;
        }
        float angleStep = settings.ArcDegrees / settings.NumberOfBullets;
       

        foreach (var drone in drones)
        {
            float angle = -(settings.NumberOfBullets / 2) * angleStep;
            for (int i = 0; i < settings.NumberOfBullets; i++)
            {
                BulletManager.Instance.RequestBullet(BulletType.Player, drone.transform.position, Quaternion.Euler(0, 0, angle) * drone.transform.up, drone.transform.localEulerAngles.z + angle, settings.BulletLifetime);
                angle += angleStep;
            }
        }
        SoundManager.Instance.Play(settings.ShootSFX);

    }

    public void Animate()
    {

        foreach (var d in drones)
        {
            d.Animate();
        }

    }
    #endregion


    void OnChangeWeaponCallback(int direction)
    {
        currentWeaponIndex += direction;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = Weapons.Length - 1;
        }
        if (currentWeaponIndex >= Weapons.Length)
        {
            currentWeaponIndex = 0;
        }
        ChangeWeapon();

    }

    private void OnDestroy()
    {
        ControllerInput.Instance.OnWeaponChange.RemoveListener(OnChangeWeaponCallback);
    }

    
}


[System.Serializable]
public class Weapons
{
    public WeaponType WeaponType;
    public int NumberOfDrones;

    public List<DronePositionSettings> Drones;


}

[System.Serializable]
public enum WeaponType {

    //player weapons
    Rifle,
    Shotgun,
    Chainsaw
}

[System.Serializable]
public class DronePositionSettings{

    public Vector3 OffsetPosition;
    public float OffsetRotation;
}


    
