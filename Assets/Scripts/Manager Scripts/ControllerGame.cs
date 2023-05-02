using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;

public class ControllerGame : GenericSingleton<ControllerGame>
{


    PlayerController playerController;
    public PlayerController Player => playerController;

    public Vector2 PlayerPosition => new Vector2(Player.transform.position.x, Player.transform.position.y);

    
     

    #region Debug settings
    public bool RotateTowardsArrow;

    public bool AllowReverse;

    
    public float ReverseMinAngle = 91f;

    [Range(0, 180f)]
    public float DeadZone = 90f;

    [Range(0, 50f)]
    public float LookSpeed = 1f;


    public void ToggleRotateTowardsArrow()
    {
        RotateTowardsArrow = !RotateTowardsArrow;
    }

    public void ToggleReverse()
    {
        AllowReverse = !AllowReverse;
    }

    #endregion

   
   
    private void Start()
    {
        playerController = PoolManager.Spawn<PlayerController>("Player", null);
        Camera.main.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().Follow = playerController.transform;

       
        GetComponent<ControllerDrones>().Init();
    }






}
