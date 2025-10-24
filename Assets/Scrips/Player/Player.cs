using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public bool isBusy {  get; private set; }

    [Header("Attack details")]
    public Vector2[] attackMovement;
   


    [Header("Move info")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashCooldown;
    [SerializeField]private float dashTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashFaceDir {  get; set; }

    






    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }

    public PlayerPrimaryAttackState primaryAttak {  get; private set; }
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

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }


    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

        CheckForInputDash();

        
    }




    public void AnimationTrigger()=>stateMachine.currentState.AnimationFinishTrigger();




    private void CheckForInputDash()
    {
        if (IsWallDetected())
        {
            return;
        }

        dashTimer -= Time.deltaTime;

        dashFaceDir = Input.GetAxisRaw("Horizontal");
        if (dashFaceDir == 0)
        {
            dashFaceDir = faceDirection;
        }

        if (Input.GetKeyDown(KeyCode.L)&&dashTimer<0)
        {
           
            stateMachine.ChangeState(dashState);
            dashTimer = dashCooldown;
        }
    }


    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

}
