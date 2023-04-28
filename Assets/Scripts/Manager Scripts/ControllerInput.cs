using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInput : GenericSingleton<ControllerInput>
{
    [HideInInspector]
    public UnityEvent<bool> OnMouseClick;


    void OnMove(InputValue value)
    {
        OnMouseClick.Invoke(value.Get<float>() == 1);
    }
}
