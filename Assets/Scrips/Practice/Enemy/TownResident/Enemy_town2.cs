using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Enemy_town2 : Enemy
{
    
    public System.Action onFlipped;//血条不翻转委托
    private GameObject player;

    [Header("移动参数")]
    [SerializeField] private float horizontalSpeed = 2f;      //水平移动速度
    
    [Header("检查参数")]
    [SerializeField] protected Transform playerCheck;         //获取玩家检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float playerCheckDistance;     //玩家检测的距离
    [SerializeField] public float attackRange;             //攻击范围
    [Header("攻击参数")]
    [SerializeField] public Transform checkAttack;         //获取攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] public float checkAttackRange;        //攻击检测的范围  
    [SerializeField] public float attackDamage = 15f;      //攻击伤害值

   



    private PlayerStats playerStats;                         //玩家属性

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
        
        if (showDebugLogs)
        {
            Debug.Log("Enemy_town2: Stats initialized");
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        // 墙壁检测，碰到墙回头
        if (IsWallDetected())
        {
            Flip();
        }
        
        // 检测玩家
        GameObject detectedPlayer = DetectPlayer();
        
        // 根据检测结果调整移动方向
        if (detectedPlayer != null)
        {
            Debug.Log("Enemy_town2: 检测到玩家");
            // 检测到玩家，靠近玩家行走
            float playerDirection = detectedPlayer.transform.position.x - transform.position.x > 0 ? 1 : -1;
            
            // 确保敌人面向玩家
            if (playerDirection > 0 && !faceRight)
            {
                Flip();
            }
            else if (playerDirection < 0 && faceRight)
            {
                Flip();
            }
            
            // 如果玩家在攻击范围内，准备攻击
            if (Mathf.Abs(detectedPlayer.transform.position.x - transform.position.x) <= attackRange)
            {
                // 玩家在攻击范围内，这里可以调用攻击逻辑
                ZeroVelocity();
                anim.SetBool("attack", true);

            }
            else
            {
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && 
    !anim.IsInTransition(0))
                // 玩家不在攻击范围内，靠近玩家
                rb.velocity = new Vector2(horizontalSpeed * playerDirection, rb.velocity.y);
                anim.SetBool("attack", false);
            }
        }
        else
        {
            // 未检测到玩家，正常行走
            rb.velocity = new Vector2(horizontalSpeed * faceDirection, rb.velocity.y);
        }
    }
    


    // 攻击玩家的方法会在更新后的OnTriggerEnter2D中调用
    
    // 攻击玩家
    private void AttackPlayer()
    {
        if (playerStats != null)
        {
            // 计算伤害
            int totalDamage = Mathf.RoundToInt(attackDamage);
            
            // 对玩家造成伤害
            playerStats.TakeDamage(totalDamage);
            
            if (showDebugLogs)
            {
                Debug.Log("Enemy_town2: 对玩家造成了 " + totalDamage + " 点伤害");
            }
        }
    }
    
    // 攻击效果
    public override void DamageEffect()
    {
        base.DamageEffect();
        if (showDebugLogs)
        {
            Debug.Log("Enemy_town2: 受到伤害效果");
        }
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



     #region   Collider 

    
    // 检测玩家的射线方法
    public GameObject DetectPlayer()
    {
        // 向当前面对方向发射射线检测玩家
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * faceDirection, playerCheckDistance, whatIsPlayer);
        
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log("Enemy_town2: 检测到玩家");
            return hit.collider.gameObject;
        }
        
        return null;
    }
    
    protected override void OnDrawGizmos()
    {
        // 不调用base.OnDrawGizmos()，避免attackCheck空引用错误
        
        // 画玩家检测射线
        if (playerCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance * faceDirection, playerCheck.position.y));
        }
        
        // 画攻击范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //画攻击范围
        if (checkAttack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkAttack.position, checkAttackRange);
        }
    }
    #endregion






   

   
   

    public override void Die()
    {
        base.Die();
        if (showDebugLogs)
        {
            Debug.Log("Enemy_town2: 死亡");
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
        
        // 停止所有动画
        if (anim != null)
        {
            anim.enabled = false;
        }
        
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
        // 检查是否是玩家的攻击触发器
        if (collider.CompareTag("Player") )
        {
            if (showDebugLogs)
            {
                Debug.Log("Enemy_town2: 被玩家攻击");
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
        else if (collider.CompareTag("Player") )
        {
            if (showDebugLogs)
            {
                Debug.Log("Enemy_town2: 碰撞到玩家，准备攻击");
            }
            AttackPlayer();
        }
    }
}
