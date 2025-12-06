using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState//跳跃状态
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(4);//播放跳跃音效
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);   //初始化时输入跳跃力
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(4);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        if (rb.velocity.y < 0)    //如果向下掉就进入空中状态
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
    
