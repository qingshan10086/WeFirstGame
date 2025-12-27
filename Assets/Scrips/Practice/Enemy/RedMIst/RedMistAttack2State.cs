using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class RedMistAttack2State : EnemyState
{
    private RedMist enemy;

    public RedMistAttack2State(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.attackCooldown;
        enemy.ZeroVelocity(); // 停止移动
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
        Debug.Log("RedMistAttack2State Enter");
        
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
            else if (playerDirection.x < 0 && enemy.faceDirection == -1)
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
    
    // 向当前朝向释放单个剑气
    // 可被动画事件调用
    public void ReleaseSingleSwordSlash()
    {
        // 检查剑气预制体是否存在
        if (enemy.swordSlashPrefab == null)
        {
            Debug.LogWarning("SwordSlashPrefab is not assigned in RedMist");
            return;
        }
        
        // 计算剑气生成位置：RedMist位置正前方
        Vector3 spawnPosition = enemy.transform.position + new Vector3(enemy.faceDirection * 0.5f, -0.2f, 0);
        
        // 实例化剑气
        GameObject slash = Object.Instantiate(enemy.swordSlashPrefab, spawnPosition, Quaternion.identity);
        SwordSlash swordSlash = slash.GetComponent<SwordSlash>();
        if (swordSlash != null)
        {
            // 向当前朝向发射剑气
            swordSlash.SetDirection(enemy.faceDirection);
        }
    }
}