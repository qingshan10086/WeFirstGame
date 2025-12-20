using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 波次怪物产生器
/// 功能：实现4波次怪物产生，每波怪物和数量可在Unity中设定，结合MonsterDeathDetector，第一波由触发器触发，
/// 后续波次在前一波怪物完全消灭后触发，所有波次完成后触发事件
/// </summary>
public class WaveMonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MonsterWave
    {
        [SerializeField] private GameObject[] monsterPrefabs; // 此波次的怪物预制体数组
        [SerializeField] private int[] monsterCounts; // 对应预制体的产生数量
        [SerializeField] private float spawnDelay = 0.5f; // 此波次内怪物产生的间隔
        
        public GameObject[] MonsterPrefabs { get { return monsterPrefabs; } }
        public int[] MonsterCounts { get { return monsterCounts; } }
        public float SpawnDelay { get { return spawnDelay; } }
    }
    
    [Header("核心参数")]
    [SerializeField] private string playerTag = "Player"; // 玩家标签
    [SerializeField] private MonsterDeathDetector monsterDeathDetector; // 怪物死亡检测器
    [SerializeField] private MonsterWave[] monsterWaves = new MonsterWave[4]; // 4波次配置
    [SerializeField] private float spawnRangeSize = 10f; // 产生范围大小
    [SerializeField] private Vector2 spawnRangeOffset = Vector2.zero; // 产生范围偏移
    [SerializeField] private float spawnCooldown = 0.2f; // 单个怪物产生冷却时间
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    [Header("视觉效果")]
    [SerializeField] private Color spawnRangeColor = new Color(1, 0, 0, 0.2f); // 产生范围颜色
    
    private int currentWaveIndex = -1; // 当前波次索引（-1表示未开始）
    private List<GameObject> spawnedMonstersInCurrentWave = new List<GameObject>(); // 当前波次产生的怪物列表
    private bool isWaveSpawning = false; // 是否正在产生当前波次
    private bool isAllWavesCompleted = false; // 所有波次是否已完成
    private float lastSpawnTime = -Mathf.Infinity; // 上次产生时间
    private int monstersToSpawnCount = 0; // 当前波次需要产生的总怪物数量
    private int monstersSpawnedCount = 0; // 当前波次已产生的怪物数量
    
    public System.Action OnWaveStart; // 波次开始事件
    public System.Action<int> OnWaveComplete; // 波次完成事件（参数：当前波次数）
    public System.Action OnAllWavesCompleted; // 所有波次完成事件
    public System.Action OnFirstWaveTriggered; // 第一波被触发器触发事件
    
    #region 生命周期方法
    private void Awake()
    {
        // 确保有4波次配置
        if (monsterWaves.Length != 4)
        {
            Debug.LogWarning("WaveMonsterSpawner should have exactly 4 waves. Adjusting to 4 waves.");
            MonsterWave[] tempWaves = new MonsterWave[4];
            for (int i = 0; i < Mathf.Min(4, monsterWaves.Length); i++)
            {
                tempWaves[i] = monsterWaves[i];
            }
            monsterWaves = tempWaves;
        }
        
        // 确保有碰撞体用于触发
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }
    
    private void Start()
    {
        // 如果设置了怪物死亡检测器，订阅其事件
        if (monsterDeathDetector != null)
        {
            monsterDeathDetector.OnThresholdReached += HandleMonsterWaveComplete;
        }
        else
        {
            Debug.LogWarning("MonsterDeathDetector not assigned to WaveMonsterSpawner.");
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (monsterDeathDetector != null)
        {
            monsterDeathDetector.OnThresholdReached -= HandleMonsterWaveComplete;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 当玩家进入触发器时，开始第一波（如果还未开始）
        if (other.CompareTag(playerTag) && currentWaveIndex == -1 && !isAllWavesCompleted)
        {
            StartNextWave();
            OnFirstWaveTriggered?.Invoke();
            
            if (showDebugInfo)
            {
                Debug.Log("First wave triggered by player entering spawner trigger.");
            }
        }
    }
    
    private void Update()
    {
        // 清理已销毁的怪物
        spawnedMonstersInCurrentWave.RemoveAll(monster => monster == null);
        
        // 如果当前波次正在产生且还有怪物需要产生
        if (isWaveSpawning && monstersSpawnedCount < monstersToSpawnCount)
        {
            if (Time.time >= lastSpawnTime + spawnCooldown)
            {
                SpawnSingleMonsterFromCurrentWave();
                lastSpawnTime = Time.time;
            }
        }
    }
    #endregion
    
    #region 核心功能
    /// <summary>
    /// 开始下一波怪物产生
    /// </summary>
    public void StartNextWave()
    {
        if (isAllWavesCompleted)
        {
            Debug.LogWarning("All waves have already been completed.");
            return;
        }
        
        // 增加波次索引
        currentWaveIndex++;
        
        // 检查是否所有波次都已完成
        if (currentWaveIndex >= monsterWaves.Length)
        {
            CompleteAllWaves();
            return;
        }
        
        // 重置当前波次状态
        spawnedMonstersInCurrentWave.Clear();
        isWaveSpawning = true;
        monstersSpawnedCount = 0;
        
        // 计算当前波次需要产生的总怪物数量
        monstersToSpawnCount = 0;
        MonsterWave currentWave = monsterWaves[currentWaveIndex];
        
        for (int i = 0; i < currentWave.MonsterCounts.Length; i++)
        {
            monstersToSpawnCount += currentWave.MonsterCounts[i];
        }
        
        // 更新怪物死亡检测器的阈值
        if (monsterDeathDetector != null)
        {
            monsterDeathDetector.requiredDeadCount = monstersToSpawnCount;
            monsterDeathDetector.Start();
        }
        
        OnWaveStart?.Invoke();
        
        if (showDebugInfo)
        {
            Debug.Log("Starting wave " + (currentWaveIndex + 1) + ", monsters to spawn: " + monstersToSpawnCount);
        }
    }
    
    /// <summary>
    /// 从当前波次中产生单个怪物
    /// </summary>
    private void SpawnSingleMonsterFromCurrentWave()
    {
        if (currentWaveIndex < 0 || currentWaveIndex >= monsterWaves.Length)
        {
            return;
        }
        
        MonsterWave currentWave = monsterWaves[currentWaveIndex];
        
        // 计算当前波次剩余需要产生的怪物
        int monstersLeftToSpawn = monstersToSpawnCount - monstersSpawnedCount;
        if (monstersLeftToSpawn <= 0)
        {
            isWaveSpawning = false;
            return;
        }
        
        // 查找还需要产生的怪物预制体
        GameObject monsterPrefab = null;
        
        for (int i = 0; i < currentWave.MonsterCounts.Length; i++)
        {
            if (currentWave.MonsterCounts[i] > 0)
            {
                // 检查对应索引的预制体是否存在
                if (i < currentWave.MonsterPrefabs.Length && currentWave.MonsterPrefabs[i] != null)
                {
                    monsterPrefab = currentWave.MonsterPrefabs[i];
                    // 减少计数
                    currentWave.MonsterCounts[i]--;
                    break;
                }
            }
        }
        
        if (monsterPrefab != null)
        {
            // 计算随机产生位置
            Vector2 spawnPosition = CalculateRandomSpawnPosition();
            
            // 产生怪物
            GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            
            // 设置怪物标签为Enemy
            monster.tag = "Enemy";
            
            // 设置怪物绘制层级为Enemy
            SpriteRenderer spriteRenderer = monster.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = "Enemy";
            }
            
            // 检查并设置子对象的绘制层级
            SpriteRenderer[] childRenderers = monster.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer childRenderer in childRenderers)
            {
                childRenderer.sortingLayerName = "Enemy";
            }
            
            spawnedMonstersInCurrentWave.Add(monster);
            monstersSpawnedCount++;
            
            if (showDebugInfo)
            {
                Debug.Log("Spawned monster " + monstersSpawnedCount + " of " + monstersToSpawnCount + " in wave " + (currentWaveIndex + 1));
            }
            
            // 检查是否所有怪物都已产生
            if (monstersSpawnedCount >= monstersToSpawnCount)
            {
                isWaveSpawning = false;
                if (showDebugInfo)
                {
                    Debug.Log("All monsters spawned for wave " + (currentWaveIndex + 1));
                }
            }
        }
        else
        {
            // 如果没有找到可产生的怪物，标记波次产生完成
            isWaveSpawning = false;
            if (showDebugInfo)
            {
                Debug.LogWarning("No monster prefabs available for wave " + (currentWaveIndex + 1));
            }
        }
    }
    
    /// <summary>
    /// 处理当前波次怪物完全消灭事件
    /// </summary>
    private void HandleMonsterWaveComplete()
    {
        // 检查当前波次是否真的所有怪物都已死亡
        int aliveMonsters = 0;
        foreach (GameObject monster in spawnedMonstersInCurrentWave)
        {
            if (monster != null)
            {
                EnemyStats enemyStats = monster.GetComponent<EnemyStats>();
                if (enemyStats != null && enemyStats.currentHealth > 0)
                {
                    aliveMonsters++;
                }
            }
        }
        
        if (aliveMonsters <= 0)
        {
            // 当前波次完成
            OnWaveComplete?.Invoke(currentWaveIndex + 1);
            
            if (showDebugInfo)
            {
                Debug.Log("Wave " + (currentWaveIndex + 1) + " completed. All monsters destroyed.");
            }
            
            // 开始下一波
            StartNextWave();
        }
    }
    
    /// <summary>
    /// 所有波次完成
    /// </summary>
    private void CompleteAllWaves()
    {
        isAllWavesCompleted = true;
        isWaveSpawning = false;
        
        OnAllWavesCompleted?.Invoke();
        
        if (showDebugInfo)
        {
            Debug.Log("All 4 waves completed!");
        }
    }
    
    /// <summary>
    /// 计算随机产生位置
    /// </summary>
    /// <returns>随机产生位置</returns>
    private Vector2 CalculateRandomSpawnPosition()
    {
        // 计算产生范围边界
        float minX = transform.position.x - spawnRangeSize / 2f + spawnRangeOffset.x;
        float maxX = transform.position.x + spawnRangeSize / 2f + spawnRangeOffset.x;
        float minY = transform.position.y - spawnRangeSize / 2f + spawnRangeOffset.y;
        float maxY = transform.position.y + spawnRangeSize / 2f + spawnRangeOffset.y;
        
        // 随机生成位置
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        
        return new Vector2(randomX, randomY);
    }
    
    /// <summary>
    /// 重置波次产生器
    /// </summary>
    public void ResetSpawner()
    {
        // 停止所有波次
        isWaveSpawning = false;
        isAllWavesCompleted = false;
        currentWaveIndex = -1;
        monstersSpawnedCount = 0;
        monstersToSpawnCount = 0;
        lastSpawnTime = -Mathf.Infinity;
        
        // 销毁所有已产生的怪物
        foreach (GameObject monster in spawnedMonstersInCurrentWave)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        
        spawnedMonstersInCurrentWave.Clear();
        
        if (showDebugInfo)
        {
            Debug.Log("Wave monster spawner has been reset.");
        }
    }
    
    /// <summary>
    /// 获取当前波次数（从1开始）
    /// </summary>
    public int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }
    
    /// <summary>
    /// 检查所有波次是否已完成
    /// </summary>
    public bool IsAllWavesCompleted()
    {
        return isAllWavesCompleted;
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制产生范围
        Gizmos.color = spawnRangeColor;
        Vector3 spawnRangeSizeVector = new Vector3(spawnRangeSize, spawnRangeSize, 0.1f);
        Vector3 spawnRangeCenter = new Vector3(
            transform.position.x + spawnRangeOffset.x,
            transform.position.y + spawnRangeOffset.y,
            transform.position.z
        );
        Gizmos.DrawCube(spawnRangeCenter, spawnRangeSizeVector);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(spawnRangeCenter, spawnRangeSizeVector);
        
        // 绘制产生器信息标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string spawnerInfo = "Wave Monster Spawner";
        spawnerInfo += "\nRange: " + spawnRangeSize;
        spawnerInfo += "\nWaves: " + monsterWaves.Length;
        
        if (Application.isPlaying)
        {
            spawnerInfo += "\nCurrent Wave: " + (currentWaveIndex + 1);
            spawnerInfo += "\nSpawned: " + monstersSpawnedCount + "/" + monstersToSpawnCount;
            spawnerInfo += "\nSpawning: " + isWaveSpawning;
            spawnerInfo += "\nAll Completed: " + isAllWavesCompleted;
        }
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * (spawnRangeSize / 2 + 1f), spawnerInfo, style);
        
        // 绘制当前波次已产生的怪物（如果在运行时）
        if (Application.isPlaying)
        {
            foreach (GameObject monster in spawnedMonstersInCurrentWave)
            {
                if (monster != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(monster.transform.position, 0.3f);
                }
            }
        }
    }
    #endregion
}