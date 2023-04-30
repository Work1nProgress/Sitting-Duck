using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T :  GenericSingleton<T>
{
    private static T _instance;

    public static T Instance { get => _instance; }

    protected virtual void Awake() {
        if(_instance == null)
        {
            _instance = this as T;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(this);
            }
            return;
        }
        Destroy(gameObject);
    }
}
