// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class BossBattleSpawner : MonoBehaviour
// {
//     [System.Serializable]
//     public class EnemyWave
//     {
//         public string waveName;
//         public EnemySpawn[] enemySpawns;
//         public float waveDelay = 0f;
//         public float spawnInterval = 1f;
//     }
    
//     [System.Serializable]
//     public class EnemySpawn
//     {
//         public GameObject enemyPrefab;
//         public Transform spawnPoint;
//         public int spawnCount = 1;
//         public float delayBetweenSpawns = 0.5f;
//     }
    
//     [Header("Enemy Waves")]
//     public List<EnemyWave> enemyWaves = new List<EnemyWave>();
    
//     [Header("Spawn Settings")]
//     public bool useObjectPooling = true;
//     public int maxPoolSize = 50;
//     public Transform player;
    
//     [Header("Visualization")]
//     public Color spawnPointColor = Color.green;
//     public float spawnPointGizmoSize = 0.5f;
    
//     private Dictionary<string, Queue<GameObject>> enemyPool = new Dictionary<string, Queue<GameObject>>();
//     private Coroutine waveCoroutine;
//     private bool isSpawning = false;
//     private int currentWaveIndex = 0;
    
//     private void OnEnable()
//     {
//         // 订阅Boss战事件
//         if (EventManager.Instance != null)
//         {
//             EventManager.Instance.OnBossBattleStart.AddListener(StartSpawningWaves);
//             EventManager.Instance.OnBossBattleEnd.AddListener(StopSpawning);
//         }
//     }
    
//     private void OnDisable()
//     {
//         // 取消订阅Boss战事件
//         if (EventManager.Instance != null)
//         {
//             EventManager.Instance.OnBossBattleStart.RemoveListener(StartSpawningWaves);
//             EventManager.Instance.OnBossBattleEnd.RemoveListener(StopSpawning);
//         }
//     }
    
//     private void Start()
//     {
//         // 如果启用对象池，初始化池
//         if (useObjectPooling)
//         {
//             InitializeObjectPool();
//         }
//     }
    
//     private void InitializeObjectPool()
//     {
//         enemyPool.Clear();
        
//         // 收集所有不同类型的敌人预制体
//         HashSet<GameObject> uniqueEnemies = new HashSet<GameObject>();
        
//         foreach (EnemyWave wave in enemyWaves)
//         {
//             foreach (EnemySpawn spawn in wave.enemySpawns)
//             {
//                 if (spawn.enemyPrefab != null)
//                 {
//                     uniqueEnemies.Add(spawn.enemyPrefab);
//                 }
//             }
//         }
        
//         // 为每种敌人类型创建对象池
//         foreach (GameObject enemyPrefab in uniqueEnemies)
//         {
//             string enemyKey = enemyPrefab.name;
//             if (!enemyPool.ContainsKey(enemyKey))
//             {
//                 enemyPool[enemyKey] = new Queue<GameObject>();
                
//                 // 预先实例化一些敌人到池中
//                 for (int i = 0; i < Mathf.Min(10, maxPoolSize); i++)
//                 {
//                     GameObject enemy = Instantiate(enemyPrefab);
//                     enemy.SetActive(false);
//                     enemy.transform.SetParent(transform);
//                     enemyPool[enemyKey].Enqueue(enemy);
//                 }
//             }
//         }
//     }
    
//     private void StartSpawningWaves()
//     {
//         if (isSpawning || enemyWaves.Count == 0)
//             return;
            
//         isSpawning = true;
//         currentWaveIndex = 0;
//         waveCoroutine = StartCoroutine(SpawnWavesCoroutine());
//     }
    
//     private void StopSpawning()
//     {
//         isSpawning = false;
        
//         if (waveCoroutine != null)
//         {
//             StopCoroutine(waveCoroutine);
//             waveCoroutine = null;
//         }
        
//         // 清理所有生成的敌人
//         CleanupAllEnemies();
//     }
    
//     private IEnumerator SpawnWavesCoroutine()
//     {
//         while (currentWaveIndex < enemyWaves.Count && isSpawning)
//         {
//             EnemyWave currentWave = enemyWaves[currentWaveIndex];
            
//             Debug.Log("Starting Boss Battle Wave: " + currentWave.waveName);
            
//             // 等待波次延迟
//             if (currentWave.waveDelay > 0f)
//             {
//                 yield return new WaitForSeconds(currentWave.waveDelay);
//             }
            
//             // 生成当前波次的所有敌人
//             yield return StartCoroutine(SpawnWave(currentWave));
            
//             // 等待波次间的间隔
//             yield return new WaitForSeconds(currentWave.spawnInterval);
            
//             currentWaveIndex++;
//         }
        
//         // 所有波次生成完成
//         if (isSpawning)
//         {
//             Debug.Log("All Boss Battle Waves Completed");
//         }
        
//         isSpawning = false;
//     }
    
//     private IEnumerator SpawnWave(EnemyWave wave)
//     {
//         foreach (EnemySpawn spawn in wave.enemySpawns)
//         {
//             if (!isSpawning) yield break;
            
//             if (spawn.enemyPrefab == null || spawn.spawnPoint == null)
//                 continue;
                
//             for (int i = 0; i < spawn.spawnCount; i++)
//             {
//                 if (!isSpawning) yield break;
                
//                 SpawnEnemy(spawn.enemyPrefab, spawn.spawnPoint.position);
                
//                 yield return new WaitForSeconds(spawn.delayBetweenSpawns);
//             }
//         }
//     }
    
//     private void SpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition)
//     {
//         if (enemyPrefab == null)
//             return;
            
//         GameObject enemy = null;
//         string enemyKey = enemyPrefab.name;
        
//         if (useObjectPooling && enemyPool.ContainsKey(enemyKey))
//         {
//             // 从对象池获取敌人
//             if (enemyPool[enemyKey].Count > 0)
//             {
//                 enemy = enemyPool[enemyKey].Dequeue();
//                 enemy.SetActive(true);
//             }
//             else if (enemyPool[enemyKey].Count < maxPoolSize)
//             {
//                 // 如果池已满但未达到最大大小，创建新敌人
//                 enemy = Instantiate(enemyPrefab);
//             }
//         }
        
//         if (enemy == null)
//         {
//             // 如果对象池不可用或已满，直接实例化
//             enemy = Instantiate(enemyPrefab);
//         }
        
//         if (enemy != null)
//         {
//             enemy.transform.position = spawnPosition;
//             enemy.transform.rotation = Quaternion.identity;
            
//             // 设置敌人的目标为玩家
//             EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
//             if (enemyAI != null && player != null)
//             {
//                 enemyAI.SetTarget(player);
//             }
            
//             // 注册敌人死亡事件，以便将其返回对象池
//             if (useObjectPooling)
//             {
//                 HealthComponent health = enemy.GetComponent<HealthComponent>();
//                 if (health != null)
//                 {
//                     health.OnDeath -= ReturnEnemyToPool;
//                     health.OnDeath += ReturnEnemyToPool;
//                 }
//             }
//         }
//     }
    
//     private void ReturnEnemyToPool(GameObject enemy)
//     {
//         if (!useObjectPooling || enemy == null)
//             return;
            
//         string enemyKey = enemy.name.Split('(')[0].Trim();
        
//         if (enemyPool.ContainsKey(enemyKey) && enemyPool[enemyKey].Count < maxPoolSize)
//         {
//             // 重置敌人状态
//             enemy.SetActive(false);
//             enemy.transform.SetParent(transform);
            
//             // 移除死亡事件监听
//             HealthComponent health = enemy.GetComponent<HealthComponent>();
//             if (health != null)
//             {
//                 health.OnDeath -= ReturnEnemyToPool;
//             }
            
//             // 返回对象池
//             enemyPool[enemyKey].Enqueue(enemy);
//         }
//         else
//         {
//             // 如果对象池已满，销毁敌人
//             Destroy(enemy);
//         }
//     }
    
//     private void CleanupAllEnemies()
//     {
//         // 清理所有活跃的敌人
//         foreach (Transform child in transform)
//         {
//             if (child != transform && child.gameObject.activeSelf)
//             {
//                 // 检查是否是敌人
//                 if (child.GetComponent<EnemyAI>() != null || child.GetComponent<HealthComponent>() != null)
//                 {
//                     if (useObjectPooling)
//                     {
//                         ReturnEnemyToPool(child.gameObject);
//                     }
//                     else
//                     {
//                         Destroy(child.gameObject);
//                     }
//                 }
//             }
//         }
//     }
    
//     // 手动触发特定波次的方法
//     public void SpawnSpecificWave(int waveIndex)
//     {
//         if (waveIndex >= 0 && waveIndex < enemyWaves.Count)
//         {
//             StartCoroutine(SpawnWave(enemyWaves[waveIndex]));
//         }
//     }
    
//     // 重置生成器的方法
//     public void ResetSpawner()
//     {
//         StopSpawning();
//         currentWaveIndex = 0;
        
//         // 重置对象池
//         if (useObjectPooling)
//         {
//             InitializeObjectPool();
//         }
//     }
    
//     // 可视化生成点
//     private void OnDrawGizmosSelected()
//     {
//         // 绘制所有生成点
//         foreach (EnemyWave wave in enemyWaves)
//         {
//             foreach (EnemySpawn spawn in wave.enemySpawns)
//             {
//                 if (spawn.spawnPoint != null)
//                 {
//                     Gizmos.color = spawnPointColor;
//                     Gizmos.DrawSphere(spawn.spawnPoint.position, spawnPointGizmoSize);
                    
//                     // 绘制指向玩家的线（如果玩家存在）
//                     if (player != null)
//                     {
//                         Gizmos.color = Color.yellow;
//                         Gizmos.DrawLine(spawn.spawnPoint.position, player.position);
//                     }
//                 }
//             }
//         }
        
//         // 绘制生成器位置
//         Gizmos.color = Color.blue;
//         Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
//     }
    
//     // 编辑器扩展：在Inspector中添加快速测试按钮
//     [ContextMenu("Test Spawn First Wave")]
//     private void TestSpawnFirstWave()
//     {
//         if (enemyWaves.Count > 0)
//         {
//             StartCoroutine(SpawnWave(enemyWaves[0]));
//         }
//     }
    
//     [ContextMenu("Clear All Enemies")]
//     private void TestClearAllEnemies()
//     {
//         CleanupAllEnemies();
//     }
// }
