using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyesAttack2State : EnemyState
{
    private Enemy_FlyingEyes enemy;
    

    private int attackDir;

    public FlyingEyesAttack2State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_FlyingEyes _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        attackDir = enemy.battleState.moveDir;
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        enemy.SetVelocity(20f * attackDir, rb.velocity.y);

        if (triggerCalled)//一完成攻击动画就进入战斗状态
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
