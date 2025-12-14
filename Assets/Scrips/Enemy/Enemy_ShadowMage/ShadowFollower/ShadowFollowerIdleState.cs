using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFollowerIdleState : EnemyState//随从站立状态，没实际作用
{
    private ShadowFollower enemy;
    public ShadowFollowerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, ShadowFollower enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();
    }
}
