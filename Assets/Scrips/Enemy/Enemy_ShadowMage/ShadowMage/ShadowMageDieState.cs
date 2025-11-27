using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMageDieState : EnemyState
{
    private Enemy_ShadowMage enemy;

    public ShadowMageDieState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_ShadowMage enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
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
