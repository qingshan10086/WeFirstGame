using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMageDieState : EnemyState
{
    private Enemy_ShadowMage enemy;
    public bool canNextText;//能否进入下一个文本的参数，用于TextTalk_BOSS1类中使用
    public ShadowMageDieState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_ShadowMage enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        canNextText = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();
        canNextText = true;
    }
}
