using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustIdleState : DustGroudedState
{
    public DustIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Dust _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.idleTime;//站立时间
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
            stateMachine.ChangeState(enemy.groudedState);
        }
    }
}
