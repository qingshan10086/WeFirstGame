using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ShadowMage : Enemy
{
    public ShadowMageIdleState idleState {  get; private set; }//申明站立状态
    public ShadowMageAttack2State attack2State { get; private set; }//申明近身攻击（攻击2）状态
    public ShadowMageAttack3State attack3State { get; private set; }//申明召唤小怪并位移（攻击3）状态
    private float currentHealth;//当前血量百分数形式
    private bool[] canAttack3 = new bool[4] { true, true, true, true };



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
        attack3State = new ShadowMageAttack3State(this, stateMachine, "Attack3", this);

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


        Attack3();

    }

    private void Attack3()//判断能否进入攻击3状态
    {
        currentHealth = (float)stat.currentHealth / stat.maxHealth.GetValue();

        if (canAttack3[0])
        {
            
            if (currentHealth <= 0.8f && currentHealth >= 0.7f)
            {
                stateMachine.ChangeState(attack3State);
                canAttack3[0] = false;
            }
        }
        if (canAttack3[1])
        {
            if (currentHealth <=0.6f && currentHealth >= 0.5f)
            {
                stateMachine.ChangeState(attack3State);
                canAttack3[1] = false;
            }
        }
        if (canAttack3[2])
        {
            if (currentHealth <= 0.4f && currentHealth >= 0.3f)
            {
                stateMachine.ChangeState(attack3State);
                canAttack3[2] = false;
            }
        }
        if (canAttack3[3])
        {
            if (currentHealth <= 0.2f && currentHealth >= 0.1f)
            {
                stateMachine.ChangeState(attack3State);
                canAttack3[3] = false;
            }
        }
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

