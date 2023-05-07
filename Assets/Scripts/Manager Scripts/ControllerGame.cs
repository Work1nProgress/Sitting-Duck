using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;
using System.Linq;

public class ControllerGame : ControllerLocal
{




    private static ControllerGame m_Instance;
    public static ControllerGame Instance => m_Instance;

    PlayerController playerController;
    public PlayerController PlayerController => playerController;

    public Vector2 PlayerPosition => new Vector2(playerController.transform.position.x, playerController.transform.position.y);

    [SerializeField] private float _levelResetDelay;
    [SerializeField] private ScreenFader _screenFader;

    private CountdownTimer _levelResetTimer;
    private CountdownTimer[] _allTimers;

    public RectTransform MainUIContainer;
    public RectTransform MessageContainer;
    [SerializeField]
    LevelUpPopup LevelUpPopup;

    [SerializeField]
    int CapstoneLevelInterval = 5;

    private EntityStats _playerEntity;
    public EntityStats PlayerEntity => _playerEntity;
    private List<EntityStats> _enemyEntities = new List<EntityStats>();

    [SerializeField]
    Upgrade[] Upgrades;

    Dictionary<UpgradeType, float> m_UpgradeValues;





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
    int StartingBulletDamage = 10;

    [SerializeField]
    int StartingChainsawDamage = 100;

    [SerializeField]
    int StaringRifleDroneNumber = 2;

    [SerializeField]
    int StaringShotgunDroneNumber = 2;
    [SerializeField]
    int StaringChainsawDroneNumber = 2;

    [SerializeField]
    int StartingBulletNumber = 3;

    [SerializeField]
    float StartingBulletSize = 1f;


    ControllerDrones drones;



    [SerializeField]
    int EnemyBulletDamage = 1;

    [Space()]
    public DroneShootSettings[] DroneShootSettings;

    public int BulletDamage => StartingBulletDamage + GetUpgradeValueInt(UpgradeType.BulletDamage);
    public int ChainsawDamage => StartingChainsawDamage + GetUpgradeValueInt(UpgradeType.MeeleDamage);

    public int RifleDroneNumber => StaringRifleDroneNumber + GetUpgradeValueInt(UpgradeType.DroneRifle);
    public int ShotgunDroneNumber => StaringShotgunDroneNumber + GetUpgradeValueInt(UpgradeType.DroneShotgun);
    public int ChainsawDroneNumber => StaringChainsawDroneNumber + GetUpgradeValueInt(UpgradeType.DroneChainSaw);
    public float BulletSize => StartingBulletSize + GetUpgradeValue(UpgradeType.BulletSize);
    public int BulletNumber => StartingBulletNumber + GetUpgradeValueInt(UpgradeType.BulletAmount);

    public override void Init()
    {
        m_Instance = this;
        m_UpgradeValues = new Dictionary<UpgradeType, float>();


        foreach (var upgrade in System.Enum.GetValues(typeof(UpgradeType)))
        {
            m_UpgradeValues.Add((UpgradeType)upgrade, 0);
        }


        PoolManager.Instance.Init();
        _levelResetTimer = new CountdownTimer(_levelResetDelay, true, false);
        _levelResetTimer.OnTimerExpired += GameManager.Instance.ResetCurrentScene;

        _allTimers = new CountdownTimer[]
        {
            _levelResetTimer
        };

        playerController = PoolManager.Spawn<PlayerController>("Player", null);
        
        Camera.main.transform.parent.GetComponentInChildren<CinemachineVirtualCamera>().Follow = playerController.transform;


        drones = GetComponent<ControllerDrones>().Init();
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

        var debugUpgrade = UpgradeType.None;
        if (Keyboard.current.digit1Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.BulletDamage;
        }
        if (Keyboard.current.digit2Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.MeeleDamage;
        }
        if (Keyboard.current.digit3Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.BulletAmount;
        }
        if (Keyboard.current.digit4Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.BulletSize;
        }
        if (Keyboard.current.digit5Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.DroneRifle;
        }
        if (Keyboard.current.digit6Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.DroneShotgun;
        }
        if (Keyboard.current.digit7Key.wasReleasedThisFrame)
        {
            debugUpgrade = UpgradeType.DroneChainSaw ;
        }

        for (int i = 0; i < Upgrades.Length; i++)
        {
            if (Upgrades[i].upgradeType == debugUpgrade)
            {
                var amount = Upgrades[i].Amount;
                if (Keyboard.current.shiftKey.isPressed)
                {
                    amount = -amount;
                }

                var fdn = PoolManager.Spawn<FloatingDamageNumber>("FloatingDamageNumber", MessageContainer);
                Upgrade(debugUpgrade, amount);
                fdn.Init($"{debugUpgrade}  {(amount > 0 ? "+" : "")}{amount}");
                return;
            }
        }
        


    }

    public void StartCurrentSceneReset(EntityStats player)
    {
        Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 128;
        SoundManager.Instance.Play("screamsplosion");
        PlayerController.AnimateDeath();
        _screenFader.StartFade();
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
                _playerEntity.OnLevelUpSuccess += OnLevelUp;
                break;
            case EntityType.Enemy:
                _enemyEntities.Add(entity);
                break;
        }
    }

    public int GetUpgradeValueInt(UpgradeType upgrade)
    {
        return Mathf.CeilToInt(m_UpgradeValues[upgrade]);
    }

    public float GetUpgradeValue(UpgradeType upgrade)
    {
        return m_UpgradeValues[upgrade];
    }

    public void Upgrade(UpgradeType upgrade, float amount)
    {



        m_UpgradeValues[upgrade] += amount;
        m_UpgradeValues[upgrade] = Mathf.Max(m_UpgradeValues[upgrade], 0);


        if (upgrade == UpgradeType.DroneRifle || upgrade == UpgradeType.DroneShotgun || upgrade == UpgradeType.DroneChainSaw)
        {
            m_UpgradeValues[upgrade] = Mathf.Min(m_UpgradeValues[upgrade], 8);
            drones.Refresh();
        }
    }


    void OnLevelUp(int level)
    {
        if (PlayerEntity.Health <= 0)
        {
            return;
        }
        List<Upgrade> possibleUpgrades = new List<Upgrade>();
        List<Upgrade> passedUpgrades = new List<Upgrade>();

        bool isCapstoneLevel = level % CapstoneLevelInterval == 0;
        if (isCapstoneLevel)
        {
            var capstones = System.Array.FindAll(Upgrades, x => x.Capstone);
            foreach (var u in capstones)
            {
                if (u.upgradeType == UpgradeType.DroneRifle && RifleDroneNumber < 8)
                {
                    possibleUpgrades.Add(u);
                }

                if (u.upgradeType == UpgradeType.DroneShotgun && ShotgunDroneNumber < 8)
                {
                    possibleUpgrades.Add(u);
                }

                if (u.upgradeType == UpgradeType.DroneChainSaw && ChainsawDroneNumber < 8)
                {
                    possibleUpgrades.Add(u);
                }
            }

           
        }

        if (possibleUpgrades.Count < 3)
        {
            var nonCapstones = System.Array.FindAll(Upgrades, x => !x.Capstone).ToList();
            while (possibleUpgrades.Count < 3)
            {
                var index = Random.Range(0, nonCapstones.Count);
                possibleUpgrades.Add(nonCapstones[index]);
                nonCapstones.RemoveAt(index);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            int idx = Random.Range(0, possibleUpgrades.Count);

            passedUpgrades.Add(possibleUpgrades[idx]);
            possibleUpgrades.RemoveAt(idx);

        }
        LevelUpPopup.Open(passedUpgrades);
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
            enemyToDamage[stats] += ChainsawDamage * Random.Range(0.7f, 1.3f) * Time.fixedDeltaTime;

            if (enemyToDamage[stats] > stats.Health)
            {
                ApplyChainSawDamage(stats);
            }
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
                stats.Damage(Mathf.CeilToInt(BulletDamage * Random.Range(0.7f, 1.3f)));
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


    public Vector2[] OffsetPosition;
    public float[] OffsetRotation;


}
