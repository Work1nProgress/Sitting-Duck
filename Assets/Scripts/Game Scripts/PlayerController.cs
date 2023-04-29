using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Vector2 StartPos, Steering, CurrentSteering, InputDirection;

    Rigidbody2D body;

    [SerializeField]
    float MaxAcceleration, AccelerationMultiplier, MaxSteering, SteeringMultiplier, Damping;

    float currentAcceleration;
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
            currentAcceleration = InputDirection.magnitude;
            steerDirection = -(int)Mathf.Sign(Vector2.Dot(body.transform.right, InputDirection));
            var angle1 = Vector2.Angle(body.transform.up, InputDirection);
            var angle2 = Vector2.Angle(-body.transform.up, InputDirection);
            Debug.Log($"{angle1} {angle2} {Vector2.Dot(body.transform.up, InputDirection)} {steerDirection}");

        }

    }

    private void Update()
    {
        float direction = Vector2.Dot(body.transform.up, InputDirection);
        var angle = 0f;
        if (direction > 0)
        {
            angle = Vector2.Angle(body.transform.up, InputDirection);
        }
        else
        {
            angle = Vector2.Angle(-body.transform.up, InputDirection);
        }
        //var angle1 = Vector2.Angle(body.transform.up, InputDirection);
        //var angle2 = Vector2.Angle(-body.transform.up, InputDirection);
        //Debug.Log($"{angle1} {angle2}");
        //var angle = Mathf.Min(angle1, angle2);


        //// Debug.Log($"{angle} {angle * Time.deltaTime}");
        body.MoveRotation(Quaternion.Euler( 0, 0,transform.rotation.eulerAngles.z + steerDirection *angle * Time.deltaTime * MaxSteering));
        var velocity = body.velocity;
        //(body.transform.up) * Mathf.Min(currentAcceleration, MaxAcceleration);
         body.velocity = new Vector2(velocity.x, velocity.y);
         currentAcceleration *= Damping * Time.deltaTime;
    }




}
