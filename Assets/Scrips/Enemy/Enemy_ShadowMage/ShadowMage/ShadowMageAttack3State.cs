using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class ShadowMageAttack3State : EnemyState
{
    private Enemy_ShadowMage enemy;

    private int random=3;//随机一个位移位置
    private float time = 1f;


    public ShadowMageAttack3State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_ShadowMage enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.stat.evasion.AddModifier(101);//添加超过100的闪避值，使怪物在此状态不能被攻击

        random = Random.Range(1, 4);

        time = 1f;
    }

    

    public override void Exit()
    {
        base.Exit();
        enemy.stat.evasion.RemoveModifier(101);//去除该闪避值
        time = 1f;
    }

    public override void Updata()
    {
        base.Updata();

        time-= Time.deltaTime;

        if (time < 0)
        {

            if (random == 1)
            {
                   enemy.transform.position =new Vector2(10, 6.15f);
            }

            if (random == 2) 
            {
                 enemy.transform.position = new Vector2(65, 6.15f);
            }
            if(random == 3)
            {
               enemy.transform.position = new Vector2(38, 11.25f);
            }

           
        }



        enemy.ZeroVelocity();//攻击时速度为零



        if (triggerCalled)//一完成攻击动画就进入战斗状态
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
