using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RedMistAttackState : EnemyState
{
    private RedMist enemy;

    public RedMistAttackState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.attackCooldown;
        enemy.ZeroVelocity();
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
        
        // 随机选择一种攻击状态
        int attackIndex = UnityEngine.Random.Range(0, 4);
        switch (attackIndex)
        {
            case 0:
                stateMachine.ChangeState(enemy.attack1State);
                break;
            case 1:
                stateMachine.ChangeState(enemy.attack2State);
                break;
            case 2:
                stateMachine.ChangeState(enemy.attack3State);
                break;
            case 3:
                stateMachine.ChangeState(enemy.attackDashState);
                break;
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
    }

    public override void Exit()
    {
        base.Exit();
    }
}
