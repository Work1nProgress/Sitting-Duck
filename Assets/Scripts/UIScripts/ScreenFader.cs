using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fade;
    private bool fadingToBlack = false;

    public void StartFade()
    {
        fadingToBlack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingToBlack)
        {
            var oldAlpha = fade.color.a;
            fade.color = new Color(fade.color.r, fade.color.b, fade.color.g, oldAlpha += Time.deltaTime / 2);
        }
    }
}
