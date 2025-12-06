using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Death : Enemy
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public BattleState battleState { get; private set; }
    public AttackState attackState { get; private set; }
    public StunnedState stunnedState { get; private set; }
    public DeadState deadState { get; private set; }

   


    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState(this, stateMachine, "Idle", this);
        moveState = new MoveState(this, stateMachine, "Move", this);
        battleState = new BattleState(this, stateMachine, "Move", this);
        attackState = new AttackState(this, stateMachine, "Attack", this);
        stunnedState = new StunnedState(this, stateMachine, "Stunned", this);
        deadState = new DeadState(this, stateMachine, "Die", this);
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);//初始化状态
    }

    protected override void Update()
    {
        base.Update();

    }

    public override bool CanBeStunned()//被弹反状态优先级较高
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}
