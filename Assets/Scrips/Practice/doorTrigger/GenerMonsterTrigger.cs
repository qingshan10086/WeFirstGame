using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物产生器触发器
/// 功能：在指定范围内随机产生怪物，支持触发条件和产生参数配置
/// </summary>
public class GenerMonsterTrigger : MonoBehaviour
{
    [Header("核心参数")]
    [SerializeField] private Transform playerTransform; // 玩家位置（用于触发条件）
    [SerializeField] private GameObject[] monsterPrefabs; // 怪物预制体数组
    [SerializeField] private int maxMonsterCount = 5; // 最大怪物数量
    [SerializeField] private float spawnInterval = 2f; // 怪物产生间隔（秒）
    [SerializeField] private float spawnRangeSize = 10f; // 产生范围大小
    [SerializeField] private Vector2 spawnRangeOffset = Vector2.zero; // 产生范围偏移
    [SerializeField] private bool spawnOnTriggerEnter = true; // 是否在玩家进入触发器时开始产生
    [SerializeField] private bool spawnOnStart = false; // 是否在游戏开始时开始产生
    [SerializeField] private bool spawnInWave = false; // 是否以波次方式产生
    [SerializeField] private int monstersPerWave = 3; // 每波产生的怪物数量
    [SerializeField] private float waveInterval = 5f; // 波次间隔（秒）
    
    [Header("高级设置")]
    [SerializeField] private string playerTag = "Player"; // 玩家标签
    [SerializeField] private float spawnCooldown = 0.5f; // 单个怪物产生冷却时间
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    [SerializeField] private bool destroyOnComplete = false; // 完成产生后是否销毁自身
    
    [Header("视觉效果")]
    [SerializeField] private Color spawnRangeColor = new Color(1, 0, 0, 0.2f); // 产生范围颜色
    
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 已产生的怪物列表
    private Coroutine spawnCoroutine; // 产生协程
    private Coroutine waveCoroutine; // 波次产生协程
    private bool isSpawning = false; // 是否正在产生怪物
    private float lastSpawnTime = -Mathf.Infinity; // 上次产生时间
    private int currentWave = 0; // 当前波次数
    
    public System.Action OnStartSpawning; // 开始产生事件
    public System.Action OnStopSpawning; // 停止产生事件
    public System.Action OnWaveComplete; // 波次完成事件
    public System.Action OnAllMonstersSpawned; // 所有怪物产生完成事件
    
    #region 生命周期方法
    private void Awake()
    {
        // 确保触发器有碰撞体
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
        collider.size = new Vector2(spawnRangeSize, spawnRangeSize);
    }
    
    private void Start()
    {
        // 如果设置了在游戏开始时产生，启动产生
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }
    
    private void Update()
    {
        // 清理已销毁的怪物
        for (int i = spawnedMonsters.Count - 1; i >= 0; i--)
        {
            if (spawnedMonsters[i] == null)
            {
                spawnedMonsters.RemoveAt(i);
            }
        }
        
        // 如果达到最大数量，停止产生
        if (isSpawning && spawnedMonsters.Count >= maxMonsterCount)
        {
            StopSpawning();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果设置了在玩家进入时产生，且碰撞对象是玩家
        if (spawnOnTriggerEnter && other.CompareTag(playerTag))
        {
            StartSpawning();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // 玩家离开时可以选择停止产生
        // 这里可以根据需求添加逻辑
    }
    #endregion
    
    #region 核心功能
    /// <summary>
    /// 开始产生怪物
    /// </summary>
    public void StartSpawning()
    {
        if (isSpawning || monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            return;
        }
        
        isSpawning = true;
        OnStartSpawning?.Invoke();
        
        if (showDebugInfo)
        {
            Debug.Log("怪物产生器开始产生怪物：" + gameObject.name);
        }
        
        // 根据产生模式选择不同的产生方式
        if (spawnInWave)
        {
            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine);
            }
            waveCoroutine = StartCoroutine(SpawnWaveCoroutine());
        }
        else
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            spawnCoroutine = StartCoroutine(SpawnMonsterCoroutine());
        }
    }
    
    /// <summary>
    /// 停止产生怪物
    /// </summary>
    public void StopSpawning()
    {
        if (!isSpawning)
        {
            return;
        }
        
        isSpawning = false;
        OnStopSpawning?.Invoke();
        
        if (showDebugInfo)
        {
            Debug.Log("怪物产生器停止产生怪物：" + gameObject.name);
        }
        
        // 停止所有产生协程
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }
        
        // 如果设置了完成后销毁，销毁自身
        if (destroyOnComplete)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 怪物产生协程（持续产生）
    /// </summary>
    private IEnumerator SpawnMonsterCoroutine()
    {
        while (isSpawning && spawnedMonsters.Count < maxMonsterCount)
        {
            if (Time.time >= lastSpawnTime + spawnCooldown)
            {
                SpawnSingleMonster();
                lastSpawnTime = Time.time;
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
        
        OnAllMonstersSpawned?.Invoke();
        StopSpawning();
    }
    
    /// <summary>
    /// 波次产生协程
    /// </summary>
    private IEnumerator SpawnWaveCoroutine()
    {
        while (isSpawning && spawnedMonsters.Count < maxMonsterCount)
        {
            currentWave++;
            
            if (showDebugInfo)
            {
                Debug.Log("开始第 " + currentWave + " 波怪物产生");
            }
            
            // 产生一波怪物
            for (int i = 0; i < monstersPerWave && spawnedMonsters.Count < maxMonsterCount; i++)
            {
                if (Time.time >= lastSpawnTime + spawnCooldown)
                {
                    SpawnSingleMonster();
                    lastSpawnTime = Time.time;
                    yield return new WaitForSeconds(spawnCooldown);
                }
            }
            
            OnWaveComplete?.Invoke();
            
            // 如果已达到最大数量，停止
            if (spawnedMonsters.Count >= maxMonsterCount)
            {
                break;
            }
            
            // 等待波次间隔
            yield return new WaitForSeconds(waveInterval);
        }
        
        OnAllMonstersSpawned?.Invoke();
        StopSpawning();
    }
    
    /// <summary>
    /// 产生单个怪物
    /// </summary>
    private void SpawnSingleMonster()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            return;
        }
        
        // 随机选择一个怪物预制体
        int prefabIndex = Random.Range(0, monsterPrefabs.Length);
        GameObject monsterPrefab = monsterPrefabs[prefabIndex];
        
        // 计算随机产生位置
        Vector2 spawnPosition = CalculateRandomSpawnPosition();
        
        // 产生怪物
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        spawnedMonsters.Add(monster);
        
        if (showDebugInfo)
        {
            Debug.Log("产生怪物：" + monster.name + "，位置：" + spawnPosition);
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
    /// 获取已产生的怪物数量
    /// </summary>
    public int GetSpawnedMonsterCount()
    {
        // 清理已销毁的怪物
        spawnedMonsters.RemoveAll(monster => monster == null);
        return spawnedMonsters.Count;
    }
    
    /// <summary>
    /// 获取当前波次数
    /// </summary>
    public int GetCurrentWave()
    {
        return currentWave;
    }
    
    /// <summary>
    /// 重置怪物产生器
    /// </summary>
    public void ResetSpawner()
    {
        StopSpawning();
        
        // 清理所有已产生的怪物
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        
        spawnedMonsters.Clear();
        currentWave = 0;
        lastSpawnTime = -Mathf.Infinity;
        
        if (showDebugInfo)
        {
            Debug.Log("怪物产生器已重置：" + gameObject.name);
        }
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
        
        string spawnerInfo = "Monster Spawner";
        spawnerInfo += "\nRange: " + spawnRangeSize;
        spawnerInfo += "\nMax: " + maxMonsterCount;
        spawnerInfo += "\nInterval: " + spawnInterval + "s";
        
        if (spawnInWave)
        {
            spawnerInfo += "\nWave Mode: " + monstersPerWave + " per wave";
            spawnerInfo += "\nWave Interval: " + waveInterval + "s";
        }
        
        if (Application.isPlaying)
        {
            spawnerInfo += "\nSpawned: " + spawnedMonsters.Count;
            spawnerInfo += "\nWave: " + currentWave;
            spawnerInfo += "\nSpawning: " + isSpawning;
        }
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * (spawnRangeSize / 2 + 1f), spawnerInfo, style);
        
        // 绘制已产生的怪物（如果在运行时）
        if (Application.isPlaying)
        {
            foreach (GameObject monster in spawnedMonsters)
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
