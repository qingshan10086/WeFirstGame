using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 传送目标位置标记脚本
/// 功能：作为传送门的目标位置标记，提供可视化指示
/// </summary>
public class TeleportTarget : MonoBehaviour
{
    [Header("核心参数")]
    [SerializeField] private string targetName = "Teleport Target";
    [SerializeField] private float targetSize = 1f;
    
    [Header("视觉效果")]
    [SerializeField] private Color targetColor = new Color(1, 0.5f, 0, 0.5f);
    
    #region 生命周期方法
    private void Awake()
    {
        // 确保目标位置有合适的名称
        if (string.IsNullOrEmpty(gameObject.name))
        {
            gameObject.name = targetName;
        }
        
        // 确保目标位置有合适的层级
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制目标位置标记
        Gizmos.color = targetColor;
        Vector3 targetSizeVector = new Vector3(targetSize, targetSize, 0.1f);
        Gizmos.DrawCube(transform.position, targetSizeVector);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, targetSizeVector);
        
        // 绘制十字准线
        Gizmos.color = Color.white;
        float crossSize = targetSize * 1.2f;
        Gizmos.DrawLine(transform.position - new Vector3(crossSize, 0, 0), transform.position + new Vector3(crossSize, 0, 0));
        Gizmos.DrawLine(transform.position - new Vector3(0, crossSize, 0), transform.position + new Vector3(0, crossSize, 0));
        
        // 绘制目标信息标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string targetInfo = targetName;
        targetInfo += "\nPosition: " + transform.position.ToString("F2");
        targetInfo += "\nSize: " + targetSize;
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * (targetSize / 2 + 0.5f), targetInfo, style);
    }
    #endregion
}