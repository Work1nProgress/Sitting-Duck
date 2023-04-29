using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace BulletFury.Data
{
    public enum BulletPlane {XY, XZ}
    
    /// <summary>
    /// Container for bullet settings
    /// </summary>
    [CreateAssetMenu(menuName = "BulletFury/Bullet Settings")]
    public class BulletSettings : ScriptableObject
    {
        #region Fields

        [NonSerialized, CanBeNull] private Mesh _runtimeMesh;
        [SerializeField, Tooltip("the mesh to use for the bullet")] private Mesh mesh = null;
        // public accessor for the mesh - doing it this way means we can never set the mesh in code
        public Mesh Mesh
        {
            get
            {
                if (_runtimeMesh == null)
                    _runtimeMesh = mesh;
                return _runtimeMesh;
            }
        }
        public void SetMesh (Mesh newMesh) => _runtimeMesh = newMesh;

        [NonSerialized, CanBeNull] private Material _runtimeMaterial;
        [SerializeField, Tooltip("the material to use for the bullet")] 
        private Material material = null;
        // public accessor for the material - doing it this way means we can never set the material in code
        public Material Material
        {
            get
            {
                if (_runtimeMaterial == null)
                    _runtimeMaterial = material;
                return _runtimeMaterial;
            }
        }
        public Material SetMaterial (Material newMaterial) => _runtimeMaterial = newMaterial;
        
        [SerializeField] private BulletPlane plane = BulletPlane.XY;
        public BulletPlane Plane => plane;


        [SerializeField, Tooltip("wait until the bullet manager says go - used for bullet tracing")]
        private bool waitToStart = false;
        
 
        [SerializeField, Tooltip("the amount, in seconds, of the bullet's animation that should play before the bullet stops\nusing this means the bullet will spawn with an animation, rather than just not doing anything til the manager says go")] 
        private float timeToPlayWhileWaiting;


        [NonSerialized] private float? _runtimeLifetime;
        [SerializeField, Tooltip("the amount of time, in seconds, that the bullet should stay alive")] 
        private float lifetime = 1;

        public float Lifetime
        {
            get
            {
                _runtimeLifetime ??= lifetime;
                return _runtimeLifetime.Value;
            }
        }
        public void SetLifetime(float newLifetime) => _runtimeLifetime = newLifetime;

        [NonSerialized] private float? _runtimeSpeed;
        [SerializeField, Tooltip("how quickly the bullet should move, in unity units per second")] 
        private float speed = 10;

        public float Speed
        {
            get
            {
                _runtimeSpeed ??= speed;
                return _runtimeSpeed.Value;
            }
        }
        public void SetSpeed(float newSpeed) => _runtimeSpeed = newSpeed;
        
        [SerializeField, Tooltip("move all of the bullets with the current object")] 
        private bool moveWithTransform = false;

        [SerializeField, Tooltip("rotate all of the bullets with the current object")]
        private bool rotateWithTransform = false;

        [NonSerialized] private float? _runtimeDamage;
        [SerializeField] private float damage = 1;

        public float Damage
        {
            get
            {
                _runtimeDamage ??= damage;
                return _runtimeDamage.Value;
            }
        }
        
        public void SetDamage(float newDamage) => _runtimeDamage = newDamage;

        [SerializeField, Tooltip("how big, in unity units, the bullet should be\nthis affects scale - so a value of 1 correlates to a scale of (1,1,1)")] 
        private float size = 1;
        
        [SerializeField, Tooltip("how big, as a percentage of size, the bullet's collider should be.")] 
        private float colliderSize = 1;
        
        [SerializeField, Tooltip("Should the bullets be forced to spawn from the centre?")] 
        private bool spawnFromCentre = false;
        public bool SpawnFromCentre => spawnFromCentre;

        [SerializeField, Tooltip("How long should it take for the bullets to move to their original position?")] 
        private float secondsToOriginalPosition;
        public float SecondsToOriginalPosition => secondsToOriginalPosition;

        [NonSerialized] private float? _runtimeSize;
        public float Size
        {
            get
            {
                _runtimeSize ??= size;
                return _runtimeSize.Value;
            }
        }
        public void SetSize (float newSize) => _runtimeSize = newSize;

        [NonSerialized] private float? _runtimeColliderSize;
        public float ColliderSize
        {
            get
            {
                _runtimeColliderSize ??= colliderSize;
                return _runtimeColliderSize.Value;
            }
        }

        public void SetColliderSize(float newSize) => _runtimeColliderSize = newSize;
        
        
        [SerializeField, Tooltip("the amount the bullet should rotate, in degrees per second")] 
        private float angularVelocity = 0;
        [NonSerialized] private float? _runtimeAngularVelocity;

        public float AngularVelocity
        {
            get
            {
                _runtimeAngularVelocity ??= angularVelocity;
                return _runtimeAngularVelocity.Value;
            }
        }
        public void SetAngularVelocity(float newAngularVelocity) => _runtimeAngularVelocity = newAngularVelocity;

        [NonSerialized] private Color? _runtimeStartColor;
        [SerializeField, Tooltip("the start colour of the bullet"), ColorUsage(true, true)] 
        private Color startColor = Color.white;

        public Color StartColor
        {
            get
            {
                _runtimeStartColor ??= startColor;
                return _runtimeStartColor.Value;
            }
        }
        public void SetStartColor(Color newStartColor) => _runtimeStartColor = newStartColor;

        [SerializeField, Tooltip("colour the bullet over time (multiplied with the start colour)")] 
        private bool useColorOverTime = false;
        [SerializeField, Tooltip("Gradient's length is the total lifetime of the bullet, or a set time")]
        private CurveUsage colorCurveUsage;

        [SerializeField, Tooltip("Length of time the gradient describes")] 
        private float colorCurveTime;
        [SerializeField, GradientUsage(true)] private Gradient colorOverTime = null;


        [SerializeField, Tooltip("rotate the bullet over time")] private bool useRotationOverTime = false;
        [SerializeField, Tooltip("Curve's length is the total lifetime of the bullet, or a set time")]
        private CurveUsage rotationCurveUsage;

        [SerializeField, Tooltip("Length of time the curve describes")] 
        private float rotationCurveTime;
        [SerializeField] private AnimationCurve rotationOverTime = null;
        [SerializeField] private bool useRotationForDirection = true;

        [SerializeField, Tooltip("scale the bullet over time")] private bool useSizeOverTime = false;
        [SerializeField, Tooltip("Curve's length is the total lifetime of the bullet, or a set time")]
        private CurveUsage sizeCurveUsage;

        [SerializeField, Tooltip("Length of time the curve describes")] 
        private float sizeCurveTime;
        [SerializeField] private AnimationCurve sizeOverTime = null;
        

        [SerializeField, Tooltip("change velocity over time")] 
        private bool useVelocityOverTime = false;
        [SerializeField, Tooltip("local: use the bullet's forward/up/right axes rather than world axes\nworld: use the world forward/up/right axes rather than the object's local axes")] 
        private ForceSpace velocitySpace = ForceSpace.Local;

        [SerializeField, Tooltip("scale with speed or use the absolute value of the animation curve")] 
        private bool scaleWithSpeed = false;

        [SerializeField, Tooltip("Curve's length is the total lifetime of the bullet, or a set time")]
        private CurveUsage velocityCurveUsage;

        [SerializeField, Tooltip("Length of time the curve describes")] 
        private float velocityCurveTime;
        [SerializeField] 
        private Vector3 velocityScaleInDirection = Vector3.one;
        [SerializeField] private AnimationCurve velocityOverTimeX = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve velocityOverTimeY = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve velocityOverTimeZ = AnimationCurve.Linear(0,0,1, 0);

        [SerializeField] private bool trackObject;
        [SerializeField] private string trackedObjectTag;
        [SerializeField] private float turnSpeed;
        
        [SerializeField, Tooltip("add force over time. N.B. this is ADDITIVE, so it'll get fast quickly")] 
        private bool useForceOverTime = false;
        [SerializeField, Tooltip("Curve's length is the total lifetime of the bullet, or a set time")]
        private CurveUsage forceCurveUsage;

        [SerializeField, Tooltip("")] 
        private float forceCurveTime;
        [SerializeField] 
        private Vector3 forceScaleInDirection = Vector3.one;
        [SerializeField, Tooltip("local: use the bullet's forward/up/right axes rather than world axes\nworld: use the world forward/up/right axes rather than the object's local axes")] 
        private ForceSpace forceSpace = ForceSpace.Local;
        [SerializeField] private AnimationCurve forceOverTimeX = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve forceOverTimeY = AnimationCurve.Linear(0,0,1, 0);
        [SerializeField] private AnimationCurve forceOverTimeZ = AnimationCurve.Linear(0,0,1, 0);
#if UNITY_EDITOR
        [SerializeField] private bool isExpanded;
#endif
        #endregion

        private Dictionary<int, Transform> _trackedObjects = new Dictionary<int, Transform>();


        public void ResetFields()
        {
            // reset all _runtime fields to their non-runtime values
            _runtimeMesh = mesh;
            _runtimeMaterial = material;
            _runtimeLifetime = lifetime;
            _runtimeDamage = damage;
            _runtimeSize = size;
            _runtimeSpeed = speed;
            _runtimeAngularVelocity = angularVelocity;
            _runtimeColliderSize = colliderSize;
            _runtimeStartColor = startColor;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                _runtimeMesh = mesh;
                _runtimeMaterial = material;
                _runtimeLifetime = lifetime;
                _runtimeDamage = damage;
                _runtimeSize = size;
                _runtimeSpeed = speed;
                _runtimeAngularVelocity = angularVelocity;
                _runtimeColliderSize = colliderSize;
                _runtimeStartColor = startColor;
            }
        }
        
        public void SetTrackedObject(ref BulletContainer b, Transform obj)
        {
            if (!_trackedObjects.ContainsKey(b.Id))
                _trackedObjects.Add(b.Id, obj);
            else
                _trackedObjects[b.Id] = obj;
        }

        public void Setup()
        {
            _trackedObjects = new Dictionary<int, Transform>();
        }
        
        /// <summary>
        /// Initialise the bullet
        /// </summary>
        /// <param name="position">the starting value, as a percentage of the number of bullets spawned this cycle</param>
        /// <param name="bullet">a reference to the current bullet</param>
        public void Init(Vector3 originPos, ref BulletContainer bullet)
        {
            bullet.Lifetime = Lifetime;
            bullet.AngularVelocity = AngularVelocity;
            bullet.CurrentSpeed = Speed;
            bullet.StartSize = Size;
            bullet.CurrentSize = bullet.StartSize;
            bullet.StartColor = StartColor;
            bullet.Color = bullet.StartColor;
            bullet.RotationChangedThisFrame = 0;
            bullet.Waiting = waitToStart ? (byte) 1 : (byte) 0;
            bullet.TimeToWait = timeToPlayWhileWaiting;
            bullet.Forward = bullet.Rotation * Vector3.forward;
            bullet.Right = bullet.Rotation * Vector3.right;
            bullet.Up = bullet.Rotation * Vector3.up;
            if (trackObject)
            {
                if (!_trackedObjects.ContainsKey(bullet.Id))
                    _trackedObjects.Add(bullet.Id, null);
                
                _trackedObjects[bullet.Id] = GameObject.FindWithTag(trackedObjectTag)?.transform;
            }
            bullet.TrackObject = trackObject ? (byte) 1 : (byte) 0;
            bullet.Damage = Damage;

            if (spawnFromCentre)
            {
                bullet.OriginPosition = bullet.Position;
                bullet.Position = originPos;
                bullet.MovingToOrigin = 1;
                // set the bullet's MoveToOriginVelocity to the velocity needed to move the bullet to its OriginPosition over secondsToOriginalPosition seconds
                bullet.MoveToOriginVelocity = (bullet.OriginPosition - bullet.Position) / secondsToOriginalPosition;
                Debug.Log(bullet.MoveToOriginVelocity);
            }
            else
                bullet.MovingToOrigin = 0;
        }


        /// <summary>
        /// Set the values of the bullet
        /// </summary>
        /// <param name="bullet">the current bullet</param>
        /// <param name="deltaTime">cached Time.deltaTime</param>
        public void SetValues(ref BulletContainer bullet, float deltaTime, Transform transform, Vector3 previousPosition, Vector3 prevRotation, bool managerIsActive)
        {
            // if the bullet is dead or waiting, don't do anything
            if (bullet.Dead == 1 || (bullet.Waiting == 1 && bullet.CurrentLifeSeconds > bullet.TimeToWait))
                return;
            
            bullet.Lifetime = Lifetime;
            bullet.AngularVelocity = AngularVelocity;
            bullet.StartSize = Size;
            bullet.StartColor = StartColor;
            bullet.Damage = Damage;
            
            if (!managerIsActive)
                bullet.Dead = 1;

            // change the colour over time
            if (useColorOverTime)
            {

                bullet.Color = bullet.StartColor * colorOverTime.Evaluate(colorCurveUsage == CurveUsage.Lifetime
                    ? bullet.CurrentLifePercent
                    : bullet.CurrentLifeSeconds % colorCurveTime / colorCurveTime);
            }
            else
                bullet.Color = bullet.StartColor;

            // if we've got some extra angular velocity, rotate the bullet over time and mark the rotation as changed
            if (useRotationOverTime)
            {
                // if we've got some angular velocity, rotate the bullet over time and mark the rotation as changed
                if (Mathf.Abs(bullet.AngularVelocity) > 0)
                {
                    
                    if (plane == BulletPlane.XY)
                        bullet.Rotation *= Quaternion.Euler(0, 0, bullet.AngularVelocity * deltaTime);
                    else
                        bullet.Rotation *= Quaternion.Euler(0, bullet.AngularVelocity * deltaTime, 0);
                }
                    
                if (plane == BulletPlane.XY)
                    bullet.Rotation *= Quaternion.Euler(0, 0, rotationOverTime.Evaluate(rotationCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % rotationCurveTime / rotationCurveTime) * deltaTime);
                else
                    bullet.Rotation *= Quaternion.Euler(0, rotationOverTime.Evaluate(rotationCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % rotationCurveTime / rotationCurveTime) * deltaTime, 0);
                
                bullet.RotationChangedThisFrame = 1;
            }

            // if the bullet's rotation has changed this frame, calculate the forward/right/up axes for the bullet
            if (bullet.RotationChangedThisFrame == 1)
            {
                bullet.Forward = bullet.Rotation * Vector3.forward;
                bullet.Right = bullet.Rotation * Vector3.right;
                bullet.Up = bullet.Rotation * Vector3.up;
            }

            if (useRotationOverTime && bullet.TrackObject == 1 && _trackedObjects.ContainsKey(bullet.Id) && _trackedObjects[bullet.Id] != null)
            {
                var target = _trackedObjects[bullet.Id].position - (Vector3)bullet.Position;
                if (plane == BulletPlane.XY)
                    bullet.Up = Vector3.RotateTowards(bullet.Up, target, turnSpeed * deltaTime, 0.0f);
                else 
                    bullet.Forward = Vector3.RotateTowards(bullet.Forward, target, turnSpeed * deltaTime, 0.0f);

                bullet.Rotation = Quaternion.LookRotation(bullet.Forward, bullet.Up);
            }

            // change the size over time
            if (useSizeOverTime)
                bullet.CurrentSize = bullet.StartSize * sizeOverTime.Evaluate(sizeCurveUsage == CurveUsage.Lifetime
                    ? bullet.CurrentLifePercent
                    : bullet.CurrentLifeSeconds % sizeCurveTime / sizeCurveTime);
            else
                bullet.CurrentSize = bullet.StartSize;
            
            bullet.ColliderSize = bullet.CurrentSize * ColliderSize / 2f;
 
            // move the bullet forwards
            if (useRotationForDirection)
                bullet.Velocity = bullet.Rotation * (Plane == BulletPlane.XY ? Vector3.up : Vector3.forward);
            else
                bullet.Velocity = bullet.Direction * (Plane == BulletPlane.XY ? Vector3.up : Vector3.forward);

            if (moveWithTransform)
                bullet.Position += (float3)(transform.position - previousPosition);

            if (rotateWithTransform)
            {
                var rotationDelta = Quaternion.Euler(transform.eulerAngles - prevRotation);
                bullet.Position = (rotationDelta) * (bullet.Position - (float3) transform.position) + transform.position;
                bullet.Rotation = rotationDelta * bullet.Rotation;
            }
            
            // change the velocity over time
            if (useVelocityOverTime)
            {
                // if the force space is local, use the bullet's local axes
                if (velocitySpace == ForceSpace.Local)
                {
                    if (useRotationForDirection)
                    {
                        bullet.Velocity = bullet.Right * velocityOverTimeX.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.x +
                                          bullet.Up * velocityOverTimeY.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.y +
                                          bullet.Forward * velocityOverTimeZ.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.z;
                    }
                    else
                    {
                        bullet.Velocity = (bullet.Direction * Vector3.right) * velocityOverTimeX.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.x +
                                          (bullet.Direction * Vector3.up) * velocityOverTimeY.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.y +
                                          (bullet.Direction * Vector3.forward) * velocityOverTimeZ.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.z;
                    }
                }
                else // if the force space is world, use the world axes
                {
                    bullet.Velocity = new float3(
                        velocityOverTimeX.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.x,
                        velocityOverTimeY.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.y,
                        velocityOverTimeZ.Evaluate(velocityCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % velocityCurveTime / velocityCurveTime) * velocityScaleInDirection.z
                    );
                }

                // if we're scaling with the speed value, do that
                if (scaleWithSpeed)
                    bullet.Velocity *= bullet.CurrentSpeed;
            }
            else // if we're not using velocity over time, we definitely want to scale with speed;
                bullet.Velocity *= bullet.CurrentSpeed;

            // add force over time
            if (useForceOverTime)
            {
                // if the force space is local, use the bullet's local axes 
                if (forceSpace == ForceSpace.Local)
                {
                    bullet.Force += bullet.Right * forceOverTimeX.Evaluate(forceCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % forceCurveTime / forceCurveTime) * forceScaleInDirection.x +
                                   bullet.Up * forceOverTimeY.Evaluate(forceCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % forceCurveTime / forceCurveTime) * forceScaleInDirection.y +
                                   bullet.Forward * forceOverTimeZ.Evaluate(forceCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % forceCurveTime / forceCurveTime) * forceScaleInDirection.z;
                }
                else // if the force space is world, use the world axes
                {
                    bullet.Force += new float3(
                        forceOverTimeX.Evaluate(forceCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % forceCurveTime / forceCurveTime) * forceScaleInDirection.x,
                        forceOverTimeY.Evaluate(forceCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % forceCurveTime / forceCurveTime) * forceScaleInDirection.y,
                        forceOverTimeZ.Evaluate(forceCurveUsage == CurveUsage.Lifetime ? bullet.CurrentLifePercent : bullet.CurrentLifeSeconds % forceCurveTime / forceCurveTime) * forceScaleInDirection.z
                    );
                }
            }
            
            bullet.RotationChangedThisFrame = 0;
        }
    }
}