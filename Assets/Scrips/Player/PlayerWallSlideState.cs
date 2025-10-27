using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState//滑墙状态
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (Input.GetKeyDown(KeyCode.K))//如果输入K就进入跳墙状态
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        if (yInput < 0)//控制滑墙速度
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y * 0.5f);//不输入下滑速度只有正常的一半
        }


        if (xInput != 0 && player.faceDirection != xInput)//退出滑墙，进入站立状态
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (player.IsGroundDetected())//到地面就进入站立状态
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
