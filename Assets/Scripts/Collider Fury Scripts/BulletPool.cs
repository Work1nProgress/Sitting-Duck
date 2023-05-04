using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletPool {

    Rigidbody2D BulletPrefab;

    Transform m_DeadBulletContainer;


    #region rendering
    private Camera renderCamera;
    private List<Matrix4x4> _matrices;
    private MaterialPropertyBlock _materialPropertyBlock;
    bool _hasBullets = false;
    BulletContainer _currentBullet;
    private Vector4[] _colors;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    #endregion


    int m_MaxBullets;
    int PoolID;
    BulletContainer[] m_Bullets;


    public void Init(int maxBullets, int poolID, Transform deadBulletContainer, Rigidbody2D bulletPrefab)
    {
        m_Bullets = new BulletContainer[maxBullets];
        _colors = new Vector4[maxBullets];
        for (int i = 0; i < maxBullets; i++)
        {
            _colors[i] = Vector4.one;
        }
        m_DeadBulletContainer = deadBulletContainer;
        m_MaxBullets = maxBullets;
        PoolID = poolID;
        BulletPrefab = bulletPrefab;
        for (int i = 0; i < m_MaxBullets/2; i++)
        {
            var bullet = m_Bullets[i];
            if (!bullet.IsInitialized)
            {
                bullet.Bullet = Object.Instantiate(BulletPrefab);
                bullet.Bullet.gameObject.transform.name = $"{i}";
                bullet.IsInitialized = true;
            }
            m_Bullets[i] = bullet;
            continue;


            
        }
    }

    public void RequestBullet(Vector3 position, Vector3 direction, float speed, Transform parent)
    {
        for (int i = 0; i < m_MaxBullets; i++)
        {
            var bullet = m_Bullets[i];
            if (!bullet.IsAlive)
            {
                if (!bullet.IsInitialized)
                {
                    bullet.Bullet = Object.Instantiate(BulletPrefab);
                    bullet.Bullet.gameObject.transform.name = $"{i}";
                    bullet.IsInitialized = true;
                }
                bullet.PoolID = PoolID;
                bullet.ID = i;
                bullet.Bullet.transform.SetParent(parent);
                bullet.Bullet.transform.position = position;
                bullet.IsAlive = true;
                bullet.Bullet.gameObject.SetActive(true);
                bullet.Bullet.velocity = direction * speed;
                bullet.lifetime = 2f;
                m_Bullets[i] = bullet;
                return;
               
            }
        }
    }

    public int UpdateLifetime(float deltaTime)
    {
        int bullets = 0;
        for (int i = 0; i < m_MaxBullets; i++)
        {
            if (!m_Bullets[i].IsAlive) continue;
            bullets++;
            var bullet = m_Bullets[i];

            bullet.timer += deltaTime;
            if (bullet.timer > bullet.lifetime)
            {
                ReturnBullet(bullet.ID);
            }
            else
            {

                m_Bullets[i] = bullet;
            }
        }
        return bullets;

    }

    public void ReturnBullet(Transform rb)
    {
        ReturnBullet(System.Array.FindIndex(m_Bullets, x => x.Bullet.transform == rb));
    }

    public void ReturnBullet(int ID)
    {
        m_Bullets[ID].timer = 0;
        m_Bullets[ID].IsAlive = false;
        m_Bullets[ID].Bullet.gameObject.SetActive(false);
        m_Bullets[ID].Bullet.transform.SetParent(m_DeadBulletContainer);
    }

//    private void Render(ScriptableRenderContext context, Camera cam)
//    {
//        if (renderCamera == null)
//            renderCamera = Camera.main;


//        // create a new buffer - this will contain the render data
//        var buffer = new CommandBuffer();
//#if UNITY_EDITOR
//        buffer.name = "Rampslayer";
//#endif

//        // create a new material property block - this contains the different colours for every instance
//        _materialPropertyBlock = new MaterialPropertyBlock();

//        _hasBullets = false;
//        _matrices.Clear();

//        // loop through and render the bullets
//        for (int i = m_Bullets.Length - 1; i >= 0; i--)
//        {
//            _currentBullet = m_Bullets[i];
//            // if the bullet is alive
//            if (_currentBullet.IsAlive)
//            {
//                // we've got at least one active bullet, so we should render something
//                _hasBullets = true;

//                // set the colour for the bullet
//             //   _colors[i] = _currentBullet.Color;

//                // if the "w" part of the rotation is 0, the Quaternion is invalid. Set it to the a rotation of 0,0,0
//                if (_currentBullet.Rotation.w == 0)
//                    _currentBullet.Rotation = Quaternion.identity;

//                if (bulletSettings.Plane == BulletPlane.XZ)
//                    _currentBullet.Rotation *= Quaternion.AngleAxis(90, Vector3.right);
//                if (IsNaN(_currentBullet.Rotation))
//                    _matrices.Add(Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero));
//                else
//                    // set the matrix for the current bullet - translation, rotation, scale, in that order.
//                    _matrices.Add(Matrix4x4.TRS(_currentBullet.Position,
//                        _currentBullet.Rotation,
//                        Vector3.one * _currentBullet.CurrentSize));
//            }
//        }

//        // if we don't have any bullets, don't bother rendering anything
//        if (!_hasBullets)
//            return;

//        // set the colours of the material property block
//        _materialPropertyBlock.SetVectorArray(ColorProperty,_colors);

//        // draw all the meshes
//        // n.b. this is why we can only have 1023 bullets per spawner
//        Graphics.DrawMeshInstanced(bulletSettings.Mesh, 0, bulletSettings.Material, _matrices, _materialPropertyBlock, ShadowCastingMode.Off, false, gameObject.layer, cam);

//        maxActiveBullets = Mathf.Max(maxActiveBullets, currentActiveBullets);
//    }


}

public struct BulletContainer
{
    public bool IsInitialized;
    public int ID;
    public int PoolID;
    public Rigidbody2D Bullet;
    public bool IsAlive;
    public float lifetime;
    public float timer;
}
