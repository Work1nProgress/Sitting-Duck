using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolObject
{
    EntityType _typeToHit;
    int _damage;
    float _speed;

    CountdownTimer _projectileLifeTimer;

    public virtual void Initialize(int damage, EntityType typeToHit, float speed)
    {
        _damage = damage;
        _typeToHit = typeToHit;
        _speed = speed;

        _projectileLifeTimer = new CountdownTimer(5f, false, false);
        _projectileLifeTimer.OnTimerExpired += Despawn;
    }

    void Start()
    {
        
    }

    protected virtual void Update()
    {
        if(_projectileLifeTimer != null)
        _projectileLifeTimer.Update(Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        transform.position = transform.position + transform.up * _speed;

        CheckForHit();
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
            Despawn();
    }

    protected void Despawn()
    {
        PoolManager.Despawn(this);
    }
}
