using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDrones : MonoBehaviour
{


    [SerializeField]
    Weapons[] Weapons;

    int currentWeaponIndex;

    List<DroneBase> drones = new List<DroneBase>();
    public void Init()
    {
        currentWeaponIndex = 0;
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
    }


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

    public List<DroneSettings> Drones;


}

[System.Serializable]
public enum WeaponType {

    //player weapons
    Rifle,
    Shotgun,
    Chainsaw
}

[System.Serializable]
public class DroneSettings{

    public Vector3 OffsetPosition;
    public float OffsetRotation;
}


    
