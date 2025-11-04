using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDeadState : EnemyState//÷¼÷ÃËÀÍö×´Ì¬
{
    private Enemy_Skeleton enemy;
    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Updata()
    {
        base.Updata();

        enemy.ZeroVelocity();//ËÀÍöÊ±ËÙ¶ÈÎªÁã
    }
}
