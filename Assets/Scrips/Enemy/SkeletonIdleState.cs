using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState :SkeletonGroundedState
{
    public SkeletonIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateTimer =enemy.idleTime ;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
