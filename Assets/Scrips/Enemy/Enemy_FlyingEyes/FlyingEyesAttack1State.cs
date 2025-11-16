using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyesAttack1State : EnemyState
{
    private Enemy_FlyingEyes enemy;

    public FlyingEyesAttack1State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_FlyingEyes _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        enemy.ZeroVelocity();//攻击时速度为零

        if (triggerCalled)//一完成攻击动画就进入战斗状态
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
