using UnityEngine;

/// <summary>
/// 玩家剑气动画事件脚本
/// 作为动画事件调用，在玩家面前生成剑气预制体
/// 根据玩家朝向调整生成位置和旋转
/// </summary>
public class PlayerSlashAnimationEvent : MonoBehaviour
{
    [Header("剑气设置")]
    [Tooltip("剑气预制体（能量等级2）")]
    public GameObject slashPrefabLevel2;
    
    [Tooltip("剑气预制体（能量等级3）")]
    public GameObject slashPrefabLevel3;
    
    [Tooltip("剑气预制体（能量等级4）")]
    public GameObject slashPrefabLevel4;
    
    [Tooltip("剑气生成位置偏移")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(1.0f, 0.3f);
   
    
    [Tooltip("剑气缩放")]
    [SerializeField]
    private Vector3 slashScale = Vector3.one;
    
    // 用于检测玩家朝向的SpriteRenderer
    private SpriteRenderer playerSpriteRenderer;
    [Header("玩家信息")]
    public Player player;
    [Tooltip("玩家能量系统")]
    public PlayerEnergySystem playerEnergySystem;
    
    
    private void Start()
    {
        // 自动查找玩家的SpriteRenderer组件
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on player. Using default facing direction.");
        }
    }
    
    /// <summary>
    /// 动画事件：生成剑气（能量等级2）
    /// 从动画关键帧调用此方法
    /// </summary>
    public void SpawnSlash()
    {
        if (playerEnergySystem.currentLevel >= PlayerEnergySystem.EnergyLevel.Level2)
        {

            if (slashPrefabLevel2 == null)
            {
                Debug.LogError("SlashPrefabLevel2 is not assigned in PlayerSlashAnimationEvent!");
                return;
            }
            if (playerEnergySystem.currentLevel >= PlayerEnergySystem.EnergyLevel.Level2)
            {
                // 确定玩家朝向
                bool isFacingRight = DetermineFacingDirection();

                // 计算生成位置
                Vector3 spawnPosition = CalculateSpawnPosition(isFacingRight);

                // 计算生成旋转
                Quaternion spawnRotation = CalculateSpawnRotation(isFacingRight);

                // 生成剑气预制体
                GameObject slash = Instantiate(slashPrefabLevel2, spawnPosition, spawnRotation);

            }
        }
    }
    
    /// <summary>
    /// 动画事件：生成剑气（能量等级3）
    /// 从动画关键帧调用此方法
    /// </summary>
    public void SpawnSlashLevel3()
    {
        if (playerEnergySystem.currentLevel >= PlayerEnergySystem.EnergyLevel.Level3)
        {
            if (slashPrefabLevel3 == null)
            {
                Debug.LogError("SlashPrefabLevel3 is not assigned in PlayerSlashAnimationEvent!");
                return;
            }

            // 确定玩家朝向
            bool isFacingRight = DetermineFacingDirection();

            // 计算生成位置
            Vector3 spawnPosition = CalculateSpawnPosition(isFacingRight);

            // 计算生成旋转
            Quaternion spawnRotation = CalculateSpawnRotation(isFacingRight);

            // 生成剑气预制体
            GameObject slash = Instantiate(slashPrefabLevel3, spawnPosition, spawnRotation);

  
        }
    }
    
    /// <summary>
    /// 动画事件：生成剑气（能量等级4）
    /// 从动画关键帧调用此方法
    /// </summary>
    public void SpawnSlashLevel4()
    {
        if (playerEnergySystem.currentLevel == PlayerEnergySystem.EnergyLevel.Level4)
        {
            if (slashPrefabLevel4 == null)
            {
                Debug.LogError("SlashPrefabLevel4 is not assigned in PlayerSlashAnimationEvent!");
                return;
            }

            // 确定玩家朝向
            bool isFacingRight = DetermineFacingDirection();

            // 计算生成位置
            Vector3 spawnPosition = CalculateSpawnPosition(isFacingRight);

            // 计算生成旋转
            Quaternion spawnRotation = CalculateSpawnRotation(isFacingRight);

            // 生成剑气预制体
            GameObject slash = Instantiate(slashPrefabLevel4, spawnPosition, spawnRotation);


        }
    }
    
    /// <summary>
    /// 动画事件：生成剑气（通用）
    /// 根据当前能量等级自动选择相应的剑气预制体
    /// 从动画关键帧调用此方法
    /// </summary>
    public void SpawnSlashGeneral()
    {
        GameObject currentSlashPrefab = null;
        
        // 根据当前能量等级选择相应的预制体
        switch (playerEnergySystem.currentLevel)
        {
            case PlayerEnergySystem.EnergyLevel.Level2:
                currentSlashPrefab = slashPrefabLevel2;
                break;
            case PlayerEnergySystem.EnergyLevel.Level3:
                currentSlashPrefab = slashPrefabLevel3;
                break;
            case PlayerEnergySystem.EnergyLevel.Level4:
                currentSlashPrefab = slashPrefabLevel4;
                break;
            default:
                // 能量等级不足，不生成剑气
                return;
        }
        
        if (currentSlashPrefab == null)
        {
            Debug.LogError($"Slash prefab for energy level {playerEnergySystem.currentLevel} is not assigned!");
            return;
        }
        
        // 确定玩家朝向
        bool isFacingRight = DetermineFacingDirection();
        
        // 计算生成位置
        Vector3 spawnPosition = CalculateSpawnPosition(isFacingRight);
        
        // 计算生成旋转
        Quaternion spawnRotation = CalculateSpawnRotation(isFacingRight);
        
        // 生成剑气预制体
        GameObject slash = Instantiate(currentSlashPrefab, spawnPosition, spawnRotation);
        
    
    }
    
    /// <summary>
    /// 确定玩家朝向
    /// </summary>
    private bool DetermineFacingDirection()
    {
        if (playerSpriteRenderer != null)
        {
            if(player.faceDirection == 1)
            // 使用SpriteRenderer的flipX属性判断朝向
            return true;
            else if(player.faceDirection == -1)
            return false;
            
        }
        
        // 默认朝向右侧
        return true;
    }
    
    /// <summary>
    /// 根据玩家朝向计算剑气生成位置
    /// </summary>
    private Vector3 CalculateSpawnPosition(bool isFacingRight)
    {
        Vector3 position = transform.position;
        
        // 根据朝向调整X轴偏移
        float xOffset = isFacingRight ? spawnOffset.x : -spawnOffset.x;
        
        position.x += xOffset;
        position.y += spawnOffset.y;
        
        return position;
    }
    
    /// <summary>
    /// 根据玩家朝向计算剑气生成旋转
    /// </summary>
    private Quaternion CalculateSpawnRotation(bool isFacingRight)
    {
        // 面向左侧时翻转X轴
        return isFacingRight ?  Quaternion.Euler(0f, 180f, 0f): Quaternion.identity;
    }
    
    // 可视化生成位置
    private void OnDrawGizmosSelected()
    {
        bool isFacingRight = DetermineFacingDirection();
        Vector3 spawnPosition = CalculateSpawnPosition(isFacingRight);
        Quaternion spawnRotation = CalculateSpawnRotation(isFacingRight);
        
        // 绘制生成位置
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spawnPosition, 0.1f);
        
        // 绘制剑气方向
        Gizmos.color = Color.yellow;
        Vector3 direction = spawnRotation * Vector3.right;
        Gizmos.DrawLine(spawnPosition, spawnPosition + direction * 0.5f);
        
        // 绘制不同能量等级的剑气范围
        Matrix4x4 oldMatrix = Gizmos.matrix;
        
        // 能量等级2
        if (slashPrefabLevel2 != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f); // 黄色半透明
            Gizmos.matrix = Matrix4x4.TRS(spawnPosition, spawnRotation, slashScale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
        
        // 能量等级3
        if (slashPrefabLevel3 != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // 红色半透明
            Gizmos.matrix = Matrix4x4.TRS(spawnPosition, spawnRotation, slashScale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 1.2f); // 稍微大一点
        }
        
        // 能量等级4
        if (slashPrefabLevel4 != null)
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.2f); // 洋红色半透明
            Gizmos.matrix = Matrix4x4.TRS(spawnPosition, spawnRotation, slashScale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 1.4f); // 更大一些
        }
        
        Gizmos.matrix = oldMatrix;
    }
}