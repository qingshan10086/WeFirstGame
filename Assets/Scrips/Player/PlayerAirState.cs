using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState  //空中状态，用来过渡的状态
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetected())//如果检测到墙壁，则进入滑墙状态
        {
            stateMachine.ChangeState(player.wallSlideState);
        }


        if (player.IsGroundDetected())//如果检测到地面就进入站立状态
        {
            AudioManager.instance.PlaySFX(3, null);
            stateMachine.ChangeState(player.idleState);
        }


        if (xInput != 0)//保证空中可以移动
        {
            player.SetVelocity(player.moveSpeed * xInput, rb.velocity.y);
        }
    }
}
