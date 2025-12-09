using UnityEngine;

/// <summary>
/// 动态电锯陷阱脚本
/// 功能：检测与玩家碰撞并左右来回移动
/// </summary>
public class DynamicChainsaw : MonoBehaviour
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

    #region 移动参数
    [Header("移动参数")]
    [SerializeField] private float moveDistance = 2f;        // 左右移动的总距离
    [SerializeField] private float moveSpeed = 1f;           // 移动速度
    #endregion

    #region 私有变量
    private Vector2 startPosition;                           // 初始位置
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

        // 记录初始位置
        startPosition = transform.position;
    }

    private void Update()
    {
        // 实现左右来回移动
        float pingPongValue = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + Vector2.right * (pingPongValue - moveDistance / 2f);
    }

    #endregion

    #region 碰撞检测
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 检查是否碰撞到玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            // 碰撞检测逻辑保留，伤害功能将由用户自行实现
            Debug.Log("玩家与电锯碰撞！");
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

        // 绘制移动路径
        Gizmos.color = Color.blue;
        Vector2 pos = transform.position;
        Gizmos.DrawWireSphere(pos - Vector2.right * moveDistance / 2f, 0.1f);
        Gizmos.DrawWireSphere(pos + Vector2.right * moveDistance / 2f, 0.1f);
        Gizmos.DrawLine(pos - Vector2.right * moveDistance / 2f, pos + Vector2.right * moveDistance / 2f);
    }
    #endregion
}