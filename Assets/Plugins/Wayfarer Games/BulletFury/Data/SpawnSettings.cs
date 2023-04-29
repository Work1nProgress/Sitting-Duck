using System;
using WayfarerGames.Common;
using UnityEngine;

namespace BulletFury.Data
{
    /// <summary>
    /// Container for the bullet spawning settings
    /// </summary>
    [CreateAssetMenu(menuName = "BulletFury/Spawn Settings")]
    public class SpawnSettings : ScriptableObject
    {
        [NonSerialized] private float? _runtimeFireRate;
        [SerializeField, Tooltip("how often the bullet should fire, in seconds")] private float fireRate = 0.1f;
        // public accessor for the fire rate, so we can't change it in code
        public float FireRate 
        {
            get
            {
                _runtimeFireRate ??= fireRate;
                return _runtimeFireRate.Value;
            }
        }
        
        public void SetFireRate (float newFireRate) => _runtimeFireRate = newFireRate;

        [NonSerialized] private int? _runtimeBurstCount;
        [SerializeField, Tooltip("the number of bursts each shot should fire")] private int burstCount = 1;
        // public accessor
        public int BurstCount  
        {
            get
            {
                _runtimeBurstCount ??= burstCount;
                return _runtimeBurstCount.Value;
            }
        }
        public void SetBurstCount(int newBurstCount) => _runtimeBurstCount = newBurstCount;

        [NonSerialized] private float? _runtimeBurstDelay;
        [SerializeField, Tooltip("the delay between burst shots")] private float burstDelay = 0.1f;
        // public accessor
        public float BurstDelay 
        {
            get
            {
                _runtimeBurstDelay ??= burstDelay;
                return _runtimeBurstDelay.Value;
            }
        }
        public void SetBurstDelay(float newBurstDelay) => _runtimeBurstDelay = newBurstDelay;


        [NonSerialized] private SpawnDir? _runtimeSpawnDir;
        [SerializeField, Tooltip("the method by which we decide what direction to spawn the bullets in")]
        private SpawnDir spawnDir;
        
        public SpawnDir SpawnDir 
        {
            get
            {
                _runtimeSpawnDir ??= spawnDir;
                return _runtimeSpawnDir.Value;
            }
        }
        public void SetSpawnDir(SpawnDir newSpawnDir) => _runtimeSpawnDir = newSpawnDir;

        [NonSerialized] private float? _runtimeDirectionArc;
        [SerializeField, Range(0, 360), Tooltip("The arc to use for random directional spawning")]
        private float directionArc;
        public float DirectionArc 
        {
            get
            {
                _runtimeDirectionArc ??= directionArc;
                return _runtimeDirectionArc.Value;
            }
        }
        public void SetDirectionArc(float newDirectionArc) => _runtimeDirectionArc = newDirectionArc;

        [NonSerialized] private bool? _runtimeRandomise;
        [SerializeField, Tooltip("spawn points randomly rather than in a specific shape?")]
        private bool randomise;

        public bool Randomise
        {
            get
            {
                _runtimeRandomise ??= randomise;
                return _runtimeRandomise.Value;
            }
        }
        public void SetRandomise(bool newRandomise) => _runtimeRandomise = newRandomise;

        [NonSerialized] private bool? _runtimeOnEdge;
        [SerializeField, Tooltip("spawn all the random points at the radius of the shape?")] 
        private bool onEdge;
        public bool OnEdge
        {
            get
            {
                _runtimeOnEdge ??= onEdge;
                return _runtimeOnEdge.Value;
            }
        }
        public void SetOnEdge(bool newOnEdge) => _runtimeOnEdge = newOnEdge;

        [NonSerialized] private int? _runtimeNumSides;
        [SerializeField, Tooltip("the amount of sides the spawn shape should have")] private int numSides;

        public int NumSides
        {
            get
            {
                _runtimeNumSides ??= numSides;
                return _runtimeNumSides.Value;
            }
        }
        public void SetNumSides(int newNumSides) => _runtimeNumSides = newNumSides;


        [NonSerialized] private int? _runtimeNumPerSide;
        [SerializeField, Tooltip("the number of bullets per side the shape should have")] private int numPerSide;
        public int NumPerSide
        {
            get
            {
                _runtimeNumPerSide ??= numPerSide;
                return _runtimeNumPerSide.Value;
            }
        }
        public void SetNumPerSide(int newNumPerSide) => _runtimeNumPerSide = newNumPerSide;

        [NonSerialized] private float? _runtimeRadius;
        [SerializeField, Tooltip("the radius of the shape")] private float radius;

        public float Radius
        {
            get
            {
                _runtimeRadius ??= radius;
                return _runtimeRadius.Value;
            }
        }
        public void SetRadius(float newRadius) => _runtimeRadius = newRadius;

        [NonSerialized] private float? _runtimeArc;
        // the arc of the shape
        [SerializeField, Range(0, 360)] private float arc;

        public float Arc
        {
            get
            {
                _runtimeArc ??= arc;
                return _runtimeArc.Value;
            }
        }
        public void SetArc(float newArc) => _runtimeArc = newArc;
        
        #if UNITY_EDITOR
        [SerializeField] private bool isExpanded;
        #endif

        public void ResetFields()
        {
            _runtimeFireRate = null;
            _runtimeBurstCount = null;
            _runtimeBurstDelay = null;
            _runtimeSpawnDir = null;
            _runtimeDirectionArc = null;
            _runtimeRandomise = null;
            _runtimeOnEdge = null;
            _runtimeNumSides = null;
            _runtimeNumPerSide = null;
            _runtimeRadius = null;
            _runtimeArc = null;
        }
        
        /// <summary>
        /// Get a point based on the spawning settings
        /// </summary>
        /// <param name="onGetPoint"> a function to run for every point that has been found </param>
        public void Spawn(Action<Vector2, Vector2> onGetPoint, Squirrel3 rnd)
        {
            // initialise the array
            var points = new Vector2[NumSides];
            // take a first pass and add some points to every side

            var offset = Arc / (2 * NumSides) - ((0.5f * arc)) + 90f;
            var anglePerSide = Arc / NumSides;

            if (NumSides == 2)
            {
                Vector2 dir = Vector2.up;
                
                if (spawnDir == SpawnDir.Randomised)
                {
                    var rndAngle = rnd.Next() * DirectionArc * Mathf.Deg2Rad;
                    dir = new Vector2(Mathf.Cos(rndAngle), Mathf.Sin(rndAngle));
                }

                // for every bullet we should spawn on this side of the shape
                for (int i = 0; i < NumPerSide; ++i)
                {
                    // position the current point a percentage of the way between each end of the side
                    var t = i / (float) NumPerSide;
                    t += (1f / NumPerSide) / 2f;
                    var point = Vector2.Lerp(new Vector2(-1, 0), new Vector2(1, 0), t);
                    point *= Radius;

                    // tell function what the point and direction is 
                    onGetPoint?.Invoke(point, dir);
                }

                return;
            }

            for (int i = 0; i < NumSides; i++)
            {
                var angle = (!Randomise ? i * anglePerSide : rnd.Next() * Arc) + offset;  

                angle *= Mathf.Deg2Rad;
                points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                // set the direction based on the spawnDir enum
                Vector2 dir;
                switch (spawnDir)
                {
                    case SpawnDir.Directional:
                        dir = points[i];
                        break;
                    case SpawnDir.Randomised:
                        dir = Vector2.up;
                        break;
                    case SpawnDir.Spherised:
                        dir = points[i];
                        break;
                    default:
                        dir = Vector2.up;
                        break;
                }

                if (spawnDir == SpawnDir.Randomised)
                {
                    var rndAngle = rnd.Next() * DirectionArc * Mathf.Deg2Rad;
                    dir = new Vector2(Mathf.Cos(rndAngle), Mathf.Sin(rndAngle));
                }
                
                if ((Randomise || spawnDir == SpawnDir.Randomised) && !OnEdge)
                    points[i] *= rnd.Next();
                    

                if (NumPerSide == 1)
                    onGetPoint?.Invoke(points[i] * Radius, dir);
            }

            if (NumPerSide == 1)
                return;

            // for every side
            for (int i = 0; i < NumSides; ++i)
            {
                // get the next position
                var next = i + 1;
                if (next == NumSides)
                    next = 0;

                // the normal of the current side
                var direction = Vector2.Lerp(points[i], points[next], 0.5f).normalized;

                // for every bullet we should spawn on this side of the shape
                for (int j = 0; j < NumPerSide; ++j)
                {
                    // position the current point a percentage of the way between each end of the side
                    var t = j / (float) NumPerSide;
                    t += (1f / NumPerSide) / 2f;
                    var point = Vector2.Lerp(points[i], points[next], t);
                    point *= Radius;

                    Vector2 dir;
                    switch (spawnDir)
                    {
                        case SpawnDir.Directional:
                            dir = direction;
                            break;
                        case SpawnDir.Randomised:
                            var rndAngle = rnd.Next() * DirectionArc * Mathf.Deg2Rad;
                            dir = new Vector2(Mathf.Cos(rndAngle), Mathf.Sin(rndAngle));
                            break;
                        case SpawnDir.Spherised:
                            dir = point.normalized;
                            break;
                        default:
                            dir = Vector2.up;
                            break;
                    }
                    
                    // tell function what the point and direction is 
                    onGetPoint?.Invoke(point, dir);
                }
            }
        }
    }
}