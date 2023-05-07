using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonocleController : EnemyController
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private int _attackDamage;
    [SerializeField] private Vector2 _range;
    [SerializeField] private float shootDelay;


    EnemyState _holdDistanceState;
    EnemyState _shootPlayerState;

    protected override void Start()
    {
        _deathState = new DeathState();
        _deathState.OnDeathStateExited += DespawnSelf;

        _holdDistanceState = new HoldDistanceToPlayerEnemyState(_walkSpeed, _range);
        _shootPlayerState = new ShootEnemyState(_range, shootDelay);

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
        new EnemyState[1] { _shootPlayerState },
        1,
        true,
        "Hold Distance to Player"
        );
        _holdDistanceState.InitializeState(stateData);

        stateData.transitionStates = new EnemyState[1] { _holdDistanceState };
        stateData.stateName = "Ranged Attack Player";
        _shootPlayerState.InitializeState(stateData);

        stateData.timeInState = 0.9f;
        stateData.stateName = "Death State";
        _deathState.InitializeState(stateData);

        _activeState = _holdDistanceState;
        base.Start();
    }


    private IEnumerator WaitForPlayer()
    {
        yield return new WaitUntil(() => ControllerGame.Instance.PlayerController != null);
        Init();
    }
}
