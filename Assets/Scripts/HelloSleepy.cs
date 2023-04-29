using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletFury;

public class HelloSleepy : MonoBehaviour
{
    [SerializeField] private BulletManager _bulletManager;
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hello Sleepy!");
    }
}
