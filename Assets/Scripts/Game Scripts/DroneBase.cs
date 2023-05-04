using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBase : PoolObject
{


    
    Vector3 offset;
    Quaternion rotationOffset;

    bool isInitialized = false;

    Vector3 targetPosition;
    Quaternion targetRotation;

    [SerializeField]
    float followRate;

    float MaxDistance = 5;


    [SerializeField]
    Animator Animator;


    public void Init(Vector3 offset, Quaternion rotationOffset)
    {
        this.offset = offset;
        this.rotationOffset = rotationOffset;
        isInitialized = true;
        Animator.SetFloat("Offset", Random.value);
        //Time.timeScale = 0.1f;
    }


    private void Update()
    {
        if (isInitialized)
        {
            targetPosition = ControllerGame.Instance.Player.transform.position + (ControllerGame.Instance.Player.transform.rotation * offset);
            targetRotation = ControllerGame.Instance.Player.transform.rotation * rotationOffset;

            var distanceT = Mathf.Min(1,Vector3.Distance(transform.position, targetPosition) / MaxDistance);
            var t = followRate * distanceT;
            transform.position = Vector3.Lerp(transform.position, targetPosition, t * Time.deltaTime);
            transform.rotation = targetRotation;//Quaternion.Lerp(transform.rotation, targetRotation, t * Time.deltaTime);
        }


    }

    public void OnFire()
    {
        Animator.SetTrigger("Shoot");
    }

}
