using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFollower : Enemy
{
    public ShadowFollowerIdleState idleState {  get; private set; }//申明站立状态
    private Enemy_ShadowMage enemy;//获取影法师


    private float follower1CoolDown = 3f;//攻击1的前摇(其实没用)原因未知
    private float follower1CoolDownTimer=1f;

    private float follower2CoolDown = 3f;//攻击2的前摇（其实也没有）原因未知
    private float follower2CoolDownTimer = 1f;
    protected override void Awake()
    {
        base.Awake();
        enemy=GetComponentInParent<Enemy_ShadowMage>();//获取影法师

        idleState = new ShadowFollowerIdleState(this, stateMachine, "Idle", this);

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);//初始化状态
    }

    protected override void Update()
    {
        base.Update();

        

        if (enemy.attack1State.follower1)//第一种攻击方式
        {
            
            follower1CoolDownTimer -= Time.deltaTime;
            if( follower1CoolDownTimer < 0)
            {               
                rb.velocity = new Vector2(0, -40);
                follower1CoolDownTimer = follower1CoolDown;
            }
            
        }

        if (enemy.attack1State.follower2)//第二种攻击方式
        {

            follower2CoolDownTimer -= Time.deltaTime;
            if (follower2CoolDownTimer<0)
            {
                rb.velocity = new Vector2(40, 0);
                follower2CoolDownTimer = follower2CoolDown;               
            }
           
        }

    }
}
