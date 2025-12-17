using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyState
{
    private Enemy_nothing enemy;

    public EnemyMoveState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (Enemy_nothing)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.moveTime;
    }

    public override void Updata()
    {
        base.Updata();
        
        RaycastHit2D playerHit = enemy.IsPlayerDetected();
        if (playerHit.collider != null)
        {
            // 检测到玩家，持续朝玩家移动
            Vector2 directionToPlayer = playerHit.transform.position - enemy.transform.position;
            
            // 翻转方向朝向玩家
            if (enemy.faceDirection * directionToPlayer.x < 0)
            {
                enemy.Flip();
            }
            
            // 朝玩家方向移动
            enemy.rb.velocity = new Vector2(enemy.moveSpeed * enemy.faceDirection, enemy.rb.velocity.y);
            
            // 检测墙壁或边缘，翻转方向但继续移动
            if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
            {
                enemy.Flip();
            }
            
            // 忽略移动时间限制，持续追逐玩家
            stateTimer = enemy.moveTime; // 重置移动时间
        }
        else
        {
            // 没有检测到玩家，使用原来的移动逻辑
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
        }
        
       
        
        // 检查attack3冷却是否完成，如果完成则释放技能
        if (enemy.attack3CooldownTimer <= 0 && enemy.player != null)
        {
            // 重置冷却计时器
            enemy.attack3CooldownTimer = enemy.attack3Cooldown;
            // 进入第三种攻击状态（召唤spike）
            stateMachine.ChangeState(enemy.attackThreeState);
        }
         // 检查是否可以攻击
        else if (enemy.checkAttack())
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
