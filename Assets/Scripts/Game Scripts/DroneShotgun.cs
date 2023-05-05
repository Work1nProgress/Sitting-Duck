using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShotgun : DroneShooting
{
    [SerializeField]
    int numberOfBullers;
    [SerializeField]
    float arcDegrees;

    protected override void ShootBullet()
    {

        float angleStep = arcDegrees / numberOfBullers;
        float angle =-(numberOfBullers/2)*angleStep;
        for (int i = 0; i < numberOfBullers; i++)
        {
            BulletManager.Instance.RequestBullet(BulletType.Player, transform.position, Quaternion.Euler(0, 0, angle)* transform.up , transform.localEulerAngles.z+angle, BulletLifetime);
            angle += angleStep;
        }
    }
}
