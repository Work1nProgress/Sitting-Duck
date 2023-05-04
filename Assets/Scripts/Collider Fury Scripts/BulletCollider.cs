using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{

    string[] split;

    public delegate void OnBulletHit(BulletType bulletType);
    public event OnBulletHit OnBulletHitEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        split = collision.transform.name.Split('_');
        (int pool, int id) = (int.Parse(split[0]), int.Parse(split[1]));
        OnBulletHitEvent.Invoke((BulletType)pool);
        BulletManager.Instance.ReturnBullet(pool, id);
    }


    Vector3 Direction;
    private void FixedUpdate()
    {
        Direction +=  new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0) * Time.deltaTime;
        transform.position += Direction;
    }

}
