using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity//所有敌人的父类
{
    [SerializeField] protected LayerMask whatIsPlayer;//获取玩家所在层

    [Header("Stunned info")]//被弹反相关输入
    public float stunDuration;//可被弹反持续时间
    public Vector2 stunDirection;//可被弹反方向
    protected bool canBeStunned;//能否被弹反
    [SerializeField] protected GameObject counterImage;//弹反特效


    [Header("Move info")]//移动输入
    public float moveSpeed;//移动速度
    public float idleTime;//站立时间
    public float battleTime;//战斗状态持续时间

    [Header("Attack info")]//攻击输入
    public float attackDistance;//攻击距离
    public float attackCooldown;//攻击冷却
    [HideInInspector]public float lastTimeAttacked;//上一次攻击时间


    public EnemyStateMachine stateMachine { get; private set; }//声明敌人状态机


    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();//获取敌人状态机
    }
    
    


 
    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Updata();

        
    }

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, 50, whatIsPlayer);//接收是否检测到玩家的射线结果

    public virtual void AnimationFinishTrigger()=>stateMachine.currentState.AnimationFinishTrigger();//接受动画是否完成触发的函数


    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();//接收父类的画线，如地面检测，墙壁检测

        Gizmos.color= Color.yellow;//画黄线
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x + attackDistance * faceDirection, transform.position.y));//攻击检测射线的长度
    }

    public virtual bool CanBeStunned()//判断能否被弹反的函数
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    public virtual void OpenCounterAttackWindow()//打开可以被弹反，激活一个小红框
    {
        counterImage.SetActive(true);
        canBeStunned = true;
    }
    public virtual void CloseCounterAttackWindow()//关闭可以被弹反，失活一个小红框
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }


}
