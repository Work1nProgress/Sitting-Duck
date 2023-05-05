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

        if (OnBulletHitEvent == null)
        {
            return;
        }

        //this handles chainsaw drones
        if (collision.attachedRigidbody.gameObject.layer == 7 && gameObject.layer == 10)
        {
            Debug.Log($"on trigger enter chainsaw", this);
          
            OnBulletHitEvent.Invoke(BulletType.Player);
            return;
        }

        split = collision.transform.name.Split('_');
        (int pool, int id) = (int.Parse(split[0]), int.Parse(split[1]));
        OnBulletHitEvent.Invoke((BulletType)pool);
        BulletManager.Instance.ReturnBullet(pool, id);
    }

  

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (OnBulletHitEvent == null)
        {
            return;
        }

        //this handles chainsaw drones
        if (collision.attachedRigidbody.gameObject.layer == 7 && gameObject.layer == 10)
        {
            Debug.Log($"on trigger stay chainsaw", this);
            OnBulletHitEvent.Invoke(BulletType.Player);
            return;
        }

        split = collision.transform.name.Split('_');
        (int pool, int id) = (int.Parse(split[0]), int.Parse(split[1]));
        OnBulletHitEvent.Invoke((BulletType)pool);
        BulletManager.Instance.ReturnBullet(pool, id);
    }
}
