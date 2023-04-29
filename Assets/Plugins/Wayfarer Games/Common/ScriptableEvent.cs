using System;
using UnityEngine;

namespace WayfarerGames.Common
{
    [CreateAssetMenu(fileName = "Scriptable Event", menuName = "Wayfarer Games/Scriptable Event")]
    public class ScriptableEvent : ScriptableObject
    {
        public event Action<GameObject> OnEvent;

        public void Invoke(GameObject sender)
        {
            OnEvent?.Invoke(sender);
        }
    }
    
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        public event Action<GameObject, T> OnEvent;

        public void Invoke(GameObject sender, T val)
        {
            OnEvent?.Invoke(sender, val);
        }
    }
}