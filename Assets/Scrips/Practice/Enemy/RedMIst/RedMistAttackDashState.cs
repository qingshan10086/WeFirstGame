using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMistAttackDashState : EnemyState
{
    private RedMist enemy;
    private float dashSpeed ;
    private Vector3 targetPosition;
    private Vector2 dashDirection;

    public RedMistAttackDashState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    // 保存原始旋转角度
    private float originalRotation;
    
    public override void Enter()
    {
         // 技能开始时读取玩家位置（只读取一次）
        if (enemy.player != null)
        {
            targetPosition = enemy.player.transform.position;
            // 计算冲刺方向：从当前位置（出现点）到玩家位置的方向
            dashDirection = (targetPosition - enemy.transform.position).normalized;
           
            // //控制转向
            // if(dashDirection.x < 0&&enemy.faceDirection == -1)
            // {
            //     enemy.Flip();
            // }
            // else if(dashDirection.x > 0&&enemy.faceDirection == 1)
            // {
            //     enemy.Flip();
            // }
             //设置冲刺方向取负值
            dashDirection = -dashDirection;
            // 保存原始旋转角度
            originalRotation = enemy.transform.rotation.eulerAngles.z;
            // 设置冲刺速度
            dashSpeed = enemy.attackDashSpeed;
            
            // 计算旋转角度：根据冲刺方向计算角色应该朝向的角度
            float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
            Debug.Log("冲刺方向角度: " + angle);
            // 设置敌人旋转到冲刺方向
            enemy.transform.rotation = Quaternion.Euler(0, 0, angle);
            dashDirection = -dashDirection;

        }

        base.Enter();
        //更新攻击次数
        enemy.dashAttackCounter--;
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
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
        
        // 持续朝目标位置冲刺
        if (enemy.player != null)
        {
            enemy.rb.velocity = dashDirection * dashSpeed;
        }
        
        // 检测墙壁，如果检测到则停止冲刺并进入消失状态
        if (enemy.IsWallDetected())
        {
            enemy.ZeroVelocity();
            
            // 恢复重力
            enemy.rb.gravityScale = enemy.defaultGravityScale;
            
            // 切换到消失状态
            stateMachine.ChangeState(enemy.disappearState);
            return;
        }
        
        // 检测地面，如果检测到则停止冲刺并进入攻击状态
        if (enemy.IsGroundDetected())
        {
            enemy.ZeroVelocity();
            
            // 恢复重力
            enemy.rb.gravityScale = enemy.defaultGravityScale;
            
            // 切换到攻击状态
            TriggerAttackTransition();
            return;
        }
        
        // 检查是否到达目标位置附近
        if (enemy.player != null && Vector2.Distance(enemy.transform.position, targetPosition) < 0.5f)
        {
            enemy.ZeroVelocity();
            
            // 恢复重力
            enemy.rb.gravityScale = enemy.defaultGravityScale;
            
            // 切换到攻击状态
            TriggerAttackTransition();
            return;
        }
        
        // 如果攻击动画完成，切换到攻击状态
        if (triggerCalled)
        {
            enemy.ZeroVelocity();
            
            // 恢复重力
            enemy.rb.gravityScale = enemy.defaultGravityScale;
            
            TriggerAttackTransition();
        }
    }
    
    // 随机切换到Attack1或Attack2状态
    private void TriggerAttackTransition()
    {
        int attackIndex = UnityEngine.Random.Range(0, 2);
        if (attackIndex == 0)
        {
            stateMachine.ChangeState(enemy.attack1State);
        }
        else
        {
            stateMachine.ChangeState(enemy.attack2State);
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
        
        // 确保重力被恢复
        enemy.rb.gravityScale = enemy.defaultGravityScale;
        
        // 恢复原始旋转角度
        enemy.transform.rotation = Quaternion.Euler(0, 0, originalRotation);
    }
}