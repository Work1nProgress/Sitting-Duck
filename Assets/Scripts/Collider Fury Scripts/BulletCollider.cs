using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{

    string[] split;

    public delegate void OnBulletHit(BulletType bulletType, bool chainsaw);
    public event OnBulletHit OnBulletHitEvent;

    public delegate void OnTriggerExit();
    public event OnTriggerExit OnTriggerExitEvent;



    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (OnBulletHitEvent == null)
        {
            return;
        }

        //this handles chainsaw drones
        if (!collision.attachedRigidbody.gameObject.name.Contains('_')  && gameObject.layer == 10)
        {
          
            OnBulletHitEvent.Invoke(BulletType.Player, true);
            return;
        }

        split = collision.transform.name.Split('_');
        (int pool, int id) = (int.Parse(split[0]), int.Parse(split[1]));
        OnBulletHitEvent.Invoke((BulletType)pool, false);
        BulletManager.Instance.ReturnBullet(pool, id);
    }

  

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (OnBulletHitEvent == null)
        {
            return;
        }

        //this handles chainsaw drones
        if (!collision.attachedRigidbody.gameObject.name.Contains('_') && gameObject.layer == 10)
        {
            OnBulletHitEvent.Invoke(BulletType.Player, true);
            return;
        }

        split = collision.transform.name.Split('_');
        (int pool, int id) = (int.Parse(split[0]), int.Parse(split[1]));
        OnBulletHitEvent.Invoke((BulletType)pool, false);
        BulletManager.Instance.ReturnBullet(pool, id);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnBulletHitEvent == null)
        {
            return;
        }

        //this handles chainsaw drones
        if (!collision.attachedRigidbody.gameObject.name.Contains('_') && gameObject.layer == 10)
        {
            OnTriggerExitEvent.Invoke();
            return;
        }

      
    }
}
