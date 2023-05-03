using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSingleton<T> : MonoBehaviour where T :  LocalSingleton<T>
{
    private static T _instance;

    public static T Instance { get => _instance; }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            return;
        }
    }
}
