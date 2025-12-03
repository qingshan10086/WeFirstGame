using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherGroundedState : EnemyState//骷髅兵地面状态，保含站立和移动
{
    protected  Enemy_Archer enemy;//申明骷髅兵
    protected Transform player;//申明玩家位置信息


    public ArcherGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;//获取玩家位置信息
    }

    public override void Exit()
    {
        base.Exit();
    }
    

    public override void Updata()
    {
        base.Updata();

        if (enemy.IsPlayerDetected()||Vector2.Distance(player.transform.position,enemy.transform.position)<2)//如果敌人检测到玩家，或者玩家靠近敌人，进入战斗状态
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}