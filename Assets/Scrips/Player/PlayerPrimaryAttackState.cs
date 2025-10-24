using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 0.5f;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (comboCounter > 2||Time.time > lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }
        #region ChoiceAttackDir
        float attackDir = player.faceDirection;
        if(xInput!=0)
        {
            attackDir = xInput;
        }
        #endregion

        player.anim.SetInteger("ComboCounter", comboCounter);
        player.SetVelocity(player.attackMovement[comboCounter].x* attackDir, player.attackMovement[comboCounter].y);

        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("PrimaryAttack", false);
        player.StartCoroutine("BusyFor", 0.15f);
        lastTimeAttacked = Time.time;

        comboCounter++;
     
    }

    
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
           player.ZeroVelocity();
        }

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
