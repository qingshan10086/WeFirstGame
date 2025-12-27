using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_nothing : Enemy
{
    [Header("CheckPlayer")]
    public float checkPlayerDisdance = 2.0f;
    public GameObject player;
    [Header("AttackRate info")]//技能概率输入
    public float attackRate = 0.3f;
    public float attackDamage = 10f; // 攻击伤害值
    
    [Header("Attack3 Cooldown")]
    public float attack3Cooldown = 5f; // Attack3技能冷却时间
    public float attack3CooldownTimer; // Attack3冷却计时器

    [Header("Attack Check info")]
    public Transform checkAttackSecond;
    public float checkAttackRangeSecond = 0.5f;
    public Transform checkAttackThird;
    public float checkAttackRangeThird = 0.5f;
    
    [Header("Spike Info")]
    public GameObject spikePrefab; // Spike预制体引用
    public float spikeSpawnOffset = 0.5f; // Spike生成位置的Y轴偏移
    
    [Header("Arrow Info")]
    public GameObject arrowPrefab; // Arrow预制体引用
    public float arrowSpeed = 5f; // 箭的飞行速度

    [Header("Move info")]
    public float moveTime = 2.0f; // 移动时间，用于EnemyMoveState类

#region States
    // 注意：这些状态类现在接受Enemy_nothing类型参数
    // 如果要在Enemy2中使用，需要创建对应版本的状态类
    public EnemyIdleState idleState;
    public EnemyMoveState moveState;
    public EnemyAttackState attackState;
    public EnemyDeathState deathState;
    public AttackOneState attackOneState;
    public AttackTwoState attackTwoState;
    public AttackThreeState attackThreeState;
#endregion

    // 覆盖Awake方法，初始化Enemy_nothing自己的状态机
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        
        // 初始化状态，使用Enemy_nothing类型
        idleState = new EnemyIdleState(this, stateMachine, "idle");
        moveState = new EnemyMoveState(this, stateMachine, "move");
        attackState = new EnemyAttackState(this, stateMachine, "attack");
        deathState = new EnemyDeathState(this, stateMachine, "death");
        
        // 初始化三种攻击状态
        attackOneState = new AttackOneState(this, stateMachine, "attack");
        attackTwoState = new AttackTwoState(this, stateMachine, "attack");
        attackThreeState = new AttackThreeState(this, stateMachine, "attack");
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
        
        // 更新attack3冷却计时器
        if (attack3CooldownTimer > 0)
        {
            attack3CooldownTimer -= Time.deltaTime;
        }
    }

    
    protected  bool IsPlayerInAttackRange()
    {
        return false;
    }

    // 重写IsPlayerDetected方法，使用checkPlayerDisdance作为检测范围
    public override RaycastHit2D IsPlayerDetected()
    {
        // 使用圆形检测来检测玩家，范围为checkPlayerDisdance
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, checkPlayerDisdance, whatIsPlayer);
        if (hitColliders.Length > 0)
        {
            // 返回第一个检测到的玩家碰撞体
            return Physics2D.Raycast(transform.position, hitColliders[0].transform.position - transform.position, checkPlayerDisdance, whatIsPlayer);
        }
        return new RaycastHit2D();
    }

    public  bool checkAttack()
    {
        return Physics2D.Raycast(anim.transform.position - new Vector3(attackDistance, 0), Vector2.right, attackDistance * 2, whatIsPlayer);
    }

    public void animTriggerEvent()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }
    
    public override void Die()
    {
        // 切换到死亡状态
        stateMachine.ChangeState(deathState);
        
        Debug.Log("Enemy_nothing 死亡了");
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // 绘制玩家检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkPlayerDisdance);
        
        // 绘制攻击检测范围
        Gizmos.color = Color.red;
        if(attackCheck != null) Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
        if(checkAttackSecond != null) Gizmos.DrawWireSphere(checkAttackSecond.position, checkAttackRangeSecond);
        if(checkAttackThird != null) Gizmos.DrawWireSphere(checkAttackThird.position, checkAttackRangeThird);
    }
}
