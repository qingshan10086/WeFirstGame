using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Enemy_town : Enemy
{
    public System.Action onFlipped;//血条不翻转委托
    private GameObject player;
    private PlayerStats playerStats;

    #region Stats
    [Header("移动属性")]
    [SerializeField] private float horizontalSpeed = 2f;      //水平移动速度
    [SerializeField] private float fixedJumpSpeed = 5f;        //固定跳跃速度
    [SerializeField] private float playerDetectionRange = 3f;  //玩家检测范围
    #endregion

    [Header("攻击参数")]
    [SerializeField] protected Transform checkAttack;         //获取攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float checkAttackRange;        //攻击检测的范围
    [SerializeField] protected float attackDamage = 10f;      //攻击伤害值

    [Header("Debug")]
    public bool showDebugLogs = false;                       //是否显示调试日志
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        if(playerStats == null)
        {
            Debug.LogError("Enemy_town: 未找到玩家统计组件");
        }
        if (showDebugLogs)
        {
            Debug.Log("Enemy_town: Stats initialized");
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        // 不调用base.Update()，避免Enemy类中的状态机空引用错误
        // 墙壁检测，碰到墙回头
        if (IsWallDetected())
        {
            Flip();
        }
        
        // 地面检测，一落地就跳跃
        if(IsGroundDetected())
        {
            Jump();
        }
        
        // 保持水平速度
        MaintainHorizontalVelocity();
    }
    
    // 跳跃逻辑
    private void Jump()
    {
        float jumpSpeed = fixedJumpSpeed;
        
        // 如果玩家在检测范围内，计算刚好砸到玩家的跳跃速度
        if (IsPlayerClose())
        {
            jumpSpeed = CalculateJumpSpeedToPlayer();
        }
        
        // 应用跳跃速度
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }
    
    // 保持水平速度
    private void MaintainHorizontalVelocity()
    {
        // 只修改水平速度，保持垂直速度不变
        rb.velocity = new Vector2(horizontalSpeed * faceDirection, rb.velocity.y);
    }
    
    // 检测玩家是否在近距离范围内
    private bool IsPlayerClose()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return false;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= playerDetectionRange;
    }
    
    // 计算刚好砸到玩家的跳跃速度
    private float CalculateJumpSpeedToPlayer()
    {
        if (player == null) return fixedJumpSpeed;
        
        // 获取玩家和敌人的位置
        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.transform.position;
        
        // 计算水平距离
        float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);
        
        // 使用物理公式计算所需的跳跃速度
        // 假设敌人保持水平速度移动，计算到达玩家位置所需的时间
        float timeToReachPlayer = horizontalDistance / horizontalSpeed;
        
        // 计算垂直方向需要的速度，使得敌人在timeToReachPlayer时间后到达玩家高度
        // 使用公式：y = v0 * t - 0.5 * g * t^2
        // 假设敌人和玩家在同一高度（或忽略高度差）
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float requiredJumpSpeed = 0.5f * gravity * timeToReachPlayer;
        
        // 确保跳跃速度至少为固定跳跃速度
        return Mathf.Max(requiredJumpSpeed, fixedJumpSpeed);
    }

    // 攻击玩家的方法会在更新后的OnTriggerEnter2D中调用
    
    // 攻击玩家
    private void AttackPlayer()
    {
        if (playerStats != null)
        {
            Debug.Log("Enemy_town: 攻击玩家");
            // 计算伤害
            int totalDamage = Mathf.RoundToInt(attackDamage);
            
            // 对玩家造成伤害
            playerStats.TakeDamage(totalDamage);
            
            if (showDebugLogs)
            {
                Debug.Log("Enemy_town: 对玩家造成了 " + totalDamage + " 点伤害");
            }
        }
    }
    
    // 攻击效果
    public override void DamageEffect()
    {
        if (showDebugLogs)
        {
            Debug.Log("Enemy_town: 受到伤害效果");
        }
        
        // 播放受击光效
        if (fx != null)
        {
            fx.StartCoroutine("FlashFX");
        }
        
        // 开始击退
        StartCoroutine(HitKnockback());
    }
    
    // 击退协程
    protected override IEnumerator HitKnockback()
    {
        isKnocked = true;
        
        if (rb != null)
        {
            rb.velocity = new Vector2(knockbackDirection.x * -faceDirection, knockbackDirection.y);
        }
        
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
    }

    // 绘制攻击范围
    protected override void OnDrawGizmos()
    {
        // 不调用base.OnDrawGizmos()，避免attackCheck空引用错误
        
        // 绘制攻击范围
        if (checkAttack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkAttack.position, checkAttackRange);
        }
    }


   

    public override void Die()
    {
        if (showDebugLogs)
        {
            Debug.Log("Enemy_town: 死亡");
        }
        
        // 停止移动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        // 禁用碰撞体
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        
        // 可以添加死亡动画或效果
        // if (anim != null)
        // {
        //     anim.SetBool("dead", true);
        // }
        
        // 延迟后销毁游戏对象
        StartCoroutine(DestroyAfterDelay(0.5f));
    }
    
    // 延迟销毁
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    
    // 检测玩家的攻击触发器
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Enemy_town: 触发了碰撞");
        // 检查是否是玩家的攻击触发器
        if (collider.CompareTag("Player") && collider.name.Contains("Attack") || collider.name.Contains("attack"))
        {
            Debug.Log("找到了玩家");
            if (showDebugLogs)
            {
                Debug.Log("Enemy_town: 被玩家攻击");
            }
            
            // 获取玩家的PlayerStats
            PlayerStats playerStats = collider.transform.root.GetComponent<PlayerStats>();
            
            if (playerStats != null)
            {
                // 让玩家对敌人造成伤害
                playerStats.DoDamage(stats);
            }
        }
        // 检测玩家本体碰撞（用于敌人攻击玩家）
        else if (collider.CompareTag("Player") && !collider.name.Contains("Attack") && !collider.name.Contains("attack"))
        {
            playerStats = collider.transform.root.GetComponent<PlayerStats>();
            Debug.Log("Enemy_town: 碰撞到玩家，准备攻击");
            if (showDebugLogs)
            {
                Debug.Log("Enemy_town: 碰撞到玩家，准备攻击");
            }
            AttackPlayer();
        }
    }
}



