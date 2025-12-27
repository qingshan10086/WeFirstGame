using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFollower : Enemy
{
    public ShadowFollowerIdleState idleState {  get; private set; }//申明站立状态
    private Enemy_ShadowMage enemy;//获取影法师


  
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
            
            
                rb.velocity = new Vector2(0, -40);
         
            
        }

        if (enemy.attack1State.follower2)//第二种攻击方式
        {

                rb.velocity = new Vector2(40, 0);
          
           
        }

    }
}
