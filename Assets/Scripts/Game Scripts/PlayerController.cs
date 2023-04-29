using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Vector2 StartPos, InputDirection;

    Rigidbody2D body;

    [SerializeField]
    float MaxAcceleration, AccelerationMultiplier, Damping;

    [SerializeField]
    [Range(0,50f)]
    [Tooltip("Values larger than 50 are a no no")]
    float MaxSteering;

    float angle;
    int steerDirection = 0;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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
            InputDirection = -(StartPos - Mouse.current.position.value).normalized * Mathf.Min((StartPos - Mouse.current.position.value).magnitude * AccelerationMultiplier, MaxAcceleration);

            steerDirection = -(int)Mathf.Sign(Vector2.Dot(body.transform.right, InputDirection));
          
            angle = Vector2.Angle(body.transform.up, InputDirection);
          
        
            body.velocity = Mathf.Min(InputDirection.magnitude, MaxAcceleration) * body.transform.up.normalized;
        }

    }

    private void FixedUpdate()
    {

        var change = steerDirection * angle * Time.fixedDeltaTime * MaxSteering;
        angle -= Mathf.Abs(change);
        body.MoveRotation(Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + change));
        body.velocity = body.velocity.magnitude * body.transform.up; 
    }




}
