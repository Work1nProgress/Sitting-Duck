using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    [SerializeField] private bool _playerEntityBlocksSpawn;
    [SerializeField] private int _maximumEntitiesInRadius;
    [SerializeField] private float _entityCheckRadius;

    public EnemyController TrySpawn(string key)
    {
        bool canSpawn = true;
        int entitiesInRadius = 0;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            _entityCheckRadius);

        foreach(Collider2D collider in colliders)
        {
            EntityStats entity = collider.GetComponent<EntityStats>();
            if(entity != null)
                if (entity.GetEntityType() == EntityType.Player)
                {
                    if (_playerEntityBlocksSpawn)
                        return null;

                    entitiesInRadius++;
                }
                else
                    entitiesInRadius++;
        }
        if (entitiesInRadius > _maximumEntitiesInRadius)
            canSpawn = false;

        if (canSpawn)
            return PoolManager.Spawn<EnemyController>(key, null, transform.position, transform.rotation);

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _entityCheckRadius);
    }
}
