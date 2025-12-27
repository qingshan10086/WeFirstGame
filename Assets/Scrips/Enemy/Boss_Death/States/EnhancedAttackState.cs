using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedAttackState : EnemyState
{
    private Boss_Death enemy;

    public EnhancedAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        // 启动强化伤害（如果还没启动）
        enemy.StartEnhancedBuffIfNeeded();
    }

    public override void Exit()
    {
        base.Exit();
        // 与普通攻击一致，记录上次攻击时间，防止连发
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Updata()
    {
        base.Updata();

        // 强化普攻时速度为零
        enemy.ZeroVelocity();

        // 动画命中/完成触发
        if (triggerCalled)
        {
            // 消耗一次强化普攻次数（会在最后一次恢复原始伤害）
            enemy.ConsumeEnhancedAttack();

            // 如果仍有强化普攻，则再次进入强化普攻状态，否则回战斗态
            if (enemy.HasEnhancedAttackPending())
            {
                stateMachine.ChangeState(enemy.enhancedAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}