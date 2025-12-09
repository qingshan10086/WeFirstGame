using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家健康系统
/// 功能：管理玩家生命值，处理伤害，更新UI
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    #region 健康参数
    [Header("健康参数")]
    [SerializeField] private float maxHealth = 100f;        // 最大生命值
    [SerializeField] private float currentHealth;           // 当前生命值
    #endregion

    #region 伤害效果
    [Header("伤害效果")]
    [SerializeField] private Color damageFlashColor = new Color(1f, 0f, 0f, 0.5f);  // 受伤闪烁颜色
    [SerializeField] private float flashDuration = 0.2f;    // 闪烁持续时间
    #endregion

    #region UI引用
    [Header("UI引用")]
    [SerializeField] private Image healthBar;               // 血条UI
    [SerializeField] private Text healthText;               // 血量文本
    #endregion

    #region 内部变量
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float flashTimer = 0f;
    private bool isDead = false;
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 初始化组件引用
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // 初始化生命值
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        // 更新受伤闪烁效果
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                spriteRenderer.color = originalColor;
            }
        }
        
        // 死亡后处理
        if (isDead)
        {
            // 可以在这里添加死亡动画或游戏结束逻辑
        }
    }
    #endregion

    #region 健康管理
    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="damageAmount">伤害值</param>
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        
        // 减少生命值
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        // 更新UI
        UpdateHealthUI();
        
        // 播放受伤效果
        PlayDamageEffect();
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 玩家恢复生命值
    /// </summary>
    /// <param name="healAmount">恢复值</param>
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        UpdateHealthUI();
        
        // 可以添加治疗效果
    }
    
    /// <summary>
    /// 玩家死亡
    /// </summary>
    private void Die()
    {
        isDead = true;
        Debug.Log("玩家死亡！");
        
        // 可以在这里添加死亡动画、音效和游戏结束逻辑
        // 例如：禁用玩家控制、显示死亡UI等
    }
    #endregion

    #region 效果和UI
    /// <summary>
    /// 播放受伤效果
    /// </summary>
    private void PlayDamageEffect()
    {
        // 闪烁效果
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageFlashColor;
            flashTimer = flashDuration;
        }
        
        // 可以添加音效
        // if (hitSound != null)
        // {
        //     AudioSource.PlayClipAtPoint(hitSound, transform.position);
        // }
        
        // 可以添加粒子效果
        // if (hitParticles != null)
        // {
        //     Instantiate(hitParticles, transform.position, Quaternion.identity);
        // }
    }
    
    /// <summary>
    /// 更新健康UI
    /// </summary>
    private void UpdateHealthUI()
    {
        // 更新血条
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
        
        // 更新文本
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Floor(currentHealth)}/{maxHealth}";
        }
    }
    #endregion
}