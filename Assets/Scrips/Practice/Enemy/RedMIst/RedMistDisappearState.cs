using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMistDisappearState : EnemyState
{
    private RedMist enemy;

    public RedMistDisappearState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        // 设置消失状态的持续时间
        stateTimer = 1.5f; // 假设消失动画持续1.5秒
        enemy.ZeroVelocity(); // 停止移动
        
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
        
        // 如果状态计时器结束，根据血量决定切换到哪个状态
        if (stateTimer <= 0)
        {
            if (enemy.isBelowHalfHealth && enemy.specialAttackTimer <= 0)
            {
                // 血量低于50%且特殊攻击冷却结束，切换到特殊攻击状态
                stateMachine.ChangeState(enemy.specialAttackState);
            }
            else
            {
                // 血量高于50%或特殊攻击冷却未结束，切换到出现状态
                stateMachine.ChangeState(enemy.appearState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        // 消失状态结束时的清理
    }
}