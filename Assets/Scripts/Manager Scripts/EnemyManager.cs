using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    CountdownTimer _spawnTimer;
    private void Awake()
    {
        _spawnTimer = new CountdownTimer(2, false, true);
        _spawnTimer.OnTimerExpired += SpawnEnemy;
    }

    private void Update()
    {
        _spawnTimer.Update(Time.deltaTime);
    }

    private void SpawnEnemy()
    {
        PoolManager.Spawn<EnemyController>("TestEnemy", null, Vector3.zero);
    }

}