using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherIdleState :ArcherGroundedState//骷髅兵站立状态
{
    public ArcherIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateTimer =enemy.idleTime ;//站立时间
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        if (stateTimer < 0)//站立时间一结束，进入移动状态
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}