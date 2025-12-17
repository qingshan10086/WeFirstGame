using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第一种攻击状态
/// </summary>
public class AttackOneState : EnemyState
{
    private Enemy_nothing enemy;

    public AttackOneState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (Enemy_nothing)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入第一种攻击状态");
        enemy.ZeroVelocity(); // 停止移动
        // 设置攻击动画参数
        enemy.anim.SetInteger("combo", 1);
        stateTimer = enemy.attackCooldown; // 设置攻击冷却时间
    }

    public override void Updata()
    {
        base.Updata();
        // 攻击动画播放完毕后回到空闲状态
        if (enemy.anim.GetBool("attack") == false)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("退出第一种攻击状态");
    }
}
