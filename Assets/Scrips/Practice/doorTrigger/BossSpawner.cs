
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Boss生成器
/// 功能：在所有怪物波次完成后生成Boss，并检测Boss是否被击败
/// </summary>
public class BossSpawner : MonoBehaviour
{
    [Header("Boss设置")]
    [SerializeField] private GameObject bossPrefab; // Boss预制体
    [SerializeField] private Transform bossSpawnPosition; // Boss生成位置
    [SerializeField] private bool useCustomSpawnPosition = false; // 是否使用自定义生成位置
    [SerializeField] private Vector2 customSpawnOffset = Vector2.zero; // 自定义生成位置偏移量
    
    [Header("事件源")]
    [SerializeField] private WaveMonsterSpawner waveSpawner; // 波次怪物生成器
    
    [Header("视觉效果")]
    [SerializeField] private GameObject spawnEffectPrefab; // 生成特效预制体
    [SerializeField] private AudioClip spawnSound; // 生成音效
    [SerializeField] private AudioSource audioSource; // 音频源
    [SerializeField] private Color spawnRangeColor = new Color(0.5f, 0, 1, 0.3f); // 生成范围颜色
    
    [Header("Boss死亡效果")]
    [SerializeField] private GameObject deathEffectPrefab; // 死亡特效预制体
    [SerializeField] private AudioClip deathSound; // 死亡音效
    
    [Header("音乐设置")]
    [SerializeField] private AudioClip postBossMusic; // Boss战结束后播放的音乐
    [SerializeField] private bool usePostBossMusic = true; // 是否使用Boss战后音乐
    [SerializeField] private float musicFadeDuration = 1f; // 音乐淡入淡出持续时间
    
    [Header("调试信息")]
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    // 事件系统
    public System.Action OnBossSpawned; // Boss生成事件
    public System.Action OnBossDefeated; // Boss被击败事件
    
    private bool hasSpawnedBoss = false; // 是否已经生成Boss
    private bool hasDefeatedBoss = false; // 是否已经击败Boss
    private GameObject spawnedBoss = null; // 生成的Boss实例
    private EnemyStats bossStats = null; // Boss的生命值组件
    [Header("Boss生命血条")]
    public GameObject bossHealthBar;

    #region 生命周期方法
    private void Start()
    {
        // 验证必要的组件
        if (waveSpawner == null)
        {
            Debug.LogError("BossSpawner: WaveMonsterSpawner is not assigned!");
            return;
        }
        
        if (bossPrefab == null)
        {
            Debug.LogError("BossSpawner: BossPrefab is not assigned!");
            return;
        }
        
        // 订阅所有波次完成事件
        waveSpawner.OnAllWavesCompleted += HandleAllWavesCompleted;
        OnBossDefeated += BossDeath;
        if (showDebugInfo)
        {
            Debug.Log("BossSpawner initialized and waiting for all waves to complete.");
        }
    }
    
    private void Update()
    {
        // 检测Boss是否已经死亡
        if (hasSpawnedBoss && !hasDefeatedBoss &&spawnedBoss != null)
        {
            if (bossStats != null)
            {
                // 使用EnemyStats组件检测Boss是否死亡
                if (bossStats.currentHealth <= 0)
                {
                    HandleBossDeath();
                }
            }
            // else
            // {
            //     // 如果没有EnemyStats组件，检查Boss是否被销毁
            //     if (spawnedBoss == null || spawnedBoss.Equals(null))
            //     {
            //         HandleBossDeath();
            //     }
            // }
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件（避免内存泄漏）
        if (waveSpawner != null)
        {
            waveSpawner.OnAllWavesCompleted -= HandleAllWavesCompleted;
        }
    }
    #endregion
    
    #region 核心功能
    /// <summary>
    /// 处理所有波次完成事件
    /// </summary>
    private void HandleAllWavesCompleted()
    {
        if (hasSpawnedBoss)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("BossSpawner: Boss has already been spawned!");
            }
            return;
        }
        
        SpawnBoss();
    }

    /// <summary>
    /// 从事件生成Boss的公共方法
    /// 功能：允许从外部（如TriggerEventManager）触发Boss生成
    /// </summary>
    public void SpawnBossFromEvent()
    {
        if (hasSpawnedBoss)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("BossSpawner: Boss has already been spawned!");
            }
            return;
        }
        
        SpawnBoss();
    }
    
    /// <summary>
    /// 生成Boss
    /// </summary>
    private void SpawnBoss()
    {
        // 计算Boss生成位置
        Vector3 spawnPosition = CalculateSpawnPosition();
        
        // 播放生成特效
        PlaySpawnEffect(spawnPosition);
        
        // 播放生成音效
        PlaySpawnSound();
        
        // 生成Boss
        spawnedBoss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        //设置Boss血条
        if(bossHealthBar!=null)
        {
            Debug.Log("BossSpawner: Boss health bar activated.");
            bossHealthBar.SetActive(true);
            bossHealthBar.GetComponent<PlayerHealthBar_UI>().entity = spawnedBoss.GetComponent<Enemy>();
            bossHealthBar.GetComponent<PlayerHealthBar_UI>().myStats = spawnedBoss.GetComponent<EnemyStats>();
        }

        // 设置Boss的标签和绘制层级
        if (spawnedBoss != null)
        {
            // 强制设置Z轴位置为0（2D游戏中关键）
            Vector3 finalPosition = spawnedBoss.transform.position;
            finalPosition.z = 0f;
            spawnedBoss.transform.position = finalPosition;
            
            spawnedBoss.tag = "Enemy";
            
            // 设置Boss所在Layer为Default（确保相机可见）
            // spawnedBoss.layer = 0; // 0是Default层
            
            // 设置Boss绘制层级为Enemy
            SpriteRenderer spriteRenderer = spawnedBoss.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // 确保渲染器启用
                spriteRenderer.enabled = true;
                
                // 设置绘制层级
                spriteRenderer.sortingLayerName = "Enemy";
                spriteRenderer.sortingOrder = 0;
                
                // 确保Sprite不为空
                if (spriteRenderer.sprite == null)
                {
                    Debug.LogWarning("BossSpawner: SpriteRenderer has no sprite assigned!");
                }
                
                if (showDebugInfo)
                {
                    Debug.Log($"BossSpawner: Main SpriteRenderer enabled: {spriteRenderer.enabled}");
                    Debug.Log($"BossSpawner: Main SpriteRenderer sorting layer: {spriteRenderer.sortingLayerName}");
                    Debug.Log($"BossSpawner: Main SpriteRenderer sorting order: {spriteRenderer.sortingOrder}");
                }
            }
            else
            {
                Debug.LogWarning("BossSpawner: Main SpriteRenderer not found!");
            }
            
            // 检查并设置子对象的绘制层级
            SpriteRenderer[] childRenderers = spawnedBoss.GetComponentsInChildren<SpriteRenderer>();
            if (childRenderers.Length > 0)
            {
                foreach (SpriteRenderer childRenderer in childRenderers)
                {
                    childRenderer.enabled = true;
                    childRenderer.sortingLayerName = "Enemy";
                    childRenderer.sortingOrder = 0;
                }
                
                if (showDebugInfo)
                {
                    Debug.Log($"BossSpawner: Set {childRenderers.Length} child SpriteRenderers to Enemy layer");
                }
            }
            
            // 检查相机可见性
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(spawnedBoss.transform.position);
                bool isInView = screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height;
                
                if (showDebugInfo)
                {
                    Debug.Log($"BossSpawner: Boss world position: {spawnedBoss.transform.position}");
                    Debug.Log($"BossSpawner: Boss screen position: {screenPos}");
                    Debug.Log($"BossSpawner: Boss in camera view: {isInView}");
                    Debug.Log($"BossSpawner: Boss layer: {LayerMask.LayerToName(spawnedBoss.layer)} ({spawnedBoss.layer})");
                    Debug.Log($"BossSpawner: Camera culling mask includes Boss layer: {((1 << spawnedBoss.layer) & mainCamera.cullingMask) != 0}");
                }
            }
            
            // 获取Boss的生命值组件
            bossStats = spawnedBoss.GetComponent<EnemyStats>();
            if (bossStats == null)
            {
                Debug.LogWarning("BossSpawner: EnemyStats component not found on Boss!");
            }
            bossStats.currentHealth = 400;
            Debug.Log($"BossSpawner: Boss health: {bossStats.currentHealth}");
            hasSpawnedBoss = true;
            hasDefeatedBoss = false;
            
            // 触发Boss生成事件
            OnBossSpawned?.Invoke();
            
            if (showDebugInfo)
            {
                Debug.Log("BossSpawner: Boss has been successfully spawned!");
            }
        }
        else
        {
            Debug.LogError("BossSpawner: Failed to spawn Boss!");
        }
    }
    
    /// <summary>
    /// 计算Boss生成位置
    /// </summary>
    /// <returns>Boss生成位置</returns>
    private Vector3 CalculateSpawnPosition()
    {
        if (useCustomSpawnPosition && bossSpawnPosition != null)
        {
            // 使用自定义生成位置
            return bossSpawnPosition.position + new Vector3(customSpawnOffset.x, customSpawnOffset.y, 0);
        }
        else if (bossSpawnPosition != null)
        {
            // 使用默认生成位置
            return bossSpawnPosition.position;
        }
        else
        {
            // 如果没有指定生成位置，则使用当前对象的位置
            return transform.position + new Vector3(customSpawnOffset.x, customSpawnOffset.y, 0);
        }
    }
    
    /// <summary>
    /// 播放生成特效
    /// </summary>
    /// <param name="position">特效位置</param>
    private void PlaySpawnEffect(Vector3 position)
    {
        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, position, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// 播放生成音效
    /// </summary>
    private void PlaySpawnSound()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
    
    /// <summary>
    /// 处理Boss死亡
    /// </summary>
    private void HandleBossDeath()
    {
        if (hasDefeatedBoss)
        {
            return; // 避免重复处理
        }
        hasDefeatedBoss = true;
 
        // 播放死亡特效
        if (spawnedBoss != null && deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, spawnedBoss.transform.position, Quaternion.identity);
        }
        
        // 播放死亡音效
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        // 播放Boss战后音乐
        if (usePostBossMusic && postBossMusic != null)
        {
            BloodMusicManager musicManager = BloodMusicManager.Instance;
            if (musicManager != null)
            {
                // 使用淡入效果播放Boss战后音乐
                musicManager.PlayBackgroundMusic(postBossMusic, true);
                Debug.Log("BossSpawner: Playing post-boss music after boss defeat");
            }
        }
        
        // 触发Boss被击败事件
        OnBossDefeated?.Invoke();
        
        if (showDebugInfo)
        {
            Debug.Log("BossSpawner: Boss has been defeated!");
        }
    }
    
    /// <summary>
    /// 重置Boss生成器
    /// </summary>
    public void ResetSpawner()
    {
        hasSpawnedBoss = false;
        hasDefeatedBoss = false;
        
        if (spawnedBoss != null)
        {
            Destroy(spawnedBoss);
            spawnedBoss = null;
        }
        
        bossStats = null;
        
        if (showDebugInfo)
        {
            Debug.Log("BossSpawner has been reset.");
        }
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制Boss生成位置
        Vector3 spawnPosition = CalculateSpawnPosition();
        
        Gizmos.color = spawnRangeColor;
        Gizmos.DrawSphere(spawnPosition, 2.0f);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(spawnPosition, 2.0f);
        
        // 绘制生成器信息标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string spawnerInfo = "Boss Spawner";
        spawnerInfo += "\nSpawn Position: " + spawnPosition;
        spawnerInfo += "\nBoss Prefab: " + (bossPrefab != null ? bossPrefab.name : "None");
        spawnerInfo += "\nWave Spawner: " + (waveSpawner != null ? waveSpawner.name : "None");
        
        if (Application.isPlaying)
        {
            spawnerInfo += "\nBoss Spawned: " + hasSpawnedBoss;
            spawnerInfo += "\nBoss Defeated: " + hasDefeatedBoss;
        }
        
        UnityEditor.Handles.Label(spawnPosition + Vector3.up * 3.0f, spawnerInfo, style);
    }
    #endregion
    void BossDeath()
    {
        //清除Boss血条
        Debug.Log("BossDeath: Boss health bar deactivated.");
        bossHealthBar.SetActive(false);
    }
}