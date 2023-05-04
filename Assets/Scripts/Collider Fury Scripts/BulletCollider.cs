using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {


     //   Debug.Log("hit trigger");

        BulletManager.Instance.ReturnBullet(int.Parse(collision.transform.name));

    }


    Vector3 Direction;
    private void FixedUpdate()
    {
        Direction +=  new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0) * Time.deltaTime;
        transform.position += Direction;
    }

}
