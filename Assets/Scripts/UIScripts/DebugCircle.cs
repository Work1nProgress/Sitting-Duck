using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCircle : MonoBehaviour
{


    Image image;

    [SerializeField]
    bool reverseAngle;



    private void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {



        if (reverseAngle)
        {
            image.enabled = ControllerGame.Instance.AllowReverse;
            image.fillAmount = 1 - (ControllerGame.Instance.ReverseMinAngle / 180f);
            image.transform.localRotation = Quaternion.Euler(0, 0, -ControllerGame.Instance.ReverseMinAngle);
        }
        else
        {
            image.fillAmount = 1 - (ControllerGame.Instance.DeadZone / 180f);
            image.transform.localRotation = Quaternion.Euler(0, 0, -ControllerGame.Instance.DeadZone);

        }
    }
}
