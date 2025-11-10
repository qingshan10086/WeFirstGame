using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity//玩家类其父类为实体
{
    public GameObject Text;//获取对话框物体，用来实现对话时不能移动


    public bool isBusy {  get; private set; }  //用来辅助该状态是否能转入其他状态

    [Header("Attack details")]              //攻击相关数据
    public Vector2[] attackMovement;        //攻击小幅位移
    public float counterAttackDuration = 0.2f;//弹反状态
   


    [Header("Move info")]                   //移动输入
    public float moveSpeed;                 //移动速度
    public float jumpForce;                 //跳跃力


    [Header("Dash info")]                   //冲刺输入
    public float dashSpeed;                 //冲刺速度
    public float dashDuration;              //冲刺持续时间
    public float dashFaceDir {  get;private set; }     //冲刺方向

    



    public SkillManager skill {  get;private set; }//申明SkikllManager类


    #region States   各个状态
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
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);   //初始化状态
    }

    protected override void Update()
    {
        base.Update();

        if (Text.activeSelf){ stateMachine.ChangeState(readTextState); }

        stateMachine.currentState.Update();  

        CheckForInputDash();   //冲刺函数

        
    }




    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();  //用来获取动画完成相关




    private void CheckForInputDash()//冲刺函数，实现输入L便冲刺，因为它在player中所有它具有较高的优先级
    {
        if (IsWallDetected())   //在墙上不能冲刺
        {
            return;
        }

       

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


    public IEnumerator BusyFor(float _seconds)  //忙碌协程，来实现动画执行期间不会被打断
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
