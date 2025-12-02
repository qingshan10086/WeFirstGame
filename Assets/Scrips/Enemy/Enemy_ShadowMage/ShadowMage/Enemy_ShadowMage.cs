using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Enemy_ShadowMage : Enemy
{
    public GameObject Text;//获取对话框物体，用来实现对话时不能移动
    public ShadowMageReadTextState readTextState;//阅读文本状态
    public ShadowMageIdleState idleState {  get; private set; }//申明站立状态
    public ShadowMageAttack2State attack2State { get; private set; }//申明近身攻击（攻击2）状态
    public ShadowMageAttack3State attack3State { get; private set; }//申明召唤小怪并位移（攻击3）状态
    public  GameObject[] mosters;//小怪
    private float currentHealth;//当前血量百分数形式
    private bool[] canAttack3 = new bool[4] { true, true, true, true };



    public ShadowMageAttack1State attack1State { get; private set; }//申明攻击1状态

    [Header("攻击1相关")]
    [SerializeField] private float attack1Cooldown=60f;//攻击1间隔时间
    public float attack1CooldownTimer=0f;
    private bool canAttack1=false;//能否攻击1


    public ShadowMageDieState dieState { get; private set; }


    public EnemyStats stat;//获取敌人数据


    protected override void Awake()
    {
        base.Awake();
        idleState = new ShadowMageIdleState(this,stateMachine,"Idle",this);
        attack1State = new ShadowMageAttack1State(this, stateMachine, "Attack1", this);
        attack2State = new ShadowMageAttack2State(this, stateMachine, "Attack2", this);
        attack3State = new ShadowMageAttack3State(this, stateMachine, "Attack3", this);
        dieState = new ShadowMageDieState(this, stateMachine, "Die", this);
        readTextState = new ShadowMageReadTextState(this, stateMachine, "Idle", this);
        stat=GetComponent<EnemyStats>();//获取敌人数据
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(readTextState);//初始化状态
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
                CreatMosters();
                canAttack3[0] = false;
            }
        }
        if (canAttack3[1])
        {
            if (currentHealth <=0.6f && currentHealth >= 0.5f)
            {
                stateMachine.ChangeState(attack3State);
                CreatMosters();
                canAttack3[1] = false;
            }
        }
        if (canAttack3[2])
        {
            if (currentHealth <= 0.4f && currentHealth >= 0.3f)
            {
                stateMachine.ChangeState(attack3State);
                CreatMosters();
                canAttack3[2] = false;
            }
        }
        if (canAttack3[3])
        {
            if (currentHealth <= 0.2f && currentHealth >= 0.1f)
            {
                stateMachine.ChangeState(attack3State);
                CreatMosters();
                canAttack3[3] = false;
            }
        }
    }

    private void CreatMosters()
    {
        Vector2 left = new Vector2(0, 32);
        Vector2 right = new Vector2(75, 32);
        Vector2 top = new Vector2(38, 32);
        Vector2 left1= new Vector2(13, 6.15f);
        Vector2 right1 = new Vector2(62, 6.15f);

        Instantiate(mosters[1], left, Quaternion.identity);
        Instantiate(mosters[1], right, Quaternion.identity);
        Instantiate(mosters[1], top, Quaternion.identity);
        Instantiate(mosters[0],left1, Quaternion.identity);
        Instantiate(mosters[0],right1, Quaternion.identity);
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
              

                // 开始冷却
                canAttack1 = false;
                attack1CooldownTimer = attack1Cooldown;
            
        }
    }






    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(dieState);
    }
    public override RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * idleState.faceDir, 50, whatIsPlayer);//接收是否检测到玩家的射线结果
}

