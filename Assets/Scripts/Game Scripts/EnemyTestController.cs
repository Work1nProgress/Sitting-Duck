using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestController : EnemyController
{
    EnemyState _state1;
    EnemyState _state2;

    protected override void Start()
    {
        SetComponentReferences();

        EnemyState.EnemyStateData stateData = new EnemyState.EnemyStateData(
            transform,
            _rigidbody,
            _bulletSpawner,
            new EnemyState[1],
            0,
            "");

        stateData.transitionStates[0] = _state2;
        stateData.TimeInState = 3f;
        stateData.stateName = "I'm hungry!";
        _state1 = new ChargingEnemyState(stateData);
        _state1.InitializeState();

        stateData.transitionStates[0] = _state1;
        stateData.TimeInState = 5f;
        stateData.stateName = "I'm pissed off!";
        _state2 = new ChargingEnemyState(stateData);
        _state2.InitializeState();

        _activeState = _state2;

        base.Start();
    }
}
