using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第三种攻击状态
/// </summary>
public class AttackThreeState : EnemyState2
{
    public AttackThreeState(EnemyStateMachine2 _stateMachine, Enemy_nothing _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("进入第三种攻击状态");
        enemy.ZeroVelocity(); // 停止移动
        // 设置攻击动画参数
        enemy.anim.SetInteger("attackType", 3);
        stateTimer = enemy.attackCooldown * 1.5f; // 设置攻击冷却时间（最长）
    }

    public override void Update()
    {
        base.Update();
        // 攻击动画播放完毕后回到空闲状态
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("退出第三种攻击状态");
    }
}
