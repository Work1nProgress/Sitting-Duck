using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInput : GenericSingleton<ControllerInput>
{
    [HideInInspector]
    public UnityEvent<bool> OnMouseClick;

    [HideInInspector]
    public UnityEvent<bool> OnShootClick;


    bool startWasInDeadZone;

    void OnMove(InputValue value)
    {
        var isUp = value.Get<float>() == 1;
        if (isUp)
        {
            if (Mouse.current.position.value.y >= Screen.height -40f)
            {
                startWasInDeadZone = true;
            }
            else
            {
                OnMouseClick.Invoke(isUp);
            }
        }
        else
        {
            if (!startWasInDeadZone)
            {
                OnMouseClick.Invoke(isUp);
            }
            startWasInDeadZone = false;
        }
       
        
    }

    void OnShoot(InputValue value)
    {
        var isUp = value.Get<float>() == 1;
        if (isUp)
        {
            if (Mouse.current.position.value.y >= Screen.height - 40f)
            {
                startWasInDeadZone = true;
            }
            else
            {
                OnShootClick.Invoke(isUp);
            }
        }
        else
        {
            if (!startWasInDeadZone)
            {
                OnShootClick.Invoke(isUp);
            }
            startWasInDeadZone = false;
        }


    }
}

   
