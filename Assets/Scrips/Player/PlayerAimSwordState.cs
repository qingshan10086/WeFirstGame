using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState//Íæ¼ÒÃé×¼½£×´Ì¬
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
