using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PoolObject
{

    Vector2 StartPos, InputDirection;

    Rigidbody2D body;

    [SerializeField]
    float MaxAcceleration, AccelerationMultiplier;

    [SerializeField]
    [Range(0,50f)]
    [Tooltip("Values larger than 50 are a no no")]
    float MaxSteering;

    float angle;
    int steerDirection = 0;
    bool reversing = false;

    Vector3 lastPos;

    bool waitingForRelease = false;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ControllerInput.Instance.OnMouseClick.AddListener(OnMouseClick);
        waitingForRelease = false;
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
            waitingForRelease = true;
        }
        else
        {
            var currentPos = Mouse.current.position.value;
            var magnitude = (StartPos - currentPos).magnitude;
            if (Vector3.Angle(currentPos - StartPos, ControllerGame.Instance.Player.transform.up) > ControllerGame.Instance.DeadZone)
            {
                currentPos = lastPos;
            }
            lastPos = currentPos;
            InputDirection = -(StartPos - currentPos).normalized * Mathf.Min(magnitude* AccelerationMultiplier, MaxAcceleration);
            angle = Vector2.Angle(body.transform.up, InputDirection);
            steerDirection = -(int)Mathf.Sign(Vector2.Dot(body.transform.right, InputDirection));
            if (ControllerGame.Instance.AllowReverse)
            {
                Debug.Log($"{angle} {ControllerGame.Instance.ReverseMinAngle} {steerDirection}");
                if (angle > ControllerGame.Instance.ReverseMinAngle)
                {
                    angle = (180-angle);
                    steerDirection *= -1;
                    reversing = true;

                }
                else
                {
                    reversing = false;
                }

            }





            if (ControllerGame.Instance.AllowReverse && reversing)
            {
                body.velocity = Mathf.Min(InputDirection.magnitude, MaxAcceleration) * (-body.transform.up.normalized);
            }
            else
            {
                body.velocity = Mathf.Min(InputDirection.magnitude, MaxAcceleration) * body.transform.up.normalized;

            }
            waitingForRelease = false;
        }

    }

    private void FixedUpdate()
    {

        if (waitingForRelease)
        {
            if (ControllerGame.Instance.RotateTowardsArrow)
            {
                var currentDirection = -(StartPos - Mouse.current.position.value).normalized;
                var currentAngle = Vector2.Angle(body.transform.up, currentDirection);
                var currentSteerDirection = -(int)Mathf.Sign(Vector2.Dot(body.transform.right, currentDirection));
                var currentChange = currentSteerDirection * currentAngle * Time.fixedDeltaTime * ControllerGame.Instance.LookSpeed;
                body.MoveRotation(Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + currentChange));
            }
            return;
        }
 

        var change = steerDirection * angle * Time.fixedDeltaTime * MaxSteering;
        angle -= Mathf.Abs(change);
        body.MoveRotation(Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + change));
        if (ControllerGame.Instance.AllowReverse && reversing)
        {
            body.velocity = body.velocity = body.velocity.magnitude * (-body.transform.up);
        }
        else
        {
            body.velocity = body.velocity = body.velocity.magnitude * body.transform.up;

        }
       
    }




}
