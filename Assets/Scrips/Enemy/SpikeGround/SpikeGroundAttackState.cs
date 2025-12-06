using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGroundAttackState : EnemyState
{
    private SpikeGround enemy;
    public SpikeGroundAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, SpikeGround enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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
        if (enemy.distance > 1)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
