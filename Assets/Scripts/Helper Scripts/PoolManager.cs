using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : GenericSingleton<PoolManager>
{
    private Dictionary<int,Queue<ObjectInstance>> _poolDictionary;

    protected override void Awake() {
        base.Awake();
        _poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
    }

    public void CreatePool(GameObject prefab,int poolSize)
    {
        int id = prefab.GetInstanceID();

        if(_poolDictionary.ContainsKey(id) == false)
        {
            _poolDictionary.Add(id,new Queue<ObjectInstance>());

            GameObject poolObjectsContainer = new GameObject(prefab.name + " pool");
            poolObjectsContainer.transform.parent = transform;
        
            for(int i = 0; i < poolSize; i++)
            {
                ObjectInstance objectInPool = new ObjectInstance(Instantiate(prefab));
                objectInPool.SetParent(poolObjectsContainer.transform);
                _poolDictionary[id].Enqueue(objectInPool);
            }
        }

        
    }

    public void ReuseObject(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        int id = prefab.GetInstanceID();

        if(!_poolDictionary.ContainsKey(id))
        {
            Debug.LogError(prefab.name + " Pool not found!!");
            return;
        }


        ObjectInstance objectInPool = _poolDictionary[id].Dequeue();
        objectInPool.Reuse(position,rotation);
        _poolDictionary[id].Enqueue(objectInPool);
    }
}
