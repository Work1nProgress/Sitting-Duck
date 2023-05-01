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

    protected override void Awake()
    {
        SetComponentReferences();

        _approachPlayerState = new ApproachPlayerEnemyState(_walkSpeed, _attackPlayerRadius);
        _meleeAttackEnemyState = new MeleeAttackEnemyState(_attackWindUpTime, _attackDamage);

        if (ControllerGame.Instance == null)
        {
            StartCoroutine(WaitForController());
            return;
        }

        BeforeInit();

    }

    private void BeforeInit()
    {
        if (ControllerGame.Instance.Player == null)
        {
            StartCoroutine(WaitForPlayer());
        }
        else
        {
            Init();
        }
    }

    private void Init()
    {
        EnemyState.EnemyStateData stateData = new EnemyState.EnemyStateData(
      ControllerGame.Instance.Player.transform,
       _rigidbody,
       _bulletSpawner,
       new EnemyState[1] { _meleeAttackEnemyState },
       1,
       true,
       "Approach Player"
       );
        _approachPlayerState.InitializeState(stateData);

        stateData.transitionStates = new EnemyState[1] { _approachPlayerState };
        stateData.stateName = "Melee Attack Player";
        _meleeAttackEnemyState.InitializeState(stateData);

        _activeState = _approachPlayerState;
        base.Awake();
    }

    private IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => ControllerGame.Instance.Player != null);
        Init();
        

    }

    private IEnumerator WaitForController()
    {
        yield return new WaitUntil(() => ControllerGame.Instance != null);
        BeforeInit();


    }
}
