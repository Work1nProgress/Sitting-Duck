using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestController : EnemyController
{
    EnemyState _state1;
    EnemyState _state2;
    EnemyState _state3;
    EnemyState _state4;
    EnemyState _state5;

    protected override void Awake()
    {
        SetComponentReferences();

        _state1 = new ChargingEnemyState();
        _state2 = new ChargingEnemyState();
        _state3 = new ChargingEnemyState();
        _state4 = new ChargingEnemyState();
        _state5 = new ChargingEnemyState();

        EnemyState.EnemyStateData stateData = new EnemyState.EnemyStateData(
            null,
            _rigidbody,
            new EnemyState[1],
            0,
            false,
            "");

        stateData.transitionStates[0] = _state2;
        stateData.timeInState = 2f;
        stateData.stateName = "I'm hungry!";
        _state1.InitializeState(stateData);

        stateData.transitionStates[0] = _state3;
        stateData.timeInState = 2f;
        stateData.stateName = "I'm pissed off!";
        _state2.InitializeState(stateData);

        stateData.transitionStates[0] = _state4;
        stateData.timeInState = 2f;
        stateData.stateName = "I'm hungry!";
        _state3.InitializeState(stateData);

        stateData.transitionStates[0] = _state5;
        stateData.timeInState = 4f;
        stateData.stateName = "I'm pissed off!";
        _state4.InitializeState(stateData);

        stateData.transitionStates[0] = _state1;
        stateData.timeInState = 7f;
        stateData.stateName = "I'm worried that I'm going to die alone!";
        _state5.InitializeState(stateData);

        _activeState = _state1;

        base.Awake();
    }
}
