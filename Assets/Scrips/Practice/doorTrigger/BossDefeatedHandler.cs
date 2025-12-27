using UnityEngine;

/// <summary>
/// Boss被击败事件处理器
/// 功能：演示如何使用BossSpawner的OnBossDefeated事件
/// </summary>
public class BossDefeatedHandler : MonoBehaviour
{
    [Header("事件源")]
    [SerializeField] private BossSpawner bossSpawner; // Boss生成器
    
    [Header("事件目标")]
    [SerializeField] private GameObject doorToOpen; // 要打开的门
    [SerializeField] private GameObject victoryUI; // 胜利UI
    [SerializeField] private GameObject rewardPrefab; // 奖励预制体
    [SerializeField] private Transform rewardSpawnPosition; // 奖励生成位置
    
    [Header("视觉效果")]
    [SerializeField] private AudioClip victorySound; // 胜利音效
    [SerializeField] private AudioSource audioSource; // 音频源
    
    [Header("调试信息")]
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    #region 生命周期方法
    private void Start()
    {
        // 验证必要的组件
        if (bossSpawner == null)
        {
            Debug.LogError("BossDefeatedHandler: BossSpawner is not assigned!");
            return;
        }
        
        // 订阅Boss相关事件
        bossSpawner.OnBossSpawned += HandleBossSpawned;
        bossSpawner.OnBossDefeated += HandleBossDefeated;
        
        if (showDebugInfo)
        {
            Debug.Log("BossDefeatedHandler initialized and listening for Boss events.");
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件（避免内存泄漏）
        if (bossSpawner != null)
        {
            bossSpawner.OnBossSpawned -= HandleBossSpawned;
            bossSpawner.OnBossDefeated -= HandleBossDefeated;
        }
    }
    #endregion
    
    #region 事件处理
    /// <summary>
    /// 处理Boss生成事件
    /// </summary>
    private void HandleBossSpawned()
    {
        if (showDebugInfo)
        {
            Debug.Log("BossDefeatedHandler: Boss has spawned!");
        }
        
        // 可以在这里添加Boss生成时的逻辑，比如：
        // 1. 关闭门，防止玩家逃跑
        // 2. 显示Boss战UI
        // 3. 播放Boss战音乐
        
        if (doorToOpen != null)
        {
            doorToOpen.SetActive(true); // 确保门是关闭的
        }
    }
    
    /// <summary>
    /// 处理Boss被击败事件
    /// </summary>
    private void HandleBossDefeated()
    {
        if (showDebugInfo)
        {
            Debug.Log("BossDefeatedHandler: Boss has been defeated!");
        }
        
        // 1. 打开门，允许玩家继续前进
        if (doorToOpen != null)
        {
            doorToOpen.SetActive(false); // 打开门（隐藏门对象）
            // 或者触发门的动画：doorToOpen.GetComponent<Animator>().SetTrigger("Open");
        }
        
        // 2. 显示胜利UI
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
        }
        
        // 3. 播放胜利音效
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
        
        // 4. 生成奖励
        if (rewardPrefab != null)
        {
            Vector3 spawnPosition = rewardSpawnPosition != null ? 
                rewardSpawnPosition.position : 
                transform.position; // 默认生成在当前对象位置
            
            Instantiate(rewardPrefab, spawnPosition, Quaternion.identity);
        }
        
        // 5. 可以添加更多逻辑，比如：
        // - 保存游戏进度
        // - 增加玩家经验或分数
        // - 解锁新技能或装备
        // - 显示游戏结束画面
        // - 加载下一关
    }
    #endregion
}