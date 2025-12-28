using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物死亡检测器
/// 功能：检测一定范围内的怪物死亡数量，并将检测范围可视化绘制出来
/// </summary>
public class MonsterDeathDetector : MonoBehaviour
{
    [Header("检测参数")]
    [SerializeField] private string monsterTag = "Enemy"; // 怪物的标签
    [SerializeField] private Vector2 detectionSize = new Vector2(10f, 10f); // 检测范围大小（宽, 高）
    [SerializeField] public int requiredDeadCount = 3; // 需要的死亡怪物数量阈值
    
    [Header("可视化设置")]
    [SerializeField] private Color detectionRangeColor = new Color(0, 1, 0, 0.2f); // 检测范围颜色
    [SerializeField] private Color deadMonsterColor = Color.red; // 死亡怪物颜色
    [SerializeField] private Color aliveMonsterColor = Color.green; // 存活怪物颜色
    
    public List<GameObject> detectedMonsters = new List<GameObject>(); // 检测到的怪物列表
    private int deadMonsterCount = 0; // 当前死亡怪物数量
    
    public System.Action<int, int> OnMonsterCountChanged; // 怪物数量变化事件（当前死亡数量，需要的数量）
    public System.Action OnThresholdReached; // 达到死亡数量阈值事件
    
    #region 生命周期方法
    public void Start()
    {
        deadMonsterCount = 0;
        // 初始检测范围内的怪物
        DetectMonstersInRange();
        
        // 统计初始死亡怪物数量
        UpdateDeadMonsterCount();
    }
    
    private void Update()
    {
        // 定期检测范围内的怪物
        if (Time.frameCount % 30 == 0) // 每30帧检测一次，约0.5秒
        {
            UpdateDeadMonsterCount();
            DetectMonstersInRange();
            
        }
    }
    #endregion
    
    #region 核心功能
    /// <summary>
    /// 检测范围内的怪物
    /// </summary>
    private void DetectMonstersInRange()
    {
        // 清除之前的怪物列表
        detectedMonsters.Clear();
        
        // 检测范围内所有带有指定标签的游戏对象
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0f);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(monsterTag)&&collider.GetComponent<EnemyStats>().currentHealth>=0)
            {
                detectedMonsters.Add(collider.gameObject);
                Debug.Log("检测到怪物：" + collider.gameObject.name);
            }
        }
    }
    
    /// <summary>
    /// 更新死亡怪物数量
    /// </summary>
    private void UpdateDeadMonsterCount()
    {
        int previousCount = deadMonsterCount;
        
        
        foreach (GameObject monster in detectedMonsters)
        {
            // 检查怪物是否死亡
            // 此逻辑将由用户自行补充
            if (IsMonsterDead(monster))
            {
                Debug.Log("怪物死亡：" +deadMonsterCount);
                deadMonsterCount++;
            }
        }
        
        // 如果死亡数量变化，触发事件
        if (deadMonsterCount != previousCount)
        {
            OnMonsterCountChanged?.Invoke(deadMonsterCount, requiredDeadCount);
        }
        
        // 如果达到阈值，触发事件
        if (deadMonsterCount >= requiredDeadCount)
        {
            OnThresholdReached?.Invoke();
        }
    }
    
    /// <summary>
    /// 检查怪物是否死亡
    /// 用户可以在此方法中补充具体的死亡检测逻辑
    /// </summary>
    /// <param name="monster">要检查的怪物</param>
    /// <returns>怪物是否死亡</returns>
    protected virtual bool IsMonsterDead(GameObject monster)
    {
        // 默认实现：检查怪物是否被禁用或销毁
        // 用户可以重写此方法以实现自定义的死亡检测逻辑
        if(monster.GetComponent<EnemyStats>().currentHealth<0)
        {
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 获取当前死亡怪物数量
    /// </summary>
    public int GetDeadMonsterCount()
    {
        return deadMonsterCount;
    }
    
    /// <summary>
    /// 获取需要的死亡怪物数量阈值
    /// </summary>
    public int GetRequiredDeadCount()
    {
        return requiredDeadCount;
    }
    
    /// <summary>
    /// 获取检测范围内的怪物总数
    /// </summary>
    public int GetTotalMonsterCount()
    {
        return detectedMonsters.Count;
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制检测范围
        Gizmos.color = detectionRangeColor;
        Gizmos.DrawWireCube(transform.position, detectionSize);
        Gizmos.color = new Color(detectionRangeColor.r, detectionRangeColor.g, detectionRangeColor.b, detectionRangeColor.a * 0.5f);
        Gizmos.DrawCube(transform.position, detectionSize);
        
        // 如果在运行时，绘制检测到的怪物
        if (Application.isPlaying)
        {
            foreach (GameObject monster in detectedMonsters)
            {
                // 检查怪物是否死亡
                bool isDead = IsMonsterDead(monster);
                
                // 根据怪物状态设置颜色
                Gizmos.color = isDead ? deadMonsterColor : aliveMonsterColor;
                Gizmos.DrawWireSphere(monster.transform.position, 0.5f);
                
                // 绘制怪物状态标签
                GUIStyle style = new GUIStyle();
                style.normal.textColor = isDead ? deadMonsterColor : aliveMonsterColor;
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
                
                UnityEditor.Handles.Label(monster.transform.position + Vector3.up * 0.8f, 
                                         isDead ? "Dead" : "Alive", style);
            }
        }
        
        // 绘制检测器信息标签
        GUIStyle detectorStyle = new GUIStyle();
        detectorStyle.normal.textColor = Color.white;
        detectorStyle.fontSize = 12;
        detectorStyle.fontStyle = FontStyle.Bold;
        
        string detectorInfo = "Monster Detector";
        detectorInfo += "\nSize: " + detectionSize.x + "x" + detectionSize.y;
        detectorInfo += "\nRequired: " + requiredDeadCount;
        
        if (Application.isPlaying)
        {
            detectorInfo += "\nDead/Total: " + deadMonsterCount + "/" + detectedMonsters.Count;
        }
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * (detectionSize.y / 2 + 1f), detectorInfo, detectorStyle);
    }
    #endregion
}