using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class ArrowMovementView : MonoBehaviour
{

    [SerializeField]
    Image ImageArrowHead, ImageArrowLine;



    bool onMoveState;
    bool onMoveStatePrevious;

    Vector3 startPos;
    Vector3 lastPos;


    private void Awake()
    {
        ControllerInput.Instance.OnMouseClick.AddListener(OnMouseClick);
    }

    private void OnDestroy()
    {
        ControllerInput.Instance.OnMouseClick.RemoveListener(OnMouseClick);
    }

    void OnMouseClick(bool value)
    {
        onMoveState = value;

    }

    private void Update()
    {
        if (onMoveState != onMoveStatePrevious)
        {
            if (onMoveState)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, Mouse.current.position.value, null, out var point);

                startPos = transform.parent.InverseTransformVector(point);
            }
           

            onMoveStatePrevious = onMoveState;
        }

        if (onMoveState)
        {

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, Mouse.current.position.value, null, out var point);

            var currentPos = transform.parent.InverseTransformVector(point);
            var distance = Vector3.Distance(currentPos, startPos);
            var isMaxed = Vector3.Distance(currentPos, startPos) * ControllerGame.Instance.Player.GetAccelerationMultiplier >= ControllerGame.Instance.Player.GetMaxAcceleration;
            if (Vector3.Angle(currentPos - startPos, ControllerGame.Instance.Player.transform.up) > ControllerGame.Instance.DeadZone)
            {
                var right = Mathf.Sign(Vector3.Dot(currentPos - startPos, ControllerGame.Instance.Player.transform.right));
                var lastRecalculated = right == -1 ? 2 * lastPos - startPos : lastPos;
                currentPos = startPos + (lastRecalculated - startPos).normalized * distance;
            }
            else
            {
                lastPos = currentPos;
            }

            if (Vector3.Angle(currentPos - startPos, ControllerGame.Instance.Player.transform.up) > ControllerGame.Instance.ReverseMinAngle)
            {
                var color = isMaxed ? Color.red : Color.yellow;
                ImageArrowLine.color = color;
                ImageArrowHead.color = color;


            }
            else
            {
                var color = isMaxed ? Color.green : Color.white;
                ImageArrowLine.color = color;
                ImageArrowHead.color = color;
            }
            ImageArrowLine.rectTransform.sizeDelta = new Vector2(distance, ImageArrowLine.rectTransform.sizeDelta.y );
            ImageArrowLine.transform.localPosition = Vector3.Lerp(startPos, currentPos, 0.5f);
            var diff = currentPos - startPos;
            ImageArrowLine.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x)*(180f/Mathf.PI));
            ImageArrowHead.transform.localPosition = currentPos - diff.normalized *( ImageArrowHead.sprite.textureRect.height/2+2);
            ImageArrowHead.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * (180f / Mathf.PI) -90);
        


        }
    }





}
