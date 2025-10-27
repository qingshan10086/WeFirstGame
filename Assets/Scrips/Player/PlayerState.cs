using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState//玩家状态
{
    protected PlayerStateMachine stateMachine; //获取玩家状态机
    protected Player player;                   //玩家类

    protected Rigidbody2D rb;                  //获取重力组件

    protected float xInput;                    //x轴输入，通常与左右移动有关
    protected float yInput;                    //y轴输入，暂时只与滑墙速度有关
    private string animBoolName;               //动画bool变量名

    protected float stateTimer;                //状态持续时间
    protected bool triggerCalled;               //触发器是否调用

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)//构造函数，用来初始化
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }


    public virtual void Enter()             //状态内部初始化
    {
        player.anim.SetBool(animBoolName, true);    //将动画bool变量名设置为真，方便跳入动画
        rb = player.rb;                             //方便后续获取重力组件时不用player.rb
        triggerCalled = false;                      //
    }


    public virtual void Update()            //状态每帧执行
    {
        stateTimer -= Time.deltaTime;       //

        xInput = Input.GetAxisRaw("Horizontal");//水平轴输入
        yInput = Input.GetAxisRaw("Vertical");//垂直轴输入，当前还没做相关内容

        player.anim.SetFloat("yVelocity", rb.velocity.y);//跳跃状态机中上升与下降动画管理
    }


    public virtual void Exit()              //状态结束执行
    {
        player.anim.SetBool(animBoolName,false);        //将动画bool变量设置为假，方便退出动画
    }

    public virtual void AnimationFinishTrigger()        //动画完成函数，挂载在Animation上的，用于完成某些逻辑
    {
        triggerCalled = true;
    }
}