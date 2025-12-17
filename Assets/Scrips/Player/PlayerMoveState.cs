using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState//移动状态
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(2, null);//播放移动音效
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(2);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput*player.moveSpeed,rb.velocity.y);

        if (xInput == 0)        //如果没有移动输入则转入站立状态
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
