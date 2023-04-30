using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerGame : GenericSingleton<ControllerGame>
{


    PlayerController playerController;

    private void Start()
    {
        var player = PoolManager.Spawn<PlayerController>("Player", null);
      
    }



}
