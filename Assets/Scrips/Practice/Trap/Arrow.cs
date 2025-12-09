using UnityEngine;

/// <summary>
/// 箭脚本
/// 功能：处理箭的碰撞检测和销毁逻辑
/// </summary>
public class Arrow : MonoBehaviour
{
    #region 事件委托
    /// <summary>
    /// 箭被销毁时触发的事件
    /// </summary>
    public delegate void ArrowDestroyedEventHandler();
    public event ArrowDestroyedEventHandler OnArrowDestroyed;
    #endregion

    #region 组件引用
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D arrowCollider;
    #endregion

    #region 箭参数
    private bool hasHit = false;      // 箭是否已经命中目标
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 初始化组件引用
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (arrowCollider == null) arrowCollider = GetComponent<Collider2D>();

        // 确保箭的碰撞器是实体碰撞体
        if (arrowCollider != null)
        {
            arrowCollider.isTrigger = false;
        }
        
        // 设置箭的层级为trap
        // 确保在Unity编辑器中已经创建了名为"trap"的层级
        gameObject.layer = LayerMask.NameToLayer("trap");
        
        // 初始化旋转，确保箭头朝向速度方向
        UpdateArrowRotation();
    }

    private void Update()
    {
        // 如果箭已经命中，不再更新旋转
        if (hasHit) return;

        // 根据速度方向旋转箭
        UpdateArrowRotation();
    }
    
    /// <summary>
    /// 更新箭的旋转，确保箭头朝向速度方向
    /// 需求：箭旋转角度为0时，箭头指向下
    /// </summary>
    private void UpdateArrowRotation()
    {
        if (rb != null && rb.velocity != Vector2.zero)
        {
            // 计算速度方向的角度
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            
            // 调整角度：使速度方向向下时，箭的旋转角度为0度
            // 原来的计算中，向下(0,-1)对应-90度，所以需要加90度调整
            float adjustedAngle = angle + 90f;
            
            transform.rotation = Quaternion.AngleAxis(adjustedAngle, Vector3.forward);
        }
    }
    #endregion

    #region 碰撞检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        // 标记为已命中
        hasHit = true;

        // 检查是否命中玩家（通过标签识别）
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家被箭命中");
        }

        // 停止箭的运动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // 可以添加击中效果，比如粒子效果或音效
        // PlayHitEffect(collision.contacts[0].point);

        // 立即销毁箭
        DestroyArrow();
    }
    
    /// <summary>
    /// 销毁箭
    /// </summary>
    private void DestroyArrow()
    {
        // 触发箭被销毁的事件
        if (OnArrowDestroyed != null)
        {
            OnArrowDestroyed();
        }
        
        // 立即销毁箭
        Destroy(gameObject);
    }
    #endregion



    #region 辅助方法
    /// <summary>
    /// 播放击中效果
    /// </summary>
    private void PlayHitEffect(Vector2 hitPosition)
    {
        // 这里可以添加击中效果的实现
        // 例如：Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
    }
    
    /// <summary>
    /// 当对象被销毁时调用
    /// </summary>
    private void OnDestroy()
    {
        // 确保事件被触发
        if (!hasHit && OnArrowDestroyed != null)
        {
            OnArrowDestroyed();
        }
    }
    #endregion
}