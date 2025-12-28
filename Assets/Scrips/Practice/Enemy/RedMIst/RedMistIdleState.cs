using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class RedMistIdleState : EnemyState
{
    private bool sign = false;
    private RedMist enemy;

    public RedMistIdleState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
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
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
        
        // 处理idle计时器，时间到了切换到出现状态
        if (stateTimer <= 0)
        {
            Debug.Log("切换到消失状态");
             stateMachine.ChangeState(enemy.disappearState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
