using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGround : Enemy
{
    public GameObject player;
    public float distance;
    public SpikeGroundIdleState idleState {  get; private set; }
    public SpikeGroundAttackState attackState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        idleState = new SpikeGroundIdleState(this, stateMachine, "Idle");
        attackState = new SpikeGroundAttackState(this, stateMachine, "Attack",this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        distance=Vector2.Distance(player.transform.position,this.transform.position);
        if(distance <1  )
        {
            stateMachine.ChangeState(attackState);
        }
    }
}
