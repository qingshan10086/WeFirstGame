using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    private Enemy_nothing enemy;
    public AudioClip Hello;
    public AudioClip Goodbye;
    
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_nothing>();
    }
    private void AnimationTrigger()
    {
        enemy.animTriggerEvent();
    }
    private void EndAttack()
    {
        enemy.anim.SetBool("attack", false);
    }

    private void AttackTriggerone()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // 计算伤害值
                int intDamage = Mathf.RoundToInt(enemy.attackDamage);
                
                // 获取玩家的PlayerStats组件
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // 使用PlayerStats组件造成伤害
                    playerStats.TakeDamage(intDamage);
                    
                    // 播放攻击光效
                    if (enemy.fx != null)
                    {
                        enemy.fx.StartCoroutine("FlashFX");
                    }
                    
                    UnityEngine.Debug.Log("1攻击到玩家，造成了 " + intDamage + " 点伤害");
                }
            }
        }
    }
    private void AttackTriggerTwo()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.checkAttackSecond.position, enemy.checkAttackRangeSecond);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // 计算伤害值
                int intDamage = Mathf.RoundToInt(enemy.attackDamage);
                
                // 获取玩家的PlayerStats组件
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // 使用PlayerStats组件造成伤害
                    playerStats.TakeDamage(intDamage);
                    
                    // 播放攻击光效
                    if (enemy.fx != null)
                    {
                        enemy.fx.StartCoroutine("FlashFX");
                    }
                    
                    UnityEngine.Debug.Log("2攻击到玩家，造成了 " + intDamage + " 点伤害");
                }
            }
        }
    }
    private void AttackTriggerThree()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.checkAttackThird.position, enemy.checkAttackRangeThird);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // 计算伤害值
                int intDamage = Mathf.RoundToInt(enemy.attackDamage);
                
                // 获取玩家的PlayerStats组件
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // 使用PlayerStats组件造成伤害
                    playerStats.TakeDamage(intDamage);
                    
                    // 播放攻击光效
                    if (enemy.fx != null)
                    {
                        enemy.fx.StartCoroutine("FlashFX");
                    }
                    BloodMusicManager.Instance.PreloadAudioClips(Goodbye);
                    BloodMusicManager.Instance.PlaySoundEffect(Goodbye);
                    UnityEngine.Debug.Log("3攻击到玩家，造成了 " + intDamage + " 点伤害");
                }
            }
        }
    }
    
    // 动画事件函数 - 发射箭
    public void FireArrow()
    {
        if (enemy != null)
        {
            Debug.Log("FireArrow函数被调用");
            
            if (enemy.arrowPrefab != null)
            {
                Debug.Log("arrowPrefab已赋值");
                
                // 计算箭的生成位置：敌人前方
                Vector3 spawnPosition = enemy.transform.position;
                Debug.Log("敌人位置: " + enemy.transform.position);
                Debug.Log("敌人faceDirection: " + enemy.faceDirection);
                spawnPosition.x += enemy.faceDirection * 0.5f; // 稍微偏移，避免从敌人身体中间发射
                Debug.Log("箭生成位置: " + spawnPosition);
                
                // 实例化箭预制体
                GameObject arrow = Object.Instantiate(enemy.arrowPrefab, spawnPosition, Quaternion.identity);
                BloodMusicManager.Instance.PreloadAudioClips(Hello);
                BloodMusicManager.Instance.PlaySoundEffect(Hello);
                if (arrow != null)
                {
                    Debug.Log("箭实例化成功，名称: " + arrow.name);
                    
                    // 检查并设置箭的颜色为红色，便于调试，同时设置绘制层级为Trap
                    SpriteRenderer arrowSprite = arrow.GetComponentInChildren<SpriteRenderer>();
                    if (arrowSprite != null)
                    {
                        arrowSprite.color = Color.red;
                        // 设置箭的绘制层级为Trap
                        arrowSprite.sortingLayerName = "Trap";
                        Debug.Log("箭的SpriteRenderer组件存在，已设置为红色和Trap层级");
                    }
                    else
                    {
                        Debug.LogWarning("箭预制体没有SpriteRenderer组件");
                    }
                    
                    // 获取箭的Rigidbody2D组件
                    Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
                    if (arrowRb != null)
                    {
                        arrowRb.gravityScale = 0f;
                        Debug.Log("箭的Rigidbody2D组件存在");
                        // 设置箭的速度，朝向敌人的面朝方向
                        float arrowVelocityX = enemy.arrowSpeed * enemy.faceDirection;
                        arrowRb.velocity = new Vector2(arrowVelocityX, 0f);
                        Debug.Log("箭的速度已设置: " + arrowRb.velocity);
                    }
                    else
                    {
                        Debug.LogWarning("箭预制体没有Rigidbody2D组件");
                    }
                    
                    // 翻转箭的朝向，使其与飞行方向一致
                    if (enemy.faceDirection == -1)
                    {
                        Vector3 localScale = arrow.transform.localScale;
                        localScale.x = -Mathf.Abs(localScale.x);
                        arrow.transform.localScale = localScale;
                        Debug.Log("箭的朝向已翻转");
                    }
                }
                else
                {
                    Debug.LogError("箭实例化失败");
                }
                
                Debug.Log("发射了一只箭");
            }
            else
            {
                Debug.LogWarning("Arrow prefab is not assigned");
            }
        }
        else
        {
            Debug.LogError("enemy引用为空");
        }
    }
}