using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
public class RedMistAttack3State : EnemyState
{
    private RedMist enemy;


    public RedMistAttack3State(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        enemy.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 90);
        base.Enter();
        enemy.ZeroVelocity(); // 停止移动
        //更新攻击次数
        enemy.attack3Counter--;
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
        //向下冲刺
        enemy.rb.velocity= new Vector2(0,enemy.fallVelocity);
        if(enemy.IsGroundDetected())
        {
            // 落地后，切换到 idle 状态
            enemy.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
            
            // 检测到地面后，向左右释放剑气
            ReleaseSwordSlash();
            
            stateMachine.ChangeState(enemy.idleState);
        }
        
        
    }

    public override void Exit()
    {
       
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }
    
    // 向左右释放剑气
    public void ReleaseSwordSlash()
    {
        // 检查剑气预制体是否存在
        if (enemy.swordSlashPrefab == null)
        {
            UnityEngine.Debug.LogWarning("SwordSlashPrefab is not assigned in RedMist");
            return;
        }
        
        // 计算剑气生成位置：RedMist位置
        Vector3 spawnPosition = enemy.transform.position + new Vector3(0, enemy.slashDistance, 0);
        
        // 向左释放剑气
        GameObject leftSlash = UnityEngine.Object.Instantiate(enemy.swordSlashPrefab, spawnPosition, UnityEngine.Quaternion.identity);
        SwordSlash swordSlashLeft = leftSlash.GetComponent<SwordSlash>();
        if (swordSlashLeft != null)
        {
            swordSlashLeft.SetDirection(-1); // 向左移动
        }
        
        // 向右释放剑气
        GameObject rightSlash = UnityEngine.Object.Instantiate(enemy.swordSlashPrefab, spawnPosition, UnityEngine.Quaternion.identity);
        SwordSlash swordSlashRight = rightSlash.GetComponent<SwordSlash>();
        if (swordSlashRight != null)
        {
            swordSlashRight.SetDirection(1); // 向右移动
        }
    }
}