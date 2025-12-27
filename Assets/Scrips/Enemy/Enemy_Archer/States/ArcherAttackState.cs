using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttackState : EnemyState//骷髅兵攻击状态
{
    private Enemy_Archer enemy;

    public ArcherAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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

        enemy.lastTimeAttacked = Time.time;
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