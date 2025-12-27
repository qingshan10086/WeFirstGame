using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2_ShadowFollower : Enemy
{
    public GameObject Text;//获取对话框物体，用来实现对话时不能移动


    public Level2ShadowFollowerIdleState idleState{  get; private set; }


    public Level2ShadowFollowerReadtextState readTextState{ get; private set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Level2ShadowFollowerIdleState(this, stateMachine, "Idle", this);
        readTextState = new Level2ShadowFollowerReadtextState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(readTextState);
    }

    protected override void Update()
    {
        base.Update();


        if (Text.activeSelf) { stateMachine.ChangeState(readTextState); }
    }
}
