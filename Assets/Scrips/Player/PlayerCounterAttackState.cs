using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState//玩家反击状态
{
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer=player.counterAttackDuration;//获得弹反状态时间
        player.anim.SetBool("SuccessfulCounterAttack", false);//初始设置为成功弹反
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.ZeroVelocity();//弹反时不应该移动

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);//获取攻击范围内的碰撞箱

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)//识别敌人类
            {
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = 10f;
                    player.anim.SetBool("SuccessfulCounterAttack", true);
                }
            }
        }


        if (stateTimer < 0 || triggerCalled)//trigger在动画Animation那边设置
        {
            stateMachine.ChangeState(player.idleState);//转入站立动画
           
        }

    }
}
