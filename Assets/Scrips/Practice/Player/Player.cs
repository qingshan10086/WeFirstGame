using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity//������丸��Ϊʵ��
{
    public GameObject Text;//��ȡ�Ի������壬����ʵ�ֶԻ�ʱ�����ƶ�

    public PlayerStats stat;//��ȡ�������
    public bool isBusy {  get; private set; }  //����������״̬�Ƿ���ת������״̬

    [Header("Attack details")]              //�����������
    public Vector2[] attackMovement;        //����С��λ��
    public float counterAttackDuration = 0.2f;//����״̬
   


    [Header("Move info")]                   //�ƶ�����
    public float moveSpeed;                 //�ƶ��ٶ�
    public float jumpForce;                 //��Ծ��
    public float acceleration;              //�ӳ���
    public float deceleration;              //��ֹ��
    public float airAcceleration;           //�ڻ����ӳ���
    public float airDeceleration;           //�ڻ����ֹ��
    public float coyoteTime;                //Coyote time for more forgiving jumps
    public float jumpBufferTime;            //Jump input buffer time


    [Header("Dash info")]                   //�������
    public float dashSpeed;                 //����ٶ�
    public float dashDuration;              //��̳���ʱ��
    public float dashFaceDir {  get;private set; }     //��̷���

    [Header("��Ѫ������ȴ")]//��ʱ������������Ǳ߼̳�û����
    public float RecoverHPCooldown;
    public float RecoverHpCooldownTimer=0f;
    public bool  CanRecoverHP = false;

    #region �ܵ������޵�֡���
    public int currentHealth;//��ҵ�ǰѪ��
    private int lastHealth;//�����һ֡Ѫ��
    private float GodTimer =0.5f;//�޵�ʱ��
    private bool canStunned = true;//�ܷ��ܵ�����
    #endregion



    public SkillManager skill {  get;private set; }//����SkikllManager��


    #region States   ����״̬
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }

    public PlayerPrimaryAttackState primaryAttak {  get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }

    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerReadTextState readTextState { get; private set; }
    public PlayerRecoverHPState recoverHPState { get; private set; }
    #endregion


   

    protected override void Awake()
    {
        base.Awake();

        

        
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "WallJump");

        primaryAttak = new PlayerPrimaryAttackState(this, stateMachine, "PrimaryAttack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
        readTextState = new PlayerReadTextState(this, stateMachine, "Idle");
        recoverHPState = new PlayerRecoverHPState(this, stateMachine, "RecoverHP");

        stat=GetComponent<PlayerStats>();

      

    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;
        currentHealth=stat.GetMaxHealthValue();//��ȡ���Ѫ��
        lastHealth=stat.GetMaxHealthValue();//��ȡ���Ѫ��

        stateMachine.Initialize(idleState);   //��ʼ��״̬
    }

    protected override void Update()
    {
        base.Update();

        if (!CanRecoverHP)
        {
            RecoverHpCooldownTimer-=Time.deltaTime;
        }
        if (RecoverHpCooldownTimer < 0) { CanRecoverHP = true; }


        CanGodTime();


        if (Text.activeSelf) { stateMachine.ChangeState(readTextState); }

        stateMachine.currentState.Update();

        CheckForInputDash();   //��̺���


    }

    private void CanGodTime()//�ж��Ƿ���Դ����޵�ʱ�䣬��ֹͬʱ����̫��֡��
    {
        
        currentHealth = stat.currentHealth;
        if (currentHealth < lastHealth)
        {
            if (canStunned)
            {
                stat.evasion.AddModifier(101);
                canStunned = false;
            }
            GodTimer -= Time.deltaTime;
            if (GodTimer <= 0)
            {
                lastHealth = currentHealth;
                stat.evasion.RemoveModifier(101);
                GodTimer = 0.5f;
                canStunned = true;
            }
        }
        else if (currentHealth > lastHealth) 
        {
            lastHealth=currentHealth;
            stat.evasion.ClearModifiers();
        }
    }

    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();  //������ȡ����������




    private void CheckForInputDash()//��̺�����ʵ������L���̣���Ϊ����player�����������нϸߵ����ȼ�
    {
        if (IsWallDetected())   //��ǽ�ϲ��ܳ��
        {
            return;
        }

        if (deadState.isDead) { return; }//����ʱ���ܳ��

        dashFaceDir = Input.GetAxisRaw("Horizontal");
        if (dashFaceDir == 0)
        {
            dashFaceDir = faceDirection;
        }

        if (Input.GetKeyDown(KeyCode.L)&&SkillManager.instance.dash.CanUseSkill())
        {
            
            stateMachine.ChangeState(dashState);
            
        }
    }


    public IEnumerator BusyFor(float _seconds)  //æµЭ�̣���ʵ�ֶ���ִ���ڼ䲻�ᱻ���
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }


    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
}
