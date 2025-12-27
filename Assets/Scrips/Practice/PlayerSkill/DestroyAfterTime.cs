using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 延迟销毁与碰撞检测脚本
/// 挂载到物体上，物体将在指定时间后自动销毁
/// 同时检测碰撞，碰到符合条件的物体时触发事件
/// </summary>
public class DestroyAfterTime : MonoBehaviour
{
    [Header("销毁设置")]
    [Tooltip("延迟销毁时间（秒）")]
    public float destroyDelay = 1.0f;
    
    [Header("碰撞设置")]
    [Tooltip("是否启用碰撞检测")]
    public bool enableCollisionDetection = true;
    
    [Tooltip("碰撞检测模式：触发器或碰撞体")]
    public CollisionDetectionMode detectionMode = CollisionDetectionMode.Trigger;
    
    [Tooltip("检测的标签（留空则检测所有物体）")]
    public string targetTag = "Enemy"; // 默认为Enemy标签
    
    [Tooltip("检测的层（留空则检测所有层）")]
    public LayerMask targetLayer = ~0; // 默认为检测所有层
    
    [Header("事件")]
    [Tooltip("碰撞到目标物体时触发的事件")]
    public UnityEvent<Collider2D> OnCollisionDetected = new UnityEvent<Collider2D>();
    [Header("伤害处理")]
    public int damageAmount = 10;
    [Header("与能量系统")]
    public PlayerEnergySystem playerEnergySystem;
    public float addEnergyAmount = 0.1f;
    // 碰撞检测模式枚举
    public enum CollisionDetectionMode
    {
        Trigger,  // 使用OnTriggerEnter2D
        Collision // 使用OnCollisionEnter2D
    }
    
    private void Start()
    {
        // 启动协程，延迟指定时间后销毁物体
        StartCoroutine(DestroyObjectWithDelay());
        //订阅事件
        OnCollisionDetected.AddListener(colliderEvent);
        
        // 确保物体有合适的碰撞组件
        EnsureCollisionComponent();
    }
    
    /// <summary>
    /// 确保物体有合适的碰撞组件
    /// </summary>
    private void EnsureCollisionComponent()
    {
        if (!enableCollisionDetection) return;
        
        // 检查是否有Collider2D组件
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning($"{gameObject.name} has no Collider2D component. Adding a BoxCollider2D.");
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // 设置碰撞组件类型
        if (detectionMode == CollisionDetectionMode.Trigger)
        {
            collider.isTrigger = true;
        }
        else
        {
            collider.isTrigger = false;
            // 确保有Rigidbody2D组件
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic; // 避免物理影响
            }
        }
    }
    
    /// <summary>
    /// 延迟销毁物体的协程
    /// </summary>
    private System.Collections.IEnumerator DestroyObjectWithDelay()
    {
        // 等待指定的延迟时间
        yield return new WaitForSeconds(destroyDelay);
        
        // 销毁当前挂载脚本的物体
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 触发器碰撞检测
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enableCollisionDetection || detectionMode != CollisionDetectionMode.Trigger)
            return;
        
        HandleCollision(other);
    }
    
    /// <summary>
    /// 碰撞体碰撞检测
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enableCollisionDetection || detectionMode != CollisionDetectionMode.Collision)
            return;
        
        HandleCollision(collision.collider);
    }
    
    /// <summary>
    /// 处理碰撞事件
    /// </summary>
    private void HandleCollision(Collider2D other)
    {
        // 检查标签
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;
        
        // 检查层
        if (((1 << other.gameObject.layer) & targetLayer) == 0)
            return;
        
        // 触发碰撞事件
        OnCollisionDetected.Invoke(other);
    }

    private void colliderEvent(Collider2D other)
    {
        EnemyStats enemyStats = other.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damageAmount);
            if (playerEnergySystem != null)
            {
                playerEnergySystem.AddEnergy(addEnergyAmount);
            }
        }
    }
    private void OnDestroy()
    {
        // 移除所有事件监听
        OnCollisionDetected.RemoveAllListeners();
    }
}