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
            ImageArrowLine.rectTransform.sizeDelta = new Vector2(Vector3.Distance(currentPos, startPos), ImageArrowLine.rectTransform.sizeDelta.y );
            ImageArrowLine.transform.localPosition = Vector3.Lerp(startPos, currentPos, 0.5f);
            var diff = currentPos - startPos;
            ImageArrowLine.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x)*(180f/Mathf.PI));
            ImageArrowHead.transform.localPosition = currentPos - diff.normalized *( ImageArrowHead.sprite.textureRect.height/2+2);
            ImageArrowHead.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x) * (180f / Mathf.PI) -90);
        }
    }





}
