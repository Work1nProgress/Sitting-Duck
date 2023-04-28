using System;
using System.Collections.Generic;
using System.Linq;
using BulletFury.Data;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BulletFury
{
    public enum ColliderShape {Sphere, AABB, OBB, Triangle}
    /// <summary>
    /// Tell an object to collide with bullets
    /// </summary>
    public class BulletCollider : MonoBehaviour
    {
        // the set of bullets this collider should collide with
        private List<BulletManager> _hitByBullets = new List<BulletManager>();
        
        [SerializeField] private LayerMask bulletLayersToBeHitBy;

        [SerializeField] private ColliderShape shape;
        
        // the radius of the sphere that describes this collider
        [SerializeField] private float radius = .5f;

        // the bounding box that describes this collider
        [SerializeField] private Vector3 size = Vector3.one;

        [SerializeField] private bool destroyBullet = true;
        // the offset of the collider
        [SerializeField] private Vector3 center;
        
        // the three points that make up the triangle
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB;
        [SerializeField] private Vector3 pointC;
        
        // Unity Event that fires when a bullet collides with this collider, can be set in the inspector like a button 
        // ReSharper disable once InconsistentNaming
        [SerializeField] private BulletCollisionEvent OnCollide;

        [SerializeField, Tooltip("Should the bullets bounce off this collider, rather than being immediately destroyed?")] 
        private bool bounce;
        [SerializeField, Tooltip("Make the bullets die earlier if they bounce - 0-1, as a percentage of total lifetime. If it's set to 0.5 and the lifetime is 10 seconds, the bullets will die 5 seconds earlier.")]
        private float lifetimeLoss;
        [SerializeField, Range(0, 1), Tooltip("How much momentum should the bullets keep? 0-1, as a percentage of current velocity. If it's set to 0.5, bullets will travel at half their remaining speed after bouncing.")]
        private float bounciness;

        public BulletCollisionEvent OnCollideEvent => OnCollide;
        // cached job and job handle
        private BulletDistanceJob _bulletJobCircle;
        private BulletAABBJob _bulletJobAABB;
        private BulletOBBJob _bulletJobOBB;
        private BulletTriangleJob _bulletJobTriangle;
        
        private JobHandle _handle;

        private Vector3 _triangleCentroid, _sideANorm, _sideBNorm, _sideCNorm;

        // an array of bullets
        private BulletContainer[] _bullets;
        private Bounds _bounds;

        private static List<BulletCollider> _colliders;

        public ColliderShape Shape => shape;

        private void Awake()
        {
            if (_colliders == null)
                _colliders = new List<BulletCollider>();
            _colliders.Add(this);


            if (shape == ColliderShape.Triangle)
            {
                var wPosA = LocalPointToWorld(pointA);
                var wPosB = LocalPointToWorld(pointB);
                var wPosC = LocalPointToWorld(pointC);

                _triangleCentroid = (wPosA + wPosB + wPosC) / 3f;
                
                var sideA = wPosB - wPosA;
                var midpointA = (wPosA + wPosB) / 2f;
                _sideANorm = new Vector3(-sideA.y, sideA.x).normalized;
                
                if (Vector3.Dot(_sideANorm, midpointA - _triangleCentroid) < 0)
                    _sideANorm = -_sideANorm;
                
                var sideB = wPosA - wPosC;
                var midpointB = (wPosA + wPosC) / 2f;
                _sideBNorm = new Vector3(-sideB.y, sideB.x).normalized;
                
                if (Vector3.Dot(_sideBNorm, midpointB - _triangleCentroid) < 0)
                    _sideBNorm = -_sideBNorm;

                var sideC = wPosC - wPosB;
                var midpointC = (wPosC + wPosB) / 2f;
                _sideCNorm = new Vector3(-sideC.y, sideC.x).normalized;
                
                if (Vector3.Dot(_sideCNorm, midpointC - _triangleCentroid) < 0)
                    _sideCNorm = -_sideCNorm;
            } else if (shape == ColliderShape.AABB || shape == ColliderShape.OBB)
            {
                _bounds = new Bounds(transform.position + center, Vector3.Scale(transform.localScale, size) /2f);
            }
        }

        private void OnEnable()
        {
            var managers = BulletManager.GetAllManagers();
            if (managers == null)
                return;
            for (int i = managers.Count - 1; i >= 0; i--)
            {
                if (managers[i] == null) continue;
                
                if (bulletLayersToBeHitBy == (bulletLayersToBeHitBy | (1 << managers[i].gameObject.layer)) && !_hitByBullets.Contains(managers[i]))
                    AddManagerToBullets(managers[i]);
            }
        }

        private void OnDisable()
        {
            var managers = BulletManager.GetAllManagers();
            if (managers == null)
                return;
            for (int i = managers.Count - 1; i >= 0; i--)
            {
                if (managers[i] == null)
                    continue;
                
                if (bulletLayersToBeHitBy == (bulletLayersToBeHitBy | (1 << managers[i].gameObject.layer)) && _hitByBullets.Contains(managers[i]))
                    RemoveManagerFromBullets(managers[i]);
            }
        }

        /// <summary>
        /// Unity function, called every frame
        /// Run the job, tell the bullet manager that the bullet has been collided with
        /// </summary>
        private void FixedUpdate()
        {
            if (!gameObject.activeInHierarchy) return;
            
            for (var index = _hitByBullets.Count - 1; index >= 0; index--)
            {
                if (index >= _hitByBullets.Count)
                    return;
                var manager = _hitByBullets[index];
                if (manager == null || !manager.enabled || !manager.gameObject.activeSelf ||
                    manager.GetBullets() == null)
                    continue;
                // grab the bullets in the bullet manager
                _bullets = manager.GetBullets();

                if (shape == ColliderShape.Sphere)
                {
                    // create the job
                    _bulletJobCircle = new BulletDistanceJob
                    {
                        In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Distance = radius * transform.localScale.x,
                        Position = transform.position + center
                    };

                    // run the job
                    _handle = _bulletJobCircle.Schedule(_bullets.Length, 256);
                    // make sure the job finished this frame
                    _handle.Complete();
                    // grab the results
                    _bulletJobCircle.Out.CopyTo(_bullets);
                    // dispose the native arrays
                    _bulletJobCircle.In.Dispose();
                    _bulletJobCircle.Out.Dispose();
                }
                else if (shape == ColliderShape.AABB)
                {
                    var bounds = new Bounds(transform.position + center, Vector3.Scale(transform.localScale, size));
                    // create the job
                    _bulletJobAABB = new BulletAABBJob
                    {
                        In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        BoxMin = bounds.min,
                        BoxMax = bounds.max
                    };

                    // run the job
                    _handle = _bulletJobAABB.Schedule(_bullets.Length, 256);
                    // make sure the job finished this frame
                    _handle.Complete();
                    
                    // grab the results
                    _bulletJobAABB.Out.CopyTo(_bullets);
                    // dispose the native arrays
                    _bulletJobAABB.In.Dispose();
                    _bulletJobAABB.Out.Dispose();
                } else if (shape == ColliderShape.OBB)
                {
                    var scaledSize = Vector3.Scale(transform.localScale, size);
                    
                    _bulletJobOBB = new BulletOBBJob
                    {
                        In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        BoxHalfExtents = new NativeArray<float>(new[] {scaledSize.x, scaledSize.y, scaledSize.z},
                            Allocator.TempJob),
                        BoxAxes = new NativeArray<float3>(
                            new float3[] {transform.right, transform.up, transform.forward}, Allocator.TempJob),
                        BoxCentre = transform.position + center
                    };
                    
                    // run the job
                    _handle = _bulletJobOBB.Schedule(_bullets.Length, 256);
                    // make sure the job finished this frame
                    _handle.Complete();
                    
                    // grab the results
                    _bulletJobOBB.Out.CopyTo(_bullets);
                    // dispose the native arrays
                    _bulletJobOBB.In.Dispose();
                    _bulletJobOBB.Out.Dispose();
                    _bulletJobOBB.BoxHalfExtents.Dispose();
                    _bulletJobOBB.BoxAxes.Dispose();
                }else if (shape == ColliderShape.Triangle)
                {
                    _bulletJobTriangle = new BulletTriangleJob
                    {
                        In = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        Out = new NativeArray<BulletContainer>(_bullets, Allocator.TempJob),
                        a = LocalPointToWorld(pointA),
                        b = LocalPointToWorld(pointB),
                        c = LocalPointToWorld(pointC)
                    };
                    
                    // run the job
                    _handle = _bulletJobTriangle.Schedule(_bullets.Length, 256);
                    // make sure the job finished this frame
                    _handle.Complete();
                    
                    // grab the results
                    _bulletJobTriangle.Out.CopyTo(_bullets);
                    // dispose the native arrays
                    _bulletJobTriangle.In.Dispose();
                    _bulletJobTriangle.Out.Dispose();
                }

                // loop through the bullets, if there was a collision this frame - tell the bullet manager and anything else that needs to know
                for (int i = 0; i < _bullets.Length; i++)
                {
                    if (_bullets[i].Dead == 0 && _bullets[i].Collided == 1 && _bullets[i].BouncedThisFrame == 0 && _bullets[i].CurrentSize > 0)
                    {
                        //if (Shape == ColliderShape.OBB)
                          //  Debug.DrawLine(_bullets[i].Position, _bullets[i].Position + _bullets[i].Up, Color.green, 1);

                        if (destroyBullet && !bounce)
                        {
                            manager.HitBullet(i);
                        }
                        if (bounce)
                        {
                            var normal = Vector3.zero;
                            switch (shape)
                            {
                                case ColliderShape.Sphere:
                                    normal = ComputeSphereNormal(_bullets[i].Position);
                                    break;
                                case ColliderShape.AABB:
                                    normal = ComputeAABBNormal(_bullets[i].Position);
                                    break;
                                case ColliderShape.OBB:
                                    normal = ComputeOBBNormal(_bullets[i].Position);
                                    break;
                                case ColliderShape.Triangle:
                                    normal = ComputeTriangleNormal(_bullets[i].Position);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            manager.BounceBullet(i, normal, bounciness, lifetimeLoss);
                        }

                        OnCollide?.Invoke(_bullets[i], this);
                    }
                }
            }
        }

        public Vector3 ComputeTriangleNormal(Vector3 point)
        {
            var sqDistA = Vector3.SqrMagnitude(point - pointA);
            var sqDistB = Vector3.SqrMagnitude(point - pointB);
            var sqDistC = Vector3.SqrMagnitude(point - pointC);


            if (sqDistA <= sqDistB && sqDistA <= sqDistC)
                return _sideCNorm;
            
            if (sqDistB <= sqDistA && sqDistB <= sqDistC)
                return _sideBNorm;
            
            return _sideANorm;
        }

        public Vector3 ComputeAABBNormal(Vector3 point)
        {
            var scale = Vector3.Scale(transform.localScale, size);
            
            var dir = point - (transform.position + center);

            dir.x = (Mathf.InverseLerp(-scale.x / 2f, scale.x / 2f, dir.x) - 0.5f) * 2;
            dir.y = (Mathf.InverseLerp(-scale.y / 2f, scale.y / 2f, dir.y) - 0.5f) * 2;
            dir.z = (Mathf.InverseLerp(-scale.z / 2f, scale.z / 2f, dir.z) - 0.5f) * 2;
            
            return new []
            {
                 Vector3.right,
                -Vector3.right,
                 Vector3.up,
                 -Vector3.up,
                 Vector3.forward,
                 -Vector3.forward
            }.OrderByDescending(v => Vector3.Dot(dir, v)).First();
        }

        public Vector3 ComputeOBBNormal(Vector3 point)
        {
            var scale = Vector3.Scale(transform.localScale, size);
            
            var dir = (point - (transform.position + center));

            dir = Quaternion.Inverse(transform.rotation) * dir;

            dir.x = (Mathf.InverseLerp(-scale.x / 2f, scale.x / 2f, dir.x) - 0.5f) * 2;
            dir.y = (Mathf.InverseLerp(-scale.y / 2f, scale.y / 2f, dir.y) - 0.5f) * 2;
            dir.z = (Mathf.InverseLerp(-scale.z / 2f, scale.z / 2f, dir.z) - 0.5f) * 2;
            
            dir = transform.rotation * dir;
            

            return new []
            {
                transform.rotation * Vector3.right,
                transform.rotation * -Vector3.right,
                transform.rotation * Vector3.up,
                transform.rotation * -Vector3.up,
                transform.rotation * Vector3.forward,
                transform.rotation * -Vector3.forward
            }.OrderByDescending(v => Vector3.Dot(dir, v)).First();
        }

        public Vector3 ComputeSphereNormal(Vector3 point)
        {
            var dx = point.x - transform.position.x;
            var dy = point.y - transform.position.y;

            return new Vector3(dx, dy);
        }

        public void AddManagerToBullets(BulletManager manager)
        {
            _hitByBullets.Add(manager);
        }

        public void RemoveManagerFromBullets(BulletManager manager)
        {
            _hitByBullets.Remove(manager);
        }

        public static void AddManagerToColliders(BulletManager manager)
        {
            if (_colliders == null)
                return;
            foreach (var collider in _colliders)
            {
                if (collider.bulletLayersToBeHitBy == (collider.bulletLayersToBeHitBy | (1 << manager.gameObject.layer)) && !collider._hitByBullets.Contains(manager))
                    collider.AddManagerToBullets(manager);
            }
        }

        public static void RemoveManagerFromColliders(BulletManager manager)
        {
            if (_colliders == null)
                return;
            foreach (var collider in _colliders)
            {
                if (collider.bulletLayersToBeHitBy == (collider.bulletLayersToBeHitBy | (1 << manager.gameObject.layer)) && collider._hitByBullets.Contains(manager))
                    collider.RemoveManagerFromBullets(manager);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (shape == ColliderShape.Sphere)
                Gizmos.DrawWireSphere(transform.position + center, radius * transform.localScale.x);
            else if (shape == ColliderShape.AABB)
            {
                Gizmos.DrawWireCube(transform.position + center, Vector3.Scale(transform.localScale, size));
                
                
            }
            else if (shape == ColliderShape.OBB)
            {
                var matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.matrix = matrix;
                Gizmos.DrawWireCube(center, Vector3.Scale(transform.localScale, size));
            } else if (shape == ColliderShape.Triangle)
            {
                var wPosA = LocalPointToWorld(pointA);
                var wPosB = LocalPointToWorld(pointB);
                var wPosC = LocalPointToWorld(pointC);
                
                Gizmos.DrawLine(wPosA, wPosB);
                Gizmos.DrawLine(wPosB, wPosC);
                Gizmos.DrawLine(wPosC, wPosA);
                _triangleCentroid = (wPosA + wPosB + wPosC) / 3f;

                var sideA = wPosB - wPosA;
                var midpointA = (wPosA + wPosB) / 2f;
                _sideANorm = new Vector3(-sideA.y, sideA.x).normalized;
                var isPointingIn = Vector3.Dot(_sideANorm, midpointA - _triangleCentroid);
                
                if (isPointingIn < 0)
                    _sideANorm = -_sideANorm;

                Gizmos.DrawLine(midpointA, midpointA + _sideANorm);
                
                var sideB = wPosA - wPosC;
                var midpointB = (wPosA + wPosC) / 2f;
                _sideBNorm = new Vector3(-sideB.y, sideB.x).normalized;
                isPointingIn = Vector3.Dot(_sideBNorm, midpointB - _triangleCentroid);
                
                if (isPointingIn < 0)
                    _sideBNorm = -_sideBNorm;
                
                Gizmos.DrawLine(midpointB, midpointB + _sideBNorm);
                
                var sideC = wPosC - wPosB;
                var midpointC = (wPosC + wPosB) / 2f;
                _sideCNorm = new Vector3(-sideC.y, sideC.x).normalized;
                isPointingIn = Vector3.Dot(_sideCNorm, midpointC - _triangleCentroid);
                
                if (isPointingIn < 0)
                    _sideCNorm = -_sideCNorm;
                
                
                Gizmos.DrawLine(midpointC, midpointC + _sideCNorm);
            }
        }

        private Vector3 LocalPointToWorld(Vector3 point)
        {
            return transform.position + transform.rotation * Vector3.Scale(point, transform.localScale);
        }
    }
}