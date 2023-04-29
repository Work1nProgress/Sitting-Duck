using UnityEngine;
using UnityEngine.Pool;

namespace WayfarerGames.Common
{
    public class ReturnToPool : MonoBehaviour, IPooledItem<GameObject>
    {
        private IObjectPool<GameObject> _pool;
        
        public void SetPool(IObjectPool<GameObject> pool)
        {
            _pool = pool;
        }

        private void OnDisable()
        {
            Release();
        }

        public void Release()
        {
            _pool.Release(gameObject);
        }
    }
}