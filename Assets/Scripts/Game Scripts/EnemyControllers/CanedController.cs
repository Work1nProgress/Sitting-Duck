using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanedController : EnemyController
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _attackPlayerRadius;
    [SerializeField] private int _attackDamage;
    [SerializeField] private float _attackWindUpTime;

    EnemyState _approachPlayerState;
    EnemyState _meleeAttackEnemyState;

    protected override void Start()
    {
        _approachPlayerState = new ApproachPlayerEnemyState(_walkSpeed, _attackPlayerRadius);
        _meleeAttackEnemyState = new MeleeAttackEnemyState(_attackWindUpTime, _attackDamage);
        _deathState = new DeathState();
        _deathState.OnDeathStateExited += DespawnSelf;

        if (ControllerGame.Instance.Player == null)
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
       ControllerGame.Instance.Player.transform,
       transform,
       _animator,
       new EnemyState[1] { _meleeAttackEnemyState },
       1,
       true,
       "Approach Player"
       );
        _approachPlayerState.InitializeState(stateData);

        stateData.transitionStates = new EnemyState[1] { _approachPlayerState };
        stateData.stateName = "Melee Attack Player";
        _meleeAttackEnemyState.InitializeState(stateData);

        stateData.timeInState = 0.9f;
        stateData.stateName = "Death State";
        _deathState.InitializeState(stateData);

        _activeState = _approachPlayerState;
        base.Start();
    }

    private IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => ControllerGame.Instance.Player != null);
        Init();
    }
}
