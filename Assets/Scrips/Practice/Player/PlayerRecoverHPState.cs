using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecoverHPState : PlayerState//回血状态，此时应该是无敌的
{

    public PlayerRecoverHPState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(6, null);//播放回血音效
        stateTimer = 0.5f;
        player.stat.evasion.AddModifier(101);

        player.stat.currentHealth = player.stat.currentHealth + (int)(0.3f * player.stat.GetMaxHealthValue());
        Debug.Log("恢复了" + (int)(0.3f * player.stat.GetMaxHealthValue()));
        if (player.stat.currentHealth >= player.stat.GetMaxHealthValue())
        {
            player.stat.currentHealth = player.stat.GetMaxHealthValue();

        }

    }

    public override void Exit()
    {
        base.Exit();
        player.stat.evasion.RemoveModifier(101);
        AudioManager.instance.StopSFX(6);
    }

    public override void Update()
    {
        base.Update();

        player.ZeroVelocity();
        if(stateTimer < 0)
        {
            
            stateMachine.ChangeState(player.idleState);
        }
    }
}
