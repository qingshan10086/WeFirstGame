using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 第二种攻击状态
/// </summary>
public class AttackTwoState : EnemyState
{
    private Enemy_nothing enemy;

    public AttackTwoState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (Enemy_nothing)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入第二种攻击状态");
        enemy.ZeroVelocity(); // 停止移动
        // 设置攻击动画参数
        enemy.anim.SetInteger("combo", 0);
        stateTimer = enemy.attackCooldown; // 设置攻击冷却时间（稍微长一点）
    }

    public override void Updata()
    {
        base.Updata();
        Debug.Log("第二种攻击状态更新");
        // 攻击动画播放完毕后回到空闲状态
        if (enemy.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && 
    !enemy.anim.IsInTransition(0))
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("退出第二种攻击状态");
    }
}
