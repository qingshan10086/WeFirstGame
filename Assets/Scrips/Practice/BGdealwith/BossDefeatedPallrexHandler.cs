using UnityEngine;

/// <summary>
/// Boss被打败事件处理器
/// 功能：订阅Boss被打败事件，将pallrex的sign属性设置为true
/// </summary>
public class BossDefeatedPallrexHandler : MonoBehaviour
{
    [Header("事件源")]
    [SerializeField] private BossSpawner bossSpawner; // Boss生成器
    
    [Header("目标组件")]
    [SerializeField] private pallrex pallrexScript; // pallrex脚本
    
    [Header("调试信息")]
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    #region 生命周期方法
    private void Start()
    {
        // 验证必要的组件
        if (bossSpawner == null)
        {
            Debug.LogError("BossDefeatedPallrexHandler: BossSpawner is not assigned!");
            return;
        }
        
        if (pallrexScript == null)
        {
            Debug.LogError("BossDefeatedPallrexHandler: pallrexScript is not assigned!");
            return;
        }
        
        // 订阅Boss被打败事件
        bossSpawner.OnBossDefeated += HandleBossDefeated;
        
        if (showDebugInfo)
        {
            Debug.Log("BossDefeatedPallrexHandler initialized and waiting for Boss defeated event.");
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件（避免内存泄漏）
        if (bossSpawner != null)
        {
            bossSpawner.OnBossDefeated -= HandleBossDefeated;
        }
    }
    #endregion
    
    #region 事件处理
    /// <summary>
    /// 处理Boss被打败事件
    /// </summary>
    private void HandleBossDefeated()
    {
        if (pallrexScript == null)
        {
            Debug.LogError("BossDefeatedPallrexHandler: pallrexScript is null when handling Boss defeated event!");
            return;
        }
        
        // 将pallrex的sign属性设置为true
        pallrexScript.sign = true;
        
        if (showDebugInfo)
        {
            Debug.Log("BossDefeatedPallrexHandler: Boss defeated event received, set pallrex.sign to true.");
        }
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制连接线
        if (bossSpawner != null && pallrexScript != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(bossSpawner.transform.position, pallrexScript.transform.position);
        }
        
        // 绘制组件信息标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string handlerInfo = "Boss Defeated -> Pallrex Handler";
        handlerInfo += "\nBoss Spawner: " + (bossSpawner != null ? bossSpawner.name : "None");
        handlerInfo += "\nPallrex: " + (pallrexScript != null ? pallrexScript.name : "None");
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2.0f, handlerInfo, style);
    }
    #endregion
}