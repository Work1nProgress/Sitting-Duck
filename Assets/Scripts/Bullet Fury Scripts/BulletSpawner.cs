using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletFury;

public class BulletSpawner : MonoBehaviour
{
    BulletManager _bManager;
    void Start()
    {
        _bManager = GetComponent<BulletManager>();
        
    }

    
    void Update()
    {
        _bManager.Spawn(transform.position, transform.up);
    }
}
