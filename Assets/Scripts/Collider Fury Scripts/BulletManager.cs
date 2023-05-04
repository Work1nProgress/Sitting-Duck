using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletManager : MonoBehaviour
{

    [SerializeField]
    Rigidbody2D prefab;

    [SerializeField]
    Collider2D c;

    [SerializeField]
    TextMeshProUGUI bulletCount, enemyCount, bulletsPershot;

    BulletPool bp;
    static BulletManager m_Instance;
    public static BulletManager Instance => m_Instance;
    float timer = 0;
    public void Start()
    {
        m_Instance = this;
        bp = new BulletPool();
        bp.Init(1000, 0, transform, prefab);
        //for (int i = 0; i < 1000; i++)
        //{
        //    bp.RequestBullet(new Vector3(), new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), 0), 1f, null);
        //}
        var enemies = 500;
        for (int i = 0; i < enemies; i++)
        {
            Instantiate(c, new Vector3(Random.Range(-5, 5f), Random.Range(-5f, 5f), 0), Quaternion.identity, null);

        }
        enemyCount.text = $"EnemyCount: {enemies}";
        c.gameObject.SetActive(false);
        prefab.gameObject.SetActive(false);
    }
    float shootSpeed = 0.1f;
    int bullets = 1;
    void FixedUpdate()
    {
        if (Keyboard.current.upArrowKey.value > 0)
        {
            bullets++;
        }
        else if(Keyboard.current.downArrowKey.value > 0)
        {
            bullets--;
        }
        bullets = Mathf.Max(1, bullets);

        if (timer > shootSpeed)
        {
            timer = 0;
            if (Mouse.current.leftButton.value > 0)
            {
                var circle = Mathf.PI * 2;

                var delta = circle / bullets;
                var angle = Time.timeSinceLevelLoad % Mathf.PI;
                for (int i = 0; i < bullets; i++)
                {
                    var pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
                    bp.RequestBullet(new Vector3(pos.x, pos.y, 0), new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)), 10f, null);
                    angle += delta;
                }
            }

        }
        timer += Time.fixedDeltaTime;
        bulletCount.text = $"bulletCount {bp.UpdateLifetime(Time.fixedDeltaTime)}";
        bulletsPershot.text = $"bulets per shot: {bullets}";


    }

    public void ReturnBullet(int poolID, Transform t)
    {
        bp.ReturnBullet(t);
    }

    public void ReturnBullet(int ID)
    {
        bp.ReturnBullet(ID);
    }

}
