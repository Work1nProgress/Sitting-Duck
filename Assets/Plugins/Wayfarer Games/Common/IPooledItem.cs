using System;
using UnityEngine.Pool;

namespace WayfarerGames.Common
{
    public interface IPooledItem<T> where T : UnityEngine.Object
    {
        public void SetPool (IObjectPool<T> pool);
        public void Release();
    }
}