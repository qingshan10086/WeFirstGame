using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState2 : EnemyState2
{
    public EnemyDeathState2(EnemyStateMachine2 _stateMachine, Enemy_nothing _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.ZeroVelocity();
        enemy.rb.bodyType = RigidbodyType2D.Static; // 死亡后禁用物理
        enemy.cd.enabled = false; // 死亡后禁用碰撞器
        enemy.anim.SetBool("death", true);
    }

    public override void Update()
    {
        base.Update();
        // 死亡状态不需要更新逻辑，保持直到销毁
    }

    public override void Exit()
    {
        base.Exit();
    }
}