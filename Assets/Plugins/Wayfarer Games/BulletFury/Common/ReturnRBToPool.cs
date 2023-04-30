using UnityEngine;
using UnityEngine.Pool;

namespace WayfarerGames.Common
{
    public class ReturnRBToPool : MonoBehaviour, IPooledItem<Rigidbody2D>
    {
        private IObjectPool<Rigidbody2D> _pool;
        
        public void SetPool(IObjectPool<Rigidbody2D> pool)
        {
            _pool = pool;
        }

        private void OnDisable()
        {
            Release();
        }

        public void Release()
        {
            _pool.Release(GetComponent<Rigidbody2D>());
        }
    }
}