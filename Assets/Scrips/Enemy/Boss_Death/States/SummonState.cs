using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonState : EnemyState
{
    private Boss_Death enemy;

    public SummonState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        // 进入召唤时停止移动并播放召唤动画（由 animBoolName 控制）
        enemy.ZeroVelocity();

        // 立即执行召唤
        enemy.DoSummon();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}