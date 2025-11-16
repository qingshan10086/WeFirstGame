using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyesAttack2State : EnemyState
{
    private Enemy_FlyingEyes enemy;

    public FlyingEyesAttack2State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_FlyingEyes _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();
    }
}
