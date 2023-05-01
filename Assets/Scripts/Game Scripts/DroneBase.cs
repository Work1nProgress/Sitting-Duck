using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBase : PoolObject
{


    
    Vector3 offset;

    bool isInitialized = false;

    Vector3 targetPosition;
    Quaternion targetRotation;

    [SerializeField]
    float followRate;

    float MaxDistance = 5;

    BulletSpawner bulletSpawner;


    public void Init(Vector3 offset)
    {
        this.offset = offset;
        isInitialized = true;
        bulletSpawner = GetComponent<BulletSpawner>();
        bulletSpawner.BeginFiring();
        //Time.timeScale = 0.1f;
    }


    private void Update()
    {
        if (isInitialized)
        {
            targetPosition = ControllerGame.Instance.Player.transform.position + (ControllerGame.Instance.Player.transform.rotation * offset);
            targetRotation = ControllerGame.Instance.Player.transform.rotation;

            var distanceT = Mathf.Min(1,Vector3.Distance(transform.position, targetPosition) / MaxDistance);
            var t = followRate * distanceT;
            transform.position = Vector3.Lerp(transform.position, targetPosition, t * Time.deltaTime);
            transform.rotation = targetRotation;//Quaternion.Lerp(transform.rotation, targetRotation, t * Time.deltaTime);
        }


    }

}
