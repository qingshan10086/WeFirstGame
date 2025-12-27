using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private bool sign = false;
    private Enemy_nothing enemy;

    public EnemyIdleState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (Enemy_nothing)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
        stateTimer = enemy.idleTime;
    }

    public override void Updata()
    {
        base.Updata();
        if (enemy.IsPlayerDetected())
        {
            sign = true;
        }
        if (sign)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
