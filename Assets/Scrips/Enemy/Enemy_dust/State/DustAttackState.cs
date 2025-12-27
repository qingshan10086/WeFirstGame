using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustAttackState : EnemyState
{
    private Enemy_Dust enemy;
    private Transform player;

    public DustAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Dust _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();
        // 获取玩家引用并确保面向玩家
        player = PlayerManager.instance.player.transform;
        // 使用距离差作为 FlipController 的输入，FlipController 只看符号
        enemy.FlipController(player.position.x - enemy.transform.position.x);

    }
    public override void Exit() { base.Exit(); }

    public override void Updata()
    {
        base.Updata();
        enemy.ZeroVelocity();//攻击时速度为零
        if (triggerCalled)//一完成攻击动画就进入战斗状态
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
