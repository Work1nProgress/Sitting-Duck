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
    public UnityEvent<int> OnWeaponChange;


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

   

    void OnNextWeapon(InputValue value)
    {
        OnWeaponChange.Invoke(1);
    }

    void OnChangeWeapon(InputValue value)
    {
        if (value.Get<Vector2>().y > 0)
        {
            OnWeaponChange.Invoke(1);
        }
        else if(value.Get<Vector2>().y < 0)
        {
            OnWeaponChange.Invoke(-1);
        }
    }
}

   
