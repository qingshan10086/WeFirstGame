using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public bool isDead;//À¿Õˆ ±≤ªƒ‹≥Â¥Ã
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        isDead = true;
    }

    public override void Exit()
    {
        base.Exit();
        isDead = false;
    }

    public override void Update()
    {
        base.Update();

        player.ZeroVelocity();
    }
}
