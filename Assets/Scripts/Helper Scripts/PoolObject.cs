using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{

    [HideInInspector]
    public string key;


    

    public virtual void Reuse()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Destroy()
    {
        this.gameObject.SetActive(false);
    }
}
