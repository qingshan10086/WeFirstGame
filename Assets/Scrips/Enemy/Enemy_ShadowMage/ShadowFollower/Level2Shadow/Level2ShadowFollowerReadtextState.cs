using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2ShadowFollowerReadtextState : EnemyState
{
    private Level2_ShadowFollower enemy;
    public Level2ShadowFollowerReadtextState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Level2_ShadowFollower enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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

        enemy.ZeroVelocity();

        if (!enemy.Text.activeSelf)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
