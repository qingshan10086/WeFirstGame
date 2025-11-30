using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState   //地面状态类，其包含了站立和移动状态，实现可以在两个状态中都进入下面代码中状态
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (player.CanRecoverHP)
        {
              if (Input.GetKeyDown(KeyCode.U))//按U则进入回血状态
              {
               
                player.CanRecoverHP = false;
                    player.RecoverHpCooldownTimer=player.RecoverHPCooldown;
                    stateMachine.ChangeState(player.recoverHPState);
              }

        }


        if (Input.GetKeyDown(KeyCode.Q))//按Q则进入弹反状态
        {
            
            stateMachine.ChangeState(player.counterAttackState);
        }

        if (!player.IsGroundDetected())      //不在地面就进入空中状态
        {
            stateMachine.ChangeState(player.airState);
        }


        if (Input.GetKeyDown(KeyCode.K)&&player.IsGroundDetected())  //在地面且按了K,则进入跳跃状态
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (Input.GetKey(KeyCode.J))          //按了J则进入攻击状态
        {
            
            stateMachine.ChangeState(player.primaryAttak);
        }

    }
}
