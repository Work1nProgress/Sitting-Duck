using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInput : GenericSingleton<ControllerInput>
{
    [HideInInspector]
    public UnityEvent<bool> OnMouseClick;


    bool startWasInDeadZone;

    void OnMove(InputValue value)
    {
        var isUp = value.Get<float>() == 1;
        if (isUp)
        {
            if (Mouse.current.position.value.y >= 648)
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
}

   
