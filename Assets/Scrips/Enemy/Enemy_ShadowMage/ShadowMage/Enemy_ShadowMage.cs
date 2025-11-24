using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ShadowMage : Enemy
{
    public ShadowMageIdleState idleState {  get; private set; }//申明站立状态
    public ShadowMageAttack2State attack2State { get; private set; }//申明近身攻击（攻击2）状态


    public ShadowMageAttack1State attack1State { get; private set; }//申明攻击1状态

    [Header("攻击1相关")]
    [SerializeField] private float attack1Cooldown=60f;//攻击1间隔时间
    [SerializeField] private float attack1CooldownTimer=0f;
    private bool canAttack1=false;//能否攻击1


    public EnemyStats stat;//获取敌人数据


    protected override void Awake()
    {
        base.Awake();
        idleState = new ShadowMageIdleState(this,stateMachine,"Idle",this);
        attack1State = new ShadowMageAttack1State(this, stateMachine, "Attack1", this);
        attack2State = new ShadowMageAttack2State(this, stateMachine, "Attack2", this);

        stat=GetComponent<EnemyStats>();//获取敌人数据
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);//初始化状态
    }

    protected override void Update()
    {
        base.Update();

        Attack1();
    }

   
    private void Attack1()//判断是否进入攻击1状态
    {
        if (!canAttack1)
        {
            // 冷却中
            attack1CooldownTimer -= Time.deltaTime;
            if (attack1CooldownTimer <= 0)
            {
                canAttack1 = true;
                attack1CooldownTimer = 0;
            }
        }
        else
        {
            
                stateMachine.ChangeState(attack1State);
                Debug.Log("进入攻击1状态");

                // 开始冷却
                canAttack1 = false;
                attack1CooldownTimer = attack1Cooldown;
            
        }
    }







    public override RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * idleState.faceDir, 50, whatIsPlayer);//接收是否检测到玩家的射线结果
}

