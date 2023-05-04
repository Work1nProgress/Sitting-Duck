using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolObject
{
    EntityType _typeToHit;
    int _damage;
    float _speed;

    public void Initialize(int damage, EntityType typeToHit, float speed)
    {
        _damage = damage;
        _typeToHit = typeToHit;
        _speed = speed;
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.position = transform.position + transform.up * _speed;

        
    }

    private void CheckForHit()
    {
        bool hitSuccess = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach(Collider2D collider in colliders)
        {
            EntityStats entity = collider.GetComponent<EntityStats>();
            if(entity != null)
                if(entity.GetEntityType() == _typeToHit)
                {
                    entity.Damage(_damage);
                    hitSuccess = true;
                }
        }

        if (hitSuccess)
            PoolManager.Despawn(this);
    }
}
