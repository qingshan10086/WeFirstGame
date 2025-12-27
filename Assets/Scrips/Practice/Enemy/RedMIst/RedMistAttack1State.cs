using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class RedMistAttack1State : EnemyState
{
    private RedMist enemy;

    public RedMistAttack1State(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
       
        base.Enter();
        Debug.Log("攻击状态1开始操作");
        stateTimer = enemy.attackCooldown;
        enemy.ZeroVelocity(); // 停止移动
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
        
        // 获取玩家位置并设置攻击方向
        if (enemy.player != null)
        {
            // 计算玩家相对于敌人的位置
            Vector2 playerDirection = enemy.player.transform.position - enemy.transform.position;
            
            // 设置敌人朝向玩家方向
            if (playerDirection.x > 0 && enemy.faceDirection == 1)
            {
                enemy.Flip();
            }
            else if (playerDirection.x < 0 && enemy.faceDirection ==-1)
            {
                enemy.Flip();
            }
        }
    }

    public override void Updata()
    {
        base.Updata();
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
        
        // 如果攻击动画完成，切换回空闲状态
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }
}