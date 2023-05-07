using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitcubusController : EnemyController
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _chargeSpeedMultiplier;
    [SerializeField] private float _chargePlayerRadius;
    [SerializeField] private float _chargeDelay;
    [SerializeField] private int _attackDamage;

    EnemyState _approachPlayerState;
    EnemyState _chargePlayerState;
    EnemyState _smashEnemyState;

    protected override void Start()
    {
        _deathState = new DeathState();
        _deathState.OnDeathStateExited += DespawnSelf;

        _approachPlayerState = new ApproachPlayerEnemyState(_walkSpeed, _chargePlayerRadius);
        _chargePlayerState = new ChargingEnemyState(_chargeDelay, _walkSpeed * _chargeSpeedMultiplier, 3, _attackDamage);
        _smashEnemyState = new SmashEnemyState(_attackDamage, 0.5f);

        if (ControllerGame.Instance.PlayerController == null)
        {
            StartCoroutine(WaitForPlayer());
            return;
        }

        Init();

    }



    private void Init()
    {
        EnemyState.EnemyStateData stateData = new EnemyState.EnemyStateData(
       this,
       ControllerGame.Instance.PlayerController.transform,
       transform,
       _animator,
       new EnemyState[2] { _chargePlayerState, _smashEnemyState },
       1000,
       true,
       "Approach Player"
       );
        _approachPlayerState.InitializeState(stateData);

        stateData.transitionStates = new EnemyState[1] { _approachPlayerState };
        stateData.stateName = "Charge Player";
        _chargePlayerState.InitializeState(stateData);

        stateData.timeInState = 0.9f;
        stateData.stateName = "Death State";
        _deathState.InitializeState(stateData);

        stateData.timeInState = 0.9f;
        stateData.stateName = "Smash State";
        _smashEnemyState.InitializeState(stateData);


        _activeState = _approachPlayerState;
        base.Start();
    }

    private IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => ControllerGame.Instance.PlayerController != null);
        Init();
    }
}
