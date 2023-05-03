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



    #region Debug settings
    public bool RotateTowardsArrow;

    public bool AllowReverse;

    
    public float ReverseMinAngle = 91f;

    [Range(0, 180f)]
    public float DeadZone = 90f;

    [Range(0, 50f)]
    public float LookSpeed = 1f;



    #endregion



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
        base.Init();
    }
        

    void Update()
    {
        if (!isInitialized) return;
        foreach (CountdownTimer timer in _allTimers)
            timer.Update(Time.deltaTime);
    }

    public void StartCurrentSceneReset()
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






}
