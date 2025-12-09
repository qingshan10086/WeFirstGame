using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState2 : EnemyState2
{
    private bool sign=false;
    public EnemyIdleState2(EnemyStateMachine2 _stateMachine, Enemy_nothing _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.rb.velocity = new Vector2(0, enemy.rb.velocity.y);
        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();
        if(enemy.checkPlayer())
        {
            sign=true;
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