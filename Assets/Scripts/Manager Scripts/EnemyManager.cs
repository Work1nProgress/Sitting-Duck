using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private PoolManager _poolManager;
    void Awake()
    {
        GameObject pm = GameObject.Find("Pooler");
        if (pm != null)
            _poolManager = pm.GetComponent<PoolManager>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
