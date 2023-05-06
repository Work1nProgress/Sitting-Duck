using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;

public class ControllerGame : ControllerLocal
{

    private static ControllerGame m_Instance;
    public static ControllerGame Instance => m_Instance;

    PlayerController playerController;
    public PlayerController Player => playerController;

    public Vector2 PlayerPosition => new Vector2(Player.transform.position.x, Player.transform.position.y);

    [SerializeField] private float _levelResetDelay;

    private CountdownTimer _levelResetTimer;
    private CountdownTimer[] _allTimers;

    private EntityStats _playerEntity;
    private List<EntityStats> _enemyEntities = new List<EntityStats>();




    public float OrbDuration = 10;


    #region Debug settings
    public bool RotateTowardsArrow;

    public bool AllowReverse;

    
    public float ReverseMinAngle = 91f;

    [Range(0, 180f)]
    public float DeadZone = 90f;

    [Range(0, 50f)]
    public float LookSpeed = 1f;


   


    #endregion
    [Header("DamageSettings")]
    [SerializeField]
    int PlayerBulletDamage = 10;

    [SerializeField]
    int EnemyBulletDamage = 1;

    [Space()]
    public DroneShootSettings[] DroneShootSettings;



    public override void Init()
    {
        m_Instance = this;
        PoolManager.Instance.Init();
        _levelResetTimer = new CountdownTimer(_levelResetDelay, true, false);
        _levelResetTimer.OnTimerExpired += GameManager.Instance.ResetCurrentScene;

        _allTimers = new CountdownTimer[]
        {
            _levelResetTimer
        };

        playerController = PoolManager.Spawn<PlayerController>("Player", null);
        
        Camera.main.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().Follow = playerController.transform;


        GetComponent<ControllerDrones>().Init();
        GetComponent < EnemyManager>().Init();
        GetComponentInChildren<BulletManager>().Init();
        base.Init();
    }
        

    void Update()
    {
        if (!isInitialized) return;
        foreach (CountdownTimer timer in _allTimers)
            timer.Update(Time.deltaTime);

        for (int i = chainsawTimers.Count-1; i >= 0; i--)
        {
            var ct = chainsawTimers[i];
            if (ct != null)
            {
                ct.Update(Time.deltaTime);
            }
        }
    }

    public void StartCurrentSceneReset(EntityStats player)
    {
        return;
        _levelResetTimer.Resume();
        Debug.Log("Resetting scene with delay of: " + _levelResetDelay + " seconds");
    }


    public void AddEntityReference(EntityStats entity, EntityType type)
    {
        switch (type)
        {
            case EntityType.Player:
                _playerEntity = entity;
                _playerEntity.OnDeath += StartCurrentSceneReset;
                break;
            case EntityType.Enemy:
                _enemyEntities.Add(entity);
                break;
        }
    }

    private void OnDestroy()
    {
        _levelResetTimer.OnTimerExpired -= GameManager.Instance.ResetCurrentScene;
    }

    List<CountdownTimer> chainsawTimers = new List<CountdownTimer>();
    List<EntityStats> enemiesInChainSaw = new List<EntityStats>();
    Dictionary<EntityStats, float> enemyToDamage = new Dictionary<EntityStats, float>();


    public void AddChainsawDamage(EntityStats stats)
    {
        if (stats == null || stats.Health <= 0)
        {
            return;
        }
        if (enemyToDamage.ContainsKey(stats))
        {
            enemyToDamage[stats] += PlayerBulletDamage * Random.Range(0.7f, 1.3f) * Time.fixedDeltaTime;
        }
        else
        {

            var ct = new CountdownTimer(1, false, true);
            ct.OnTimerExpired += () => ApplyChainSawDamage(stats);
            stats.OnDeath += OnEnemyDied;
            chainsawTimers.Add(ct);
            enemiesInChainSaw.Add(stats);
            enemyToDamage.Add(stats, 0);
        }

    }

    void OnEnemyDied(EntityStats stats)
    {
        if (stats == null)
        {
            return;
        }
        var idx = enemiesInChainSaw.FindIndex(x => stats);
        if (idx == -1)
        {
            return;
        }
        else
        {
            RemoveFromChainsaw(idx, stats);
        }
    }

    public void OnChainSawExit(EntityStats stats)
    {
        if (stats == null)
        {
            return;
        }
        var idx = enemiesInChainSaw.FindIndex(x => stats);
        if (idx == -1)
        {
            return;
        }
        else
        {
            ApplyChainSawDamage(stats);
            RemoveFromChainsaw(idx, stats);
        }
    }

    void RemoveFromChainsaw(int idx, EntityStats stats)
    {
        if (stats == null || chainsawTimers.Count == 0 || enemiesInChainSaw.Count == 0)
        {
            for (int i = 0; i < chainsawTimers.Count; i++)
            {
                chainsawTimers[i].Pause();
            }

            chainsawTimers.Clear();
            enemiesInChainSaw.Clear();
            enemyToDamage.Clear();
            return;
        }
      
        stats.OnDeath -= OnEnemyDied;
        enemyToDamage.Remove(stats);
        chainsawTimers[idx].Pause();
        chainsawTimers[idx] = null;
        chainsawTimers.RemoveAt(idx);
        enemiesInChainSaw.RemoveAt(idx);

     
    }

    void ApplyChainSawDamage(EntityStats entityStats) {
        if (enemyToDamage.ContainsKey(entityStats))
        {
            entityStats.Damage(Mathf.FloorToInt(enemyToDamage[entityStats]));
        }
    }

    public void OnBulletHit(EntityStats stats, BulletType bulletType)
    {
        switch (bulletType)
        {
            case BulletType.Player:
                stats.Damage(Mathf.CeilToInt(PlayerBulletDamage * Random.Range(0.7f, 1.3f)));
                break;
            case BulletType.Enemy:
                stats.Damage(EnemyBulletDamage);
                break;
        }

    }






}

[System.Serializable]
public class DroneShootSettings
{
    public string Name;
    public float ShootSpeed;
   
    public float BulletLifetime;
    
    public float AnimateBeforeShotTime;

    public float BurstDelay;

    public int BurstAmount;


    public int NumberOfBullets;

    public float ArcDegrees;

    public string ShootSFX;


}
