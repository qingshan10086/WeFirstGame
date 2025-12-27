using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMist : Enemy
{
    public System.Action onFlipped;//血条不翻转委托
    [Header("移动参数")]
    [SerializeField] private float fixedJumpSpeed = 5f;        //固定跳跃速度
    [SerializeField] public float playerDetectionRange = 3f;  //玩家检测范围


    [Header("攻击参数")]
    [SerializeField] public Transform checkAttack;         //获取攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] public float checkAttackRange;        //攻击检测的范围  
    [Header("矩形攻击参数")]
    [SerializeField] public Transform checkAttackBox;      //矩形攻击检测的位置
    [SerializeField] public Vector2 checkAttackBoxSize;    //矩形攻击检测的大小
    
    [Header("圆形攻击参数")]
    [SerializeField] public Transform checkCircleAttack;   //圆形攻击检测的位置
    [SerializeField] public float checkCircleAttackRadius; //圆形攻击检测的半径
    
    public GameObject player;
    
    #region States
    // 使用RedMist的状态类
    public RedMistIdleState idleState;

    public RedMistAttackState attackState;
    public RedMistDeathState deathState;
    // 新增状态类
    public RedMistDisappearState disappearState;
    public RedMistAppearState appearState;
    public RedMistAttack1State attack1State;
    public RedMistAttack2State attack2State;
    public RedMistAttack3State attack3State;
    public RedMistAttackDashState attackDashState;
    public RedMistSpecialAttackState specialAttackState;
    #endregion
    
    [Header("消失/出现参数")]
    public float disappearInterval = 8f; // 消失状态的间隔时间
    private float disappearTimer; // 消失状态的计时器
    public bool isBelowHalfHealth = false; // 标记是否低于50%血量
    
    [Header("出现位置参数")]
    public float sideAppearDistance = 2f; // 玩家身旁出现的距离
    public float diagonalAppearDistance = 3f; // 玩家斜上方出现的距离
    public float topAppearDistance = 4f; // 玩家正上方出现的距离
    
    [Header("特殊攻击参数")]
    public float specialAttackChance = 0.3f; // 特殊攻击的概率
    public float specialAttackInterval = 30f; // 特殊攻击的间隔时间
    public float specialAttackTimer; // 特殊攻击的计时器
    
    // 物理参数
    public float defaultGravityScale; // 默认重力缩放值
    [Header("攻击冲刺参数")]
     public float attackDashSpeed = 15f; // 攻击冲刺速度
    public int dashDamage = 10;  // 攻击冲刺伤害
    public int maxConsecutiveDashAttacks = 2; // 最大连续攻击次数
    public int dashAttackCounter = 0; // dash attack的连续攻击计数器

    public float dashAttackCooldown = 0.5f;//避免秒伤，设置伤害频率
    public float dashAttackDamageTimer = 10;  // 攻击冲刺伤害
    [Header("攻击3参数")]
    public float fallVelocity = -10f; // 攻击3的下降速度
    public int fallDamage = 10;  // 攻击3的下降伤害
    public int maxConsecutiveAttacks = 2; // 最大连续攻击次数
    public int attack3Counter = 0; // attack3的连续攻击计数器
    
    [Header("剑气参数")]
    public GameObject swordSlashPrefab; // 普通剑气预制体
    public GameObject specialSwordSlashPrefab; // 特殊攻击剑气预制体
    public Transform swordSlashSpawnPoint; // 剑气生成点
    public float slashSpeed = 8f; // 剑气的移动速度
    public float slashDistance = 2.5f; // 剑气的偏移距离
    public float generationDistance = 6f; // 生成剑气的间隔距离
    public float upOffset=3f;
    // 覆盖Awake方法，初始化RedMist自己的状态机
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        // 保存默认重力缩放值
        defaultGravityScale = rb.gravityScale;
        
        // 初始化状态，使用RedMist专用状态类
        idleState = new RedMistIdleState(this, stateMachine, "idle");

        attackState = new RedMistAttackState(this, stateMachine, "attack");
        deathState = new RedMistDeathState(this, stateMachine, "death");
        // 初始化新增状态
        disappearState = new RedMistDisappearState(this, stateMachine, "disappear");
        appearState = new RedMistAppearState(this, stateMachine, "appear");
        attack1State = new RedMistAttack1State(this, stateMachine, "attack1");
        attack2State = new RedMistAttack2State(this, stateMachine, "attack2");
        attack3State = new RedMistAttack3State(this, stateMachine, "attack3");
        attackDashState = new RedMistAttackDashState(this, stateMachine, "attackdash");
        specialAttackState = new RedMistSpecialAttackState(this, stateMachine, "specialattack");
    }
    
    // 覆盖Start方法，设置初始状态
    protected override void Start()
    {
        base.Start();
        // 初始化状态机，设置初始状态为idle
        stateMachine.Initialize(idleState);
        
        // 初始化计时器
        disappearTimer = disappearInterval;
        specialAttackTimer = specialAttackInterval;
        dashAttackDamageTimer = 0;

        
    }
    
    // 覆盖Update方法，更新状态机和各种计时器
    protected override void Update()
    {
        base.Update();
        
        // 更新特殊攻击计时器
        specialAttackTimer -= Time.deltaTime;
        // 更新攻击冲刺伤害计时器
        dashAttackDamageTimer -= Time.deltaTime;
        
        // 检查血量
        CheckHealth();
    }
    
    // 检查血量，更新isBelowHalfHealth标记
    private void CheckHealth()
    {
        if (stats != null)
        {
            isBelowHalfHealth = stats.currentHealth < stats.maxHealth.GetValue() / 2;
        }
    }
    
    // 保持水平速度
    public void MaintainHorizontalVelocity()
    {
        // 只修改水平速度，保持垂直速度不变
        rb.velocity = new Vector2(moveSpeed * faceDirection, rb.velocity.y);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("碰撞到玩家");
        }
    }
    
    // 重写IsPlayerDetected方法，使用playerDetectionRange作为检测范围
    public override RaycastHit2D IsPlayerDetected()
    {
        // 使用圆形检测来检测玩家，范围为playerDetectionRange
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, playerDetectionRange, whatIsPlayer);
        if (hitColliders.Length > 0)
        {
            // 返回第一个检测到的玩家碰撞体
            return Physics2D.Raycast(transform.position, hitColliders[0].transform.position - transform.position, playerDetectionRange, whatIsPlayer);
        }
        return new RaycastHit2D();
    }
    
    
    
    // 动画触发事件
    public void animTriggerEvent()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }


    //绘制射线 gizmos
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // 画玩家检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        
        // 绘制圆形攻击范围
        if (checkAttack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkAttack.position, checkAttackRange);
        }
        
        // 绘制矩形攻击范围
        if (checkAttackBox != null)
        {
            Gizmos.color = Color.green;
            // 保存当前的Gizmos矩阵
            Matrix4x4 originalMatrix = Gizmos.matrix;
            // 创建新的旋转矩阵
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(checkAttackBox.position, checkAttackBox.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;
            // 绘制旋转后的矩形
            Gizmos.DrawWireCube(Vector3.zero, checkAttackBoxSize);
            // 恢复原始矩阵
            Gizmos.matrix = originalMatrix;
        }
        
        // 绘制圆形攻击范围
        if (checkCircleAttack != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(checkCircleAttack.position, checkCircleAttackRadius);
        }
    }
}