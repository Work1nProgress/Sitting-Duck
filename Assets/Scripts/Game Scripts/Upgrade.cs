using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public UpgradeType upgradeType;
    public float Amount;
    public bool Capstone;
    public Sprite sprite;
    public string Description;
}

public enum UpgradeType
{
    None,
    BulletDamage,
    MeeleDamage,
    BulletAmount,
    BulletSize,
    MagnetStrength,

    DroneRifle,
    DroneShotgun,
    DroneChainSaw


}