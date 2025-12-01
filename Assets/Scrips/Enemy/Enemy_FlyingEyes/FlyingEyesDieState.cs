using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyesDieState : EnemyState
{
    private Enemy_FlyingEyes enemy;

    public FlyingEyesDieState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_FlyingEyes _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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
        enemy.ZeroVelocity();//死亡时速度为零
    }
}
