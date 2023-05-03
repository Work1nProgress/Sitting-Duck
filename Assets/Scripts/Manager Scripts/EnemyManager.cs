using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public SpawnPattern[] _spawnPatterns;

    List<SpawnLocation> _spawnLocations;

    public void Init()
    {
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
        foreach (SpawnPattern pattern in _spawnPatterns)
            pattern.Update();
    }

    private void SpawnEnemy(SpawnEnemyType enemyType)
    {
        string key = "";

        switch (enemyType)
        {
            //Add prefab names to this when they're made and set up in the pooler.
            case SpawnEnemyType.CANED:
                key = "";
                break;
            case SpawnEnemyType.MONOCLE:
                key = "";
                break;
            case SpawnEnemyType.UNITCUBUS:
                key = "";
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
    [Header("Basic Settings")]
    [Space(10)]
    [SerializeField] SpawnEnemyType _enemy;
    [Tooltip("How many enemies should be spawned on a spawn event.")]
    [SerializeField] int _spawnBatchSize;
    [Space(10)]
    [SerializeField] bool _limitSpawnEvents;
    [SerializeField] int _maximumSpawnEvents;
    [Space(20)]


    [Header("Time Settings")]
    [Space(10)]
    [Tooltip("Delay in seconds before this pattern starts spawning enemies.")]
    [SerializeField] bool _startWithDelay;
    [Tooltip("Delay in seconds before this pattern starts spawning enemies.")]
    [SerializeField] float _delay;
    [Space(10)]
    [SerializeField] SpawnTimeRange _spawnTimeRange;
    [Space(10)]
    [Tooltip("Sequence through maximum times after spawn events. Sequence starts with the above time range.")]
    [SerializeField] bool _sequenceThroughTimeRanges;
    [SerializeField] SpawnTimeRange[] _timeRanges;

    CountdownTimer _spawnTimer;
    CountdownTimer _delayTimer;
    int _spawnTimeRangeIndex;
    int _SpawnEventsExecuted = 0;

    public delegate void SpawnEnemySignature(SpawnEnemyType enemy);
    public event SpawnEnemySignature OnSpawnEnemy;

    public void Initialize()
    {
        if (_startWithDelay)
        {
            _delayTimer = new CountdownTimer(_delay, false, false);
            _delayTimer.OnTimerExpired += BeginSpawning;
        }

        _spawnTimer = new CountdownTimer(_spawnTimeRange.GetNewTime(), _startWithDelay, true);
        _spawnTimer.OnTimerExpired += SpawnEnemyBatch;

        if (_sequenceThroughTimeRanges)
        {
            _spawnTimeRangeIndex = 0;
        }
    }

    public void Update()
    {
        if (_delayTimer != null)
            _delayTimer.Update(Time.deltaTime);
        _spawnTimer.Update(Time.deltaTime);
    }

    private void SpawnEnemyBatch()
    {
        _SpawnEventsExecuted++;
        if (_SpawnEventsExecuted >= _maximumSpawnEvents)
            _spawnTimer.Pause();

        for (int i = 0; i < _spawnBatchSize; i++)
        {
            if (OnSpawnEnemy != null)
                OnSpawnEnemy.Invoke(_enemy);
        }

        if (_sequenceThroughTimeRanges)
        {
            if (_spawnTimeRangeIndex >= _timeRanges.Length)
            {
                _spawnTimer.SetNewTime(_spawnTimeRange.GetNewTime());
                _spawnTimer.Resume();
                _spawnTimeRangeIndex = 0;
            }
            else
            {
                _spawnTimer.SetNewTime(_timeRanges[_spawnTimeRangeIndex].GetNewTime());
                _spawnTimer.Resume();
                _spawnTimeRangeIndex++;
            }
        }


    }

    private void BeginSpawning()
    {
        _spawnTimer.Resume();
    }
}

[Serializable]
public class SpawnTimeRange
{
    [SerializeField] private bool randomizeSpawnTime;
    [SerializeField] private float spawnTime;
    [SerializeField] private float minimumSpawnTime;
    [Space(10)]
    [Tooltip("Reduces maximum spawn time after each spawn event. Does not go below minimum spawn time.")]
    [SerializeField] private bool reduceSpawnTime;
    [SerializeField] private float spawnTimeReduction;

    public float GetNewTime()
    {
        float time = 0;
        if (!randomizeSpawnTime)
            return time = spawnTime;
        else time = UnityEngine.Random.Range(minimumSpawnTime, spawnTime);

        if (reduceSpawnTime)
            spawnTime = Mathf.Clamp(spawnTime - spawnTimeReduction, minimumSpawnTime, Mathf.Infinity);

        return time;
    }
}

public enum SpawnEnemyType
{
    CANED,
    MONOCLE,
    UNITCUBUS,
    TESTENEMY
}