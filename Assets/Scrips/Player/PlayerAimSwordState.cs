using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState//玩家瞄准剑状态//此状态暂时无用，之后可能会做
{



    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        player.ZeroVelocity();

        if (Input.GetKeyDown(KeyCode.U))
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
