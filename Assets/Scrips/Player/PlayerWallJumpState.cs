using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState//跳墙状态
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.5f;  //跳墙状态持续时间
        player.SetVelocity(5 * -player.faceDirection, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)//跳墙时间结束进入空中状态
        {
            stateMachine.ChangeState(player.airState);
        }

        if (player.IsGroundDetected())//如果检测到地面则进入站立状态
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
