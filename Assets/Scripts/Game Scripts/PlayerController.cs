using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Vector2 StartPos;

    Rigidbody2D rb2d;

    [SerializeField]
    float MaxForce, ForceMultiplier;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ControllerInput.Instance.OnMouseClick.AddListener(OnMouseClick);
    }
   

    private void OnDestroy()
    {
        ControllerInput.Instance.OnMouseClick.RemoveListener(OnMouseClick);
    }

    void OnMouseClick(bool value)
    {
        if (value)
        {
            StartPos = Mouse.current.position.value;
        }
        else
        {

            rb2d.AddForce(-(StartPos - Mouse.current.position.value).normalized * Mathf.Min((StartPos - Mouse.current.position.value).magnitude* ForceMultiplier, MaxForce));
        }

    }

   

    
}
