using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 传送门脚本
/// 功能：当玩家进入传送门区域时，将玩家传送到目标位置
/// </summary>
public class TeleportPortal : MonoBehaviour
{
    [Header("核心参数")]
    [SerializeField] private string playerTag = "Player"; // 玩家的标签
    [SerializeField] private Transform targetPosition; // 传送目标位置
    [SerializeField] private bool requirePlayerInput = false; // 是否需要玩家输入才能触发传送
    [SerializeField] private KeyCode teleportKey = KeyCode.E; // 触发传送的按键
    [SerializeField] private float teleportCooldown = 1f; // 传送冷却时间（秒）
    [SerializeField] private float detectionWidth = 3f; // 检测范围宽度
    [SerializeField] private float detectionHeight = 1.5f; // 检测范围高度
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    [Header("视觉效果")]
    [SerializeField] private Color portalColor = new Color(0, 0.5f, 1, 0.3f); // 传送门颜色
    [SerializeField] private Color targetColor = new Color(1, 0.5f, 0, 0.3f); // 目标位置颜色
    
    private bool isPlayerInRange = false; // 玩家是否在传送门范围内
    private float lastTeleportTime = -Mathf.Infinity; // 上次传送时间
    private GameObject detectedPlayer; // 检测到的玩家对象
    
    public System.Action<GameObject> OnPlayerTeleported; // 玩家被传送事件
    
    #region 生命周期方法
    private void Awake()
    {
        // 确保传送门有碰撞体
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
        collider.size = new Vector2(detectionWidth, detectionHeight);
    }
    
    private void Update()
    {
        // 检查是否需要输入才能传送
        if (requirePlayerInput && isPlayerInRange && detectedPlayer != null)
        {
            if (Input.GetKeyDown(teleportKey) && Time.time >= lastTeleportTime + teleportCooldown)
            {
                TeleportPlayer(detectedPlayer);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            detectedPlayer = other.gameObject;
            
            // 如果不需要输入，直接传送
            if (!requirePlayerInput && Time.time >= lastTeleportTime + teleportCooldown)
            {
                TeleportPlayer(detectedPlayer);
            }
            else if (requirePlayerInput && showDebugInfo)
            {
                Debug.Log("Press " + teleportKey + " to teleport!");
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            detectedPlayer = null;
        }
    }
    #endregion
    
    #region 核心功能
    /// <summary>
    /// 传送玩家到目标位置
    /// </summary>
    /// <param name="player">要传送的玩家</param>
    private void TeleportPlayer(GameObject player)
    {
        if (targetPosition == null)
        {
            Debug.LogError("Target position not set for TeleportPortal!");
            return;
        }
        
        // 执行传送
        player.transform.position = targetPosition.position;
        lastTeleportTime = Time.time;
        
        // 触发传送事件
        OnPlayerTeleported?.Invoke(player);
        
        if (showDebugInfo)
        {
            Debug.Log("Player teleported from " + transform.position + " to " + targetPosition.position);
        }
    }
    
    /// <summary>
    /// 设置传送目标位置
    /// </summary>
    /// <param name="newTarget">新的目标位置</param>
    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
    }
    
    /// <summary>
    /// 设置是否需要玩家输入
    /// </summary>
    /// <param name="requireInput">是否需要输入</param>
    public void SetRequireInput(bool requireInput)
    {
        requirePlayerInput = requireInput;
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制传送门范围
        Gizmos.color = portalColor;
        Vector3 portalSize = new Vector3(detectionWidth, detectionHeight, 0.1f);
        Gizmos.DrawCube(transform.position, portalSize);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, portalSize);
        
        // 绘制目标位置
        if (targetPosition != null)
        {
            // 绘制传送门到目标位置的连接线
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPosition.position);
            
            // 绘制目标位置标记
            Gizmos.color = targetColor;
            Gizmos.DrawCube(targetPosition.position, new Vector3(detectionWidth, detectionHeight, 0.1f));
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(targetPosition.position, new Vector3(detectionWidth, detectionHeight, 0.1f));
            
            // 绘制目标位置箭头
            DrawArrow(transform.position, targetPosition.position, Color.cyan);
        }
        
        // 绘制传送门标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string portalInfo = "Teleport Portal";
        portalInfo += "\nWidth: " + detectionWidth;
        portalInfo += "\nHeight: " + detectionHeight;
        portalInfo += "\nInput: " + (requirePlayerInput ? teleportKey.ToString() : "Auto");
        portalInfo += "\nCooldown: " + teleportCooldown + "s";
        
        if (targetPosition != null)
        {
            portalInfo += "\nTarget: Set";
        }
        else
        {
            portalInfo += "\nTarget: Not Set";
        }
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * (detectionHeight / 2 + 0.5f), portalInfo, style);
    }
    
    /// <summary>
    /// 绘制箭头辅助函数
    /// </summary>
    private void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        Gizmos.color = color;
        Vector3 direction = end - start;
        Gizmos.DrawLine(start, end);
        
        // 绘制箭头头部
        Vector3 arrowSize = new Vector3(0.2f, 0.2f, 0);
        Quaternion arrowRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
        
        Gizmos.matrix = Matrix4x4.TRS(end, arrowRotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, arrowSize);
        Gizmos.matrix = Matrix4x4.identity;
    }
    #endregion
}