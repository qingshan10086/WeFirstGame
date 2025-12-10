using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState//主要攻击状态
{
    private int comboCounter;//攻击计数

    private float lastTimeAttacked;//上一次攻击时间，来判断是连续攻击还是间断攻击
    private float comboWindow = 0.5f;//间断攻击与连续攻击的判断时间
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(8, null);//播放攻击效果

        if (comboCounter > 2||Time.time > lastTimeAttacked + comboWindow)//重置攻击次数
        {
            comboCounter = 0;
        }

        #region ChoiceAttackDir  攻击方向
        float attackDir = player.faceDirection;
        if(xInput!=0)
        {
            attackDir = xInput;
        }
        #endregion

        player.anim.SetInteger("ComboCounter", comboCounter);//关联动画机的攻击计数
        player.SetVelocity(player.attackMovement[comboCounter].x* attackDir, player.attackMovement[comboCounter].y);//每次攻击的小位移
        xInput = 0;//重置攻击方向，方便攻击转向
        stateTimer = 0.1f;//攻击后摇
      
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(8);
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

        if (triggerCalled)//每次攻击结束进入站立状态，用triggerCalled是在Unity动画animation中调用函数
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
