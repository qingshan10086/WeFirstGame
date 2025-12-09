using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class EnemyMoveState2 : EnemyState2
{
   
    public EnemyMoveState2(EnemyStateMachine2 _stateMachine, Enemy_nothing _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
    {
      
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.moveTime;
    }

    public override void Update()
    {
        base.Update();
        
        if(enemy.checkPlayer())
        {
            if(enemy.faceDirection*(enemy.player.transform.position.x-enemy.transform.position.x)<0)
            {
                enemy.Flip();
            }
        }
        // 直接设置速度，避免触发SetVelocity中的FlipController
        enemy.rb.velocity = new Vector2(enemy.moveSpeed * enemy.faceDirection, enemy.rb.velocity.y);

        // 检测墙壁或边缘，翻转方向
        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
        if(enemy.checkAttack())
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.ZeroVelocity();
    }
    
}