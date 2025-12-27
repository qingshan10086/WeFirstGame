// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Enemy2 : Entity2
// {
//    public EnemyStateMachine2 stateMachine;

// #region States
//    // 注意：这些状态类现在接受Enemy_nothing类型参数
//    // 如果要在Enemy2中使用，需要创建对应版本的状态类
//    public EnemyIdleState2 idleState;
//    public EnemyMoveState2 moveState;
//    public EnemyAttackState2 attackState;
//    public EnemyDeathState2 deathState;
// #endregion

// #region Stats
//    [Header("移动属性")]
//    public float moveSpeed = 2f;
//    public float idleTime = 2f;
//    public float moveTime = 3f;

//    [Header("攻击属性")]
//    public float attackCooldown = 1f;
//    public float attackRange = 1.5f;
//    public int attackDamage = 10;


//     #endregion
// #region Check
// public float checkPlayerRange = 5;
// public float checkAttackRange = 1;
// #endregion

// #region  draw
// protected override void OnDrawGizmos()
//     {
//         base.OnDrawGizmos();
//         Gizmos.color=Color.red;
//         Gizmos.DrawLine(transform.position - new Vector3(checkAttackRange,1),transform.position + new Vector3(checkAttackRange,-1) );
//         Gizmos.color=Color.yellow;
//         Gizmos.DrawLine(transform.position - new Vector3(checkPlayerRange,0),transform.position + new Vector3(checkPlayerRange,0) );
//     }
// #endregion

//     #region UnityLife
//     protected override void Awake()
//    {
//        base.Awake();
//        stateMachine = new EnemyStateMachine2();

//        // 注意：由于状态类现在只接受Enemy_nothing类型，Enemy2不再初始化这些状态
//        // 如果需要在Enemy2中使用状态，需要创建接受Enemy2类型的状态类版本
//    }
//     protected override void Start()
//    {
//       base.Start();
//       stateMachine.Initialize(idleState); // 初始状态为空闲
//    }
//    protected override void Update()
//    {
//        base.Update();
//        stateMachine.currentState.Update();
       
//        // 简单的攻击检测 - 只有在非攻击和非死亡状态时才检测
//        if (IsPlayerInAttackRange() && stateMachine.currentState != attackState && stateMachine.currentState != deathState)
//        {
//            stateMachine.ChangeState(attackState);
//        }
//    }
// #endregion

// #region 自定义方法
//    protected virtual bool IsPlayerInAttackRange()
//    {
//        // 这里需要实现玩家检测逻辑，暂时返回false
//        return false;
//    }

//    public override void Die()
//    {
//        base.Die();
//        stateMachine.ChangeState(deathState);
//    }

//    public virtual bool checkPlayer()
//    {
//        // 这里需要实现玩家检测逻辑，暂时返回false
//        return false;
//    }

//    public virtual bool checkAttack()
//    {
//        // 这里需要实现攻击检测逻辑，暂时返回false
//        return false;
//    }

   
// #endregion
   
// }
