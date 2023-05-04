using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bulletCount, enemyCount, bulletsPershot;

    BulletPool[] bp;
    static BulletManager m_Instance;
    public static BulletManager Instance => m_Instance;



    [SerializeField]
    BulletSettings[] BulletSettings;

    public void Init()
    {
        m_Instance = this;
        bp = new BulletPool[BulletSettings.Length];
        for (int i = 0; i < BulletSettings.Length; i++)
        {
            var pool = new BulletPool();
            pool.Init(BulletSettings[i], transform);
            bp[i] = pool;

        }     
    }

    #region debug

    float timer = 0;
    //[SerializeField]
    //bool fixedUpdate = true;
    //void FixedUpdate()
    //{
    //    if (fixedUpdate)
    //    {
    //        UpdateFrame(Time.fixedDeltaTime);
    //    }

    //}

    private void Update()
    {

            UpdateFrame(Time.deltaTime);
    }

    void UpdateFrame(float deltaTime)
    {
        for (int i = 0; i < bp.Length; i++) {
            bp[i].UpdateLifetime(deltaTime);
        }
        //if (Keyboard.current.upArrowKey.value > 0)
        //{
        //    bullets++;
        //}
        //else if (Keyboard.current.downArrowKey.value > 0)
        //{
        //    bullets--;
        //}
        //bullets = Mathf.Max(1, bullets);

        //if (timer > shootSpeed)
        //{
        //    timer = 0;
        //    if (Mouse.current.leftButton.value > 0)
        //    {
        //        var circle = Mathf.PI * 2;

        //        var delta = circle / bullets;
        //        var angle = Time.timeSinceLevelLoad % Mathf.PI;
        //        for (int i = 0; i < bullets; i++)
        //        {
        //            var pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        //            bp[0].RequestBullet(new Vector3(pos.x, pos.y, 0), new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)), null);
        //            angle += delta;
        //        }
        //    }

        //}
        //timer += deltaTime;
        //bulletCount.text = $"bulletCount {}";
        //bulletsPershot.text = $"bulets per shot: {bullets}";

    }
    #endregion

    public void ReturnBullet(int poolID, Transform t)
    {
        bp[poolID].ReturnBullet(t);
    }

    public void ReturnBullet(int poolID, int ID)
    {
        bp[poolID].ReturnBullet(ID);
    }

    public void RequestBullet(BulletType bulletType, Vector3 position, Vector3 direction)
    {

        bp[(int)bulletType].RequestBullet(position, direction, null);
    }

}

[System.Serializable]
public class BulletSettings
{
    public int ID;
    public Sprite[] Sprites;
    public int InitialPoolSize;
    public Rigidbody2D Prefab;
    public float AnimationSpeed;
    public int MaxBullets;
    public float Speed;


}

public enum BulletType
{
    Player = 0,
    Enemy = 1
}
