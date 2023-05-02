using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PoolObject
{

    Vector2 StartPos, InputDirection;

    Rigidbody2D body;

    [SerializeField]
    float MaxAcceleration, AccelerationMultiplier, MaxCollisionRidealong;

    public float GetMaxAcceleration => MaxAcceleration;
    public float GetAccelerationMultiplier => AccelerationMultiplier;



    [SerializeField]
    [Range(0,50f)]
    [Tooltip("Values larger than 50 are a no no")]
    float MaxSteering;

    float angle;
    int steerDirection = 0;
    bool reversing = false;

    Vector3 lastPos;

    bool waitingForRelease = false;
    bool canInteractWithCollider;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ControllerInput.Instance.OnMouseClick.AddListener(OnMouseClick);
       
        waitingForRelease = false;
        canInteractWithCollider = true;
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

            SetVelocityForward(Mathf.Min(InputDirection.magnitude, MaxAcceleration));
          
            waitingForRelease = false;
            canInteractWithCollider = true;
        }

    }


 


    void SetVelocityForward(float magnitude)
    {
        if (ControllerGame.Instance.AllowReverse && reversing)
        {
            body.velocity = magnitude * (-body.transform.up);
        }
        else
        {
            body.velocity = magnitude * body.transform.up;

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
 

        var change = rotateFromCollision ? changeFromCollision : steerDirection * angle * Time.fixedDeltaTime * MaxSteering;
        if (rotateFromCollision)
        {
            angle = 0;
        }
        else
        {
            angle -= Mathf.Abs(change);
        }
        rotateFromCollision = false;
        
        body.MoveRotation(Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + change));
        SetVelocityForward(body.velocity.magnitude);


    }


    bool rotateFromCollision = false;
    float changeFromCollision = 0;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!canInteractWithCollider)
        {
            HandleCollistion(collision);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollistion(collision);
    }

    void HandleCollistion(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                canInteractWithCollider = false;
                var point = contact.point - contact.normal;
                var dir = -contact.normal;

                RaycastHit2D hitInfo = Physics2D.Raycast(point, dir, 1, LayerMask.GetMask("Walls"));
                var normal = hitInfo.normal;

                var dotRight = Vector2.Dot(normal, body.transform.right);
                if (Mathf.Abs(dotRight) > MaxCollisionRidealong)
                {

                    var ridealongDirection = Mathf.Sign(dotRight);
                    var tangent = new Vector2(-normal.y, normal.x) * ridealongDirection;
                    changeFromCollision = Vector2.SignedAngle(body.transform.up, tangent);
                    rotateFromCollision = true;
                }
                
            }

        }
    }

   




}
