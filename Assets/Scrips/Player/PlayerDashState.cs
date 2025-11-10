using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerDashState : PlayerState//冲刺状态
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        this.player= _player;
    }

    public override void Enter()
    {
        base.Enter();
        //player.skill.clone.CreatClone(player.transform);//冲刺时创造一个克隆体

        stateTimer = player.dashDuration;  //冲刺持续时间
    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocity(0, rb.velocity.y); //冲刺完成后x轴上立即停止
       
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashFaceDir, 0);

        if (!player.IsGroundDetected() && player.IsWallDetected())//冲刺如果碰到墙，则进入滑墙状态
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        if (stateTimer < 0)//冲刺正常停止进入站立状态
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
