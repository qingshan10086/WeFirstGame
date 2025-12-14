using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherDeadState : EnemyState//÷¼÷ÃËÀÍö×´Ì¬
{
    private Enemy_Archer enemy;
    public ArcherDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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