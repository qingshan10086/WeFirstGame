using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Enemy_FlyingEyes : Enemy//大眼睛怪物
{
    #region 各种状态
    public FlyingEyesFlyState flyState {  get; private set; }
    public FlyingEyesBattleState battleState { get; private set; }
    public FlyingEyesAttack1State attack1State { get; private set; }
    public FlyingEyesAttack2State attack2State { get; private set; }
    public FlyingEyesDieState dieState { get; private set; }
    #endregion




    protected override void Awake()
    {
        base.Awake();

        flyState = new FlyingEyesFlyState(this, stateMachine, "Fly", this);
        battleState = new FlyingEyesBattleState(this, stateMachine, "Fly", this);
        attack1State = new FlyingEyesAttack1State(this, stateMachine, "Attack1", this);
        attack2State = new FlyingEyesAttack2State(this, stateMachine, "Attack2", this);
        dieState = new FlyingEyesDieState(this, stateMachine, "Die", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(flyState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(dieState);
    }
}
