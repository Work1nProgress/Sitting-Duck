using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstance
{
    GameObject gameObject;
    Transform transform;

    bool hasPoolObjectComponent;
    PoolObject poolObject;

    public ObjectInstance(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        this.gameObject.SetActive(false);

        PoolObject poolObjectReference = gameObject.GetComponent<PoolObject>();
        if(poolObjectReference != null)
        {
            this.hasPoolObjectComponent = true;
            this.poolObject = poolObjectReference;
        }
    }

    public void Reuse(Vector3 position,Quaternion rotation)
    {
        if(this.hasPoolObjectComponent)
        {
            this.poolObject.Reuse();
        }
        this.gameObject.SetActive(true);
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        
        this.gameObject.transform.position = position;
        this.gameObject.transform.rotation = rotation;
    }
    

    public void SetParent(Transform parent)
    {
        this.gameObject.transform.parent = parent;
    }
}
