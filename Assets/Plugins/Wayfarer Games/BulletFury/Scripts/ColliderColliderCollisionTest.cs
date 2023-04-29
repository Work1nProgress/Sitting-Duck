using System;
using System.Collections.Generic;
using BulletFury.Data;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace BulletFury
{
    [RequireComponent(typeof(BulletCollider))]
    public class ColliderColliderCollisionTest : MonoBehaviour
    {
        [SerializeField] private List<BulletCollider> collidersToHit;
        [SerializeField] private float damageToTake = 1f;
        [SerializeField] private BulletCollisionEvent onCollision;
        public event Action<Vector3, BulletCollider> OnCollision;

        private BulletCollider _thisCollider;

        // cached job and job handle
        private DistanceJob _bulletJobCircle;
        private AABBJob _bulletJobAABB;
        private OBBJob _bulletJobOBB;
        private TriangleJob _bulletJobTriangle;
        
        private JobHandle _handle;
        
        private Dictionary<BulletCollider, float> _colliderDamage = new Dictionary<BulletCollider, float>();

        public void AddCollider(BulletCollider collider, float damage)
        {
            collidersToHit.Add(collider);
            if (_colliderDamage.ContainsKey(collider))
                _colliderDamage[collider] = damage;
            else
                _colliderDamage.Add(collider, damage);
        }

        public void RemoveCollider(BulletCollider collider)
        {
            collidersToHit.Remove(collider);
            if (_colliderDamage.ContainsKey(collider))
                _colliderDamage.Remove(collider);
        }
        
        private void Awake()
        {
            _thisCollider = GetComponent<BulletCollider>();
            if (_thisCollider.Shape != ColliderShape.Sphere)
                Debug.LogWarning("Cannot check non-sphere colliders");
        }

        private void FixedUpdate()
        {
            if (!gameObject.activeInHierarchy || _thisCollider.Shape != ColliderShape.Sphere) return;

            BulletCollider hit = null;

            for (var index = collidersToHit.Count - 1; index >= 0; index--)
            {
                if (hit != null)
                    continue;

                if (index >= collidersToHit.Count)
                    return;

                var checkCollider = collidersToHit[index];
                var circleCollider = _thisCollider;
                if (checkCollider == null || !checkCollider.enabled || !checkCollider.gameObject.activeSelf)
                    continue;


                var shape = checkCollider.Shape;

                if (shape == ColliderShape.Sphere)
                {
                    // create the job
                    _bulletJobCircle = new DistanceJob
                    {
                        CheckPosition = circleCollider.transform.position + circleCollider.Center,
                        CheckSize = circleCollider.Radius * circleCollider.transform.localScale.x,
                        Distance = checkCollider.Radius * checkCollider.transform.localScale.x,
                        Position = checkCollider.transform.position + checkCollider.Center,
                        Out = new NativeArray<byte>(1, Allocator.TempJob),
                    };

                    // run the job
                    _handle = _bulletJobCircle.Schedule();
                    // make sure the job finished this frame
                    _handle.Complete();
                    // grab the results
                    if (_bulletJobCircle.Out[0] == 1)
                        hit = checkCollider;
                }
                else if (shape == ColliderShape.AABB)
                {
                    var bounds = new Bounds(checkCollider.transform.position + checkCollider.Center,
                        Vector3.Scale(checkCollider.transform.localScale, checkCollider.Size));
                    // create the job
                    _bulletJobAABB = new AABBJob
                    {
                        CheckPosition = circleCollider.transform.position + circleCollider.Center,
                        CheckSize = circleCollider.Radius * circleCollider.transform.localScale.x,
                        BoxMin = bounds.min,
                        BoxMax = bounds.max,
                        Out = new NativeArray<byte>(1, Allocator.TempJob),
                    };

                    // run the job
                    _handle = _bulletJobAABB.Schedule();
                    // make sure the job finished this frame
                    _handle.Complete();

                    // grab the results
                    if (_bulletJobAABB.Out[0] == 1)
                        hit = checkCollider;
                }
                else if (shape == ColliderShape.OBB)
                {
                    var scaledSize = Vector3.Scale(checkCollider.transform.localScale, checkCollider.Size);

                    _bulletJobOBB = new OBBJob
                    {
                        CheckPosition = circleCollider.transform.position + circleCollider.Center,
                        CheckSize = circleCollider.Radius * circleCollider.transform.localScale.x,
                        BoxHalfExtents = new NativeArray<float>(new[] { scaledSize.x, scaledSize.y, scaledSize.z },
                            Allocator.TempJob),
                        BoxAxes = new NativeArray<float3>(
                            new float3[]
                            {
                                checkCollider.transform.right, checkCollider.transform.up,
                                checkCollider.transform.forward
                            }, Allocator.TempJob),
                        BoxCentre = checkCollider.transform.position + checkCollider.Center,
                        Out = new NativeArray<byte>(1, Allocator.TempJob),
                    };

                    // run the job
                    _handle = _bulletJobOBB.Schedule();
                    // make sure the job finished this frame
                    _handle.Complete();

                    // grab the results
                    if (_bulletJobOBB.Out[0] == 1)
                        hit = checkCollider;
                }
                else if (shape == ColliderShape.Triangle)
                {
                    _bulletJobTriangle = new TriangleJob
                    {
                        CheckPosition = circleCollider.transform.position + circleCollider.Center,
                        CheckSize = circleCollider.Radius * circleCollider.transform.localScale.x,
                        a = checkCollider.LocalPointToWorld(checkCollider.PointA),
                        b = checkCollider.LocalPointToWorld(checkCollider.PointB),
                        c = checkCollider.LocalPointToWorld(checkCollider.PointC),
                        Out = new NativeArray<byte>(1, Allocator.TempJob),
                    };

                    // run the job
                    _handle = _bulletJobTriangle.Schedule();
                    // make sure the job finished this frame
                    _handle.Complete();

                    // grab the results
                    if (_bulletJobTriangle.Out[0] == 1)
                        hit = checkCollider;
                }
            }

            if (hit != null)
            {
                onCollision?.Invoke(new BulletContainer { Damage = _colliderDamage[hit], Position = transform.position }, hit, hit.gameObject);
                OnCollision?.Invoke(transform.position, hit);
            }
        }
    }
}