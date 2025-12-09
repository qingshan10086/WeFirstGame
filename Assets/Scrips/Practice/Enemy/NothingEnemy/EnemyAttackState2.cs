using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class EnemyAttackState2 : EnemyState2
{
    public EnemyAttackState2(EnemyStateMachine2 _stateMachine, Enemy_nothing _enemy, string _animBoolName) : base(_stateMachine, _enemy, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemy.attackCooldown;
        enemy.ZeroVelocity();
        // 随机选择一种攻击状态
    }



    public override void Update()
    {
        base.Update();
        if (stateTimer <= 0)
        {
            int attackChoice = UnityEngine.Random.Range(1, 3); // 1, 2
            Debug.Log("随机选择攻击类型: " + attackChoice);

            // 切换到具体的攻击状态
            switch (attackChoice)
            {
                case 1:
                    stateMachine.ChangeState(enemy.attackOneState);
                    break;
                case 2:
                    stateMachine.ChangeState(enemy.attackTwoState);
                    break;

            }
        }
    }

                // 这个状态只是一个过渡状态，不做其他处理


    public override void Exit()
    {
        base.Exit();
    }
}
