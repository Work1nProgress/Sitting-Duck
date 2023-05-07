using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chainsawsoundplayer : MonoBehaviour
{
    [SerializeField] private float rrrrrrCooldown = 1.5f;
    private float rrrrrrTimerrr = 0;
    public void Rprpbrprpbp()
    {
        if (rrrrrrTimerrr < 0)
        {
            SoundManager.Instance.Play("chainsaw");
            rrrrrrTimerrr = rrrrrrCooldown;
        }
    }

    private void Update()
    {
        if (rrrrrrTimerrr >= 0)
        {
            rrrrrrTimerrr -= Time.deltaTime;
        }
    }
}

