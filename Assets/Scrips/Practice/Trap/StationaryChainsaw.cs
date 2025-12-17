using UnityEngine;

/// <summary>
/// 静止电锯陷阱脚本
/// 功能：检测与玩家碰撞并造成伤害
/// </summary>
public class StationaryChainsaw : MonoBehaviour
{
    #region 组件引用
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D circleCollider;
    #endregion

    #region 电锯参数
    [Header("电锯参数")]
    [SerializeField] private float damageAmount = 15f;       // 每次攻击造成的伤害
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 初始化组件引用
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (circleCollider == null) circleCollider = GetComponent<CircleCollider2D>();
        
        // 设置刚体为静态
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
        }
        
        // 设置碰撞器为实体碰撞体（非触发器）
        if (circleCollider != null)
        {
            circleCollider.isTrigger = false;
        }
    }


    #endregion

    #region 碰撞检测
    private float damageTimer = 0f;
    [SerializeField] private float damageInterval = 0.5f; // 伤害间隔时间
    private bool isFirstCollision = true; // 是否是第一次碰撞

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 检查是否碰撞到玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家与电锯碰撞！");
            
            // 第一次碰撞时立即造成伤害
            if (isFirstCollision)
            {
                isFirstCollision = false;
                damageTimer = 0f;
                // 获取玩家的PlayerStats组件并造成伤害
                PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(Mathf.RoundToInt(damageAmount));
                    Debug.Log("静止电锯对玩家造成了 " + Mathf.RoundToInt(damageAmount) + " 点伤害");
                }
            }
            else
            {
                // 计时伤害
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    damageTimer = 0f;
                    // 获取玩家的PlayerStats组件并造成伤害
                    PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(Mathf.RoundToInt(damageAmount));
                        Debug.Log("静止电锯对玩家造成了 " + Mathf.RoundToInt(damageAmount) + " 点伤害");
                    }
                }
            }
        }
    }
    
    // 当玩家离开碰撞范围时，重置第一次碰撞标志
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isFirstCollision = true;
            damageTimer = 0f;
        }
    }
    #endregion

    #region 编辑器辅助
    private void OnDrawGizmosSelected()
    {
        // 绘制电锯旋转范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
        
        // 绘制攻击范围提示
        Gizmos.color = Color.yellow;
        if (circleCollider != null)
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
    }
    #endregion
}