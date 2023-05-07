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

    [SerializeField] private int baseShotgunBullets = 5;


    

    public ControllerDrones Init()
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
        return this;
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

        var droneAmount = 0;
        switch (currentWeaponIndex)
        {
            case 0:
                droneAmount = ControllerGame.Instance.RifleDroneNumber;
                break;

            case 1:
                droneAmount = ControllerGame.Instance.ShotgunDroneNumber;
                break;

            case 2:
                droneAmount = ControllerGame.Instance.ChainsawDroneNumber;
                break;

        }
        for (int i = 0; i < droneAmount; i++)
        {
            var drone = PoolManager.Spawn<DroneBase>($"Drone{weapon.WeaponType}", null, ControllerGame.Instance.PlayerPosition);
            drone.Init(ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].OffsetPosition[i],
                Quaternion.Euler(0,0, ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].OffsetRotation[i]));
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
    public void Refresh()
    {
        var weapon = Weapons[currentWeaponIndex];

        var droneAmount = 0;
        switch (currentWeaponIndex)
        {
            case 0:
                droneAmount = ControllerGame.Instance.RifleDroneNumber;
                break;

            case 1:
                droneAmount = ControllerGame.Instance.ShotgunDroneNumber;
                break;

            case 2:
                droneAmount = ControllerGame.Instance.ChainsawDroneNumber;
                break;

        }
        if (drones.Count > droneAmount)
        {
            int i = drones.Count - 1;
            while (drones.Count > droneAmount)
            {
                PoolManager.Despawn(drones[i]);
                drones.RemoveAt(i);
                i--;
            }
        }
        else
        {
            for (int i = drones.Count; i < droneAmount; i++)
            {
                var drone = PoolManager.Spawn<DroneBase>($"Drone{weapon.WeaponType}", null, ControllerGame.Instance.PlayerPosition);
                drone.Init(ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].OffsetPosition[i],
                    Quaternion.Euler(0, 0, ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].OffsetRotation[i]));
                drones.Add(drone);
            }
        }
    }

    int BurstShots
    {
        get
        {
            int bursts = ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].BurstAmount;
            if (Weapons[currentWeaponIndex].WeaponType == WeaponType.Rifle)
            {

                bursts = ControllerGame.Instance.BulletNumber;
            }
            return bursts;
        }

    }

    int NumberOfBullets
    {
        get
        {
            var numberOfBullets = ControllerGame.Instance.DroneShootSettings[currentWeaponIndex].NumberOfBullets;

            if (Weapons[currentWeaponIndex].WeaponType == WeaponType.Shotgun)
            {
                numberOfBullets = ControllerGame.Instance.GetUpgradeValueInt(UpgradeType.BulletAmount) + baseShotgunBullets;
            }
            return numberOfBullets;

        }

    }

    #region Shooting
    public void StartShot()
    {
       


        burstCounter = 1;
        if (BurstShots > 1)
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
        if (burstCounter >= BurstShots)
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

      



        if (NumberOfBullets == 0)
        {
            return;
        }
        float angleStep = settings.ArcDegrees / NumberOfBullets;
       

        foreach (var drone in drones)
        {
            float angle = -(NumberOfBullets / 2) * angleStep;
            for (int i = 0; i < NumberOfBullets; i++)
            {
                BulletManager.Instance.RequestBullet(
                    BulletType.Player,
                    drone.transform.position,
                    Quaternion.Euler(0, 0, angle) * drone.transform.up,
                    drone.transform.localEulerAngles.z + angle,
                    settings.BulletLifetime, ControllerGame.Instance.BulletSize);
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
    public UpgradeType DroneType;



}

[System.Serializable]
public enum WeaponType {

    //player weapons
    Rifle,
    Shotgun,
    Chainsaw
}


    
