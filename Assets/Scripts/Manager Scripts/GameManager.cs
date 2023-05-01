using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _levelResetDelay;

    private CountdownTimer _levelResetTimer;
    private CountdownTimer[] _allTimers;

    private string _currentScene;

    private EntityStats _playerEntity;
    private List<EntityStats> _enemyEntities;

    void Awake()
    {
        _levelResetTimer = new CountdownTimer(_levelResetDelay, true, false);
        _levelResetTimer.OnTimerExpired += ResetCurrentScene;

        _allTimers = new CountdownTimer[]
        {
            _levelResetTimer
        };

        _currentScene = SceneManager.GetActiveScene().name;

        _enemyEntities = new List<EntityStats>();
    }

    void Update()
    {
        foreach (CountdownTimer timer in _allTimers)
            timer.Update(Time.deltaTime);
    }

    public void StartCurrentSceneReset()
    {
        _levelResetTimer.Resume();
        Debug.Log("Resetting scene with delay of: " + _levelResetDelay + " seconds");
    }

    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ResetCurrentScene()
    {
        LoadNewScene(_currentScene);
    }

    public void AddEntityReference(EntityStats entity, EntityType type)
    {
        switch (type)
        {
            case EntityType.Player:
                _playerEntity = entity;
                _playerEntity.OnDeath += StartCurrentSceneReset;
                Debug.Log("Add Player");
                break;
            case EntityType.Enemy:
                _enemyEntities.Add(entity);
                break;
        }
    }

    public Transform GetPlayerTransform() { return _playerEntity.transform; }
}
