using System;
using UnityEngine;

/// <summary>
/// 剑气脚本
/// 功能：处理剑气的碰撞检测和销毁逻辑
/// 特性：碰到玩家不消失，碰到墙才会消失
/// </summary>
public class SwordSlash : MonoBehaviour
{
    #region 事件委托
    /// <summary>
    /// 剑气被销毁时触发的事件
    /// </summary>
    public delegate void SwordSlashDestroyedEventHandler();
    public event SwordSlashDestroyedEventHandler OnSwordSlashDestroyed;
    #endregion

    #region 组件引用
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D slashCollider;
    #endregion

    #region 剑气参数
    private bool hasHitWall = false;      // 剑气是否已经碰到墙
    [SerializeField] private float damageAmount = 15f;  // 剑气造成的伤害
    [SerializeField] private float lifetime = 5f;  // 剑气的最大生命周期
    private float spawnTime;  // 剑气生成时间
    [SerializeField] private string wallTag = "Wall";  // 墙的标签
    [SerializeField] private float slashSpeed = 8f;  // 剑气的移动速度
    [SerializeField] private int slashDirection = -1;  // 剑气的移动方向，-1表示向左，1表示向右
    
    /// <summary>
    /// 设置剑气的伤害值（从外部调用）
    /// </summary>
    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }
    
    /// <summary>
    /// 设置剑气的速度
    /// </summary>
    public void SetVelocity(UnityEngine.Vector2 velocity)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
        }
    }
    
    /// <summary>
    /// 设置剑气的朝向并开始移动
    /// </summary>
    public void SetDirection(float direction)
    {
        // 确保rb引用已初始化
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (rb != null)
        {
            // 设置无视重力
            rb.gravityScale = 0;
            
            // 根据朝向设置速度
            UnityEngine.Vector2 velocity = new UnityEngine.Vector2(direction * slashSpeed, 0);
            rb.velocity = velocity;
            // 根据提供的事实：当rotation.z=0时，剑气朝向左
            // 根据方向设置旋转角度，确保朝向与移动方向一致
            if (direction < 0) // 向左移动
            {
                // 保持rotation.z=0，朝向左
                transform.rotation = Quaternion.Euler(0, 0, 0);
                slashDirection = -1;
            }
            else if (direction > 0) // 向右移动
            {
                // 设置rotation.z=180，朝向右
                transform.Rotate(0, 180, 0);
                slashDirection = 1;
            }
            
            // 移除精灵翻转，因为旋转已经能正确表示朝向
        }
        else
        {
            Debug.LogWarning("SwordSlash: Rigidbody2D component not found!");
        }
    }
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 初始化组件引用
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (slashCollider == null) slashCollider = GetComponent<Collider2D>();

        // 确保剑气的碰撞器是触发器
        if (slashCollider != null)
        {
            slashCollider.isTrigger = true;
            // 暂时禁用碰撞器，避免立即碰撞

            // 下一帧启用碰撞器
        }
        SetVelocity(new Vector2(slashSpeed*slashDirection,0));
        // 设置剑气的层级为trap
        gameObject.layer = LayerMask.NameToLayer("trap");
        
        // 设置剑气的绘制层级为Trap
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = "Trap";
        }
        
        // 设置无视重力
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
        
        // 记录生成时间
        spawnTime = Time.time;
    }

    private void Update()
    {
        // 如果剑气已经碰到墙，不再更新
        if (hasHitWall) return;
        
        // 检查是否超过最大生命周期
        if (Time.time - spawnTime > lifetime)
        {
            DestroySlash();
        }
    }
    
    /// <summary>
    /// 更新剑气的旋转，确保朝向速度方向
    /// </summary>
    private void UpdateSlashRotation()
    {
        if (rb != null && rb.velocity != Vector2.zero)
        {
            // 计算速度方向的角度
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    #endregion

    #region 碰撞检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查是否碰到墙
        if (collision.CompareTag(wallTag))
        {
            Debug.Log("剑气碰到墙");
            hasHitWall = true;
            DestroySlash();
            return;
        }
        
        // 检查是否命中玩家（通过标签识别）
        if (collision.CompareTag("Player"))
        {
            Debug.Log("玩家被剑气命中");
            // 获取玩家的PlayerStats组件并造成伤害
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(Mathf.RoundToInt(damageAmount));
                Debug.Log("剑气对玩家造成了 " + Mathf.RoundToInt(damageAmount) + " 点伤害");
            }
            // 玩家被命中后，剑气不消失，继续飞行
        }
    }
    
    /// <summary>
    /// 销毁剑气
    /// </summary>
    private void DestroySlash()
    {
        // 触发剑气被销毁的事件
        if (OnSwordSlashDestroyed != null)
        {
            OnSwordSlashDestroyed();
        }
        
        // 销毁剑气
        Destroy(gameObject);
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 当对象被销毁时调用
    /// </summary>
    private void OnDestroy()
    {
        // 确保事件被触发
        if (!hasHitWall && OnSwordSlashDestroyed != null)
        {
            OnSwordSlashDestroyed();
        }
    }
    #endregion
}