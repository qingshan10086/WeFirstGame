using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_nothing : Enemy2
{
     public GameObject player;
    [Header("AttackRate info")]//技能概率输入
    public float attackRate=0.3f;

    [Header("Attack Check info")]
    public Transform checkAttackSecond;
    public float checkAttackRangeSecond=0.5f;
    public Transform checkAttackThird;
    public float checkAttackRangeThird=0.5f;

#region States
   // 注意：这些状态类现在接受Enemy_nothing类型参数
   // 如果要在Enemy2中使用，需要创建对应版本的状态类
   public AttackOneState attackOneState;
   public AttackTwoState attackTwoState;
   public AttackThreeState attackThreeState;
#endregion

    // 覆盖Awake方法，初始化Enemy_nothing自己的状态机
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        stateMachine = new EnemyStateMachine2();

        // 初始化状态，使用Enemy_nothing类型
        idleState = new EnemyIdleState2(stateMachine, this, "idle");
        moveState = new EnemyMoveState2(stateMachine, this, "move");
        attackState = new EnemyAttackState2(stateMachine, this, "attack");
        deathState = new EnemyDeathState2(stateMachine, this, "death");
        
        // 初始化三种攻击状态
        attackOneState = new AttackOneState(stateMachine, this, "attack");
        attackTwoState = new AttackTwoState(stateMachine, this, "attack");
        attackThreeState = new AttackThreeState(stateMachine, this, "attack");
    }
    
    protected override void Start()
    {
        base.Start();
        // 初始化状态机，设置初始状态为idle
        stateMachine.Initialize(idleState);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override bool IsPlayerInAttackRange()
    {
       return false;
    }

    

    // 修复checkPlayer方法中的错误引用
    public override bool checkPlayer()
    {
        // 使用父类中的checkPlayerRange变量
        return Physics2D.Raycast(anim.transform.position - new Vector3(checkPlayerRange, 0), Vector2.right, checkPlayerRange * 2, whatisPlayer);
    }

    public override bool checkAttack()
    {
        return Physics2D.Raycast(anim.transform.position - new Vector3(checkAttackRange, 0), Vector2.right, checkAttackRange * 2, whatisPlayer);
    }

    public void animTriggerEvent()
    {
        stateMachine.currentState.triggerEvent();
    }
 
protected override void OnDrawGizmos()
{
    base.OnDrawGizmos();
    Gizmos.DrawWireSphere(checkAttackSecond.position, checkAttackRangeSecond);
    Gizmos.DrawWireSphere(checkAttackThird.position, checkAttackRangeThird);
}
}
