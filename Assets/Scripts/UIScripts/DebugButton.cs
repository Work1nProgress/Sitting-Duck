using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    TextMeshProUGUI label;

    [SerializeField]

    TextMeshProUGUI sliderTitle;



    private void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void OnToggleLookAtArrow()
    {
        ControllerGame.Instance.ToggleRotateTowardsArrow();
        if (label)
        {
            label.text = ControllerGame.Instance.RotateTowardsArrow ? "look at arrow" : "no look";
        }

    }

    public void OnToggleReverse()
    {
        ControllerGame.Instance.ToggleReverse();
        if (label)
        {
            label.text = ControllerGame.Instance.AllowReverse ? "reverse" : "forward";
        }

    }

    public void OnValueChangedLookSpeed(float value)
    {
        ControllerGame.Instance.LookSpeed = value;
        sliderTitle.text = $"look speed: { (int)ControllerGame.Instance.LookSpeed}";
    }

    public void OnValueChangedDeadZone(float value)
    {
        ControllerGame.Instance.DeadZone = value;
        sliderTitle.text = $"dead zone: { (int)ControllerGame.Instance.DeadZone}°";
    }

    public void OnValueChangedReverseMinAngle(float value)
    {
        ControllerGame.Instance.ReverseMinAngle = value;
        sliderTitle.text = $"reverse min angle: { (int)ControllerGame.Instance.ReverseMinAngle}";
    }
}
