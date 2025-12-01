using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMageAttack2State : EnemyState//怪物的近身攻击
{
    private Enemy_ShadowMage enemy;

    public ShadowMageAttack2State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_ShadowMage enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.lastTimeAttacked = Time.time;
        enemy.stat.evasion.AddModifier(101);//添加超过100的闪避值，使怪物在此状态不能被攻击
    }

    public override void Exit()
    {
        base.Exit();
        enemy.stat.evasion.RemoveModifier(101);//去除该闪避值
    }

    public override void Updata()
    {
        base.Updata();

        if (triggerCalled)//一完成攻击动画就进入站立状态
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
