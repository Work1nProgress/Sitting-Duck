using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletFury;

public class BulletSpawner : MonoBehaviour
{
    private BulletManager _bManager;
    [SerializeField] private bool _firing = true;


    

    void Awake()
    {
        _bManager = GetComponent<BulletManager>();

        
    }

    
    void Update()
    {
        if(_firing)
        _bManager.Spawn(transform);
    }

    public void BeginFiring()
    {
        _firing = true;
    }

    public void StopFiring()
    {
        _firing = false;
    }
}
