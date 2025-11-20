using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ShadowMage : Enemy
{
    public ShadowMageIdleState idleState {  get; private set; }
    protected override void Awake()
    {
        base.Awake();
        idleState = new ShadowMageIdleState(this,stateMachine,"Idle",this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);//³õÊ¼»¯×´Ì¬
    }

    protected override void Update()
    {
        base.Update();
    }
}
