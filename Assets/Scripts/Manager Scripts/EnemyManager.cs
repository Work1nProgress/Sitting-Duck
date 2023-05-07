using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public SpawnPattern[] _spawnPatterns;

    List<SpawnLocation> _spawnLocations;

    TimeTracker _timeTracker;

    public void Init()
    {
        _timeTracker = new TimeTracker();

        _spawnLocations = new List<SpawnLocation>();
        var spawnLocationsInGame = FindObjectsOfType<SpawnLocation>();
        for(int i = 0; i < spawnLocationsInGame.Length; i++)
        {
            SpawnLocation spawnLocation = spawnLocationsInGame[i];
            if (spawnLocation != null)
                _spawnLocations.Add(spawnLocation);
        }

        foreach(SpawnPattern pattern in _spawnPatterns)
        {
            pattern.Initialize();
            pattern.OnSpawnEnemy += SpawnEnemy;
        }
        
    }

    private void Update()
    {
        _timeTracker.Update();

        foreach (SpawnPattern pattern in _spawnPatterns)
            pattern.Update(
                _timeTracker.TimeElapsed,
                ControllerGame.Instance.PlayerEntity.GetLevel());
    }

    private void SpawnEnemy(SpawnEnemyType enemyType)
    {
        string key = "";

        switch (enemyType)
        {
            //Add prefab names to this when they're made and set up in the pooler.
            case SpawnEnemyType.CANED:
                key = "CanedEnemy";
                break;
            case SpawnEnemyType.MONOCLE:
                key = "";
                break;
            case SpawnEnemyType.UNITCUBUS:
                key = "UnitcubusEnemy";
                break;
            case SpawnEnemyType.TESTENEMY:
                key = "TestEnemy";
                break;
        }

        if(key != "")
            if (_spawnLocations.Count > 0)
            {
                int spawnLocationIndex = UnityEngine.Random.Range(0, _spawnLocations.Count);
                _spawnLocations[spawnLocationIndex].TrySpawn(key);
            }
    }


    
}

[Serializable]
public class SpawnPattern
{
    [SerializeField] SpawnEnemyType _enemy;
    [Tooltip("How many enemies should be spawned on a spawn event.")]
    [SerializeField] int _spawnBatchSize;
    [Tooltip("Spawn enemies every x seconds.")]
    [SerializeField]float _spawnFrequency;
    CountdownTimer _spawnTimer;
    [Space(10)]
    [SerializeField] SpawnRange _SpawnRange;
    
    bool _spawnEnemies;

    public delegate void SpawnEnemySignature(SpawnEnemyType enemy);
    public event SpawnEnemySignature OnSpawnEnemy;

    public void Initialize()
    {

        _spawnTimer = new CountdownTimer(_spawnFrequency, true, true);
        _spawnTimer.OnTimerExpired += SpawnEnemyBatch;
    }

    public void Update(float timeElapsed, int playerLevel)
    {
        _spawnTimer.Update(Time.deltaTime);

        switch (_SpawnRange.type)
        {
            case SpawnRangeType.LEVEL:
                if (IsInRange(playerLevel))
                    BeginSpawning();
                else StopSpawning();
                break;
            case SpawnRangeType.TIME:
                if (IsInRange(timeElapsed))
                    BeginSpawning();
                else StopSpawning();
                break;
        }
    }

    private void SpawnEnemyBatch()
    {
        for (int i = 0; i < _spawnBatchSize; i++)
        {
            if (OnSpawnEnemy != null)
                OnSpawnEnemy.Invoke(_enemy);
        }
    }

    private void BeginSpawning()
    {
        if (!_spawnEnemies)
        {
            _spawnEnemies = true;
            _spawnTimer.Resume();
        }
    }

    private void StopSpawning()
    {
        if (_spawnEnemies)
        {
            _spawnEnemies = false;
            _spawnTimer.Pause();
        }
    }

    private bool IsInRange(float value)
    {
        if (value <= _SpawnRange.end &&
            value >= _SpawnRange.start)
            return true;
        else return false;
    }

    private bool IsInRange(int value)
    {
        return IsInRange((float)value);
    }
}

[Serializable]
public class SpawnRange
{
    public SpawnRangeType type;
    public float start;
    public float end;
}

public enum SpawnRangeType
{
    TIME,
    LEVEL
}

public class TimeTracker
{
    private float _timeElapsed = 0;
    public float TimeElapsed => _timeElapsed;

    public void Update()
    {
        _timeElapsed += Time.deltaTime;
    }
}

public enum SpawnEnemyType
{
    CANED,
    MONOCLE,
    UNITCUBUS,
    TESTENEMY
}