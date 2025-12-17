using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMageReadTextState : EnemyState
{
    private Enemy_ShadowMage enemy;
    public ShadowMageReadTextState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_ShadowMage enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy=enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.attack1CooldownTimer = 0f;
    }

    public override void Updata()
    {
        base.Updata();
        enemy.attack1CooldownTimer = 3f;

        if (!enemy.Text.activeSelf)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
    
