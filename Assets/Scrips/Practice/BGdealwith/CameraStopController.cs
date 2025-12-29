using UnityEngine;
using Cinemachine;

/// <summary>
/// 相机停止控制器
/// 控制Cinemachine相机到达特定位置时停止跟随玩家
/// </summary>
public class CameraStopController : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform stopPositionMarker; // 停止位置标记对象
    
    [Header("停止参数")]
    [SerializeField] private float stopDistanceThreshold = 0.5f; // 触发停止的距离阈值
    [SerializeField] private bool useCameraPosition = true;      // 使用相机位置检测
    [SerializeField] private bool usePlayerPosition = false;      // 使用玩家位置检测
    [SerializeField] private bool returnToFollow = true;          // 是否在离开区域后恢复跟随
    
    [Header("轴向控制")]
    [SerializeField] private bool stopXAxis = true;               // 停止X轴移动
    [SerializeField] private bool stopYAxis = true;               // 停止Y轴移动
    [SerializeField] private bool stopZAxis = false;              // 停止Z轴移动
    
    [Header("调试")]
    [SerializeField] private bool showDebugLogs = false;          // 显示调试日志
    
    private bool isCameraStopped = false;
    private Vector3 stopPosition;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private bool hasInitialized = false;
    
    private void Start()
    {
        // 初始化停止位置
        UpdateStopPosition();
        
        // 自动获取组件引用
        InitializeComponents();
        
        hasInitialized = true;
    }
    
    /// <summary>
    /// 更新停止位置
    /// </summary>
    private void UpdateStopPosition()
    {
        if (stopPositionMarker != null)
        {
            stopPosition = stopPositionMarker.position;
        }
        else
        {
            // 如果没有指定标记对象，使用当前物体位置
            stopPosition = transform.position;
        }
    }
    
    private void InitializeComponents()
    {
        // 自动获取虚拟相机
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("未找到CinemachineVirtualCamera组件");
                return;
            }
        }
        
        // 自动获取玩家
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("未找到标签为Player的游戏对象");
            }
        }
        
        // 获取相机噪声组件（如果有）
        if (virtualCamera != null)
        {
            cameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }
    
    private void Update()
    {
        if (!hasInitialized || virtualCamera == null)
        {
            return;
        }
        
        // 实时更新停止位置
        UpdateStopPosition();
        
        CheckCameraStatus();
    }
    
    private void CheckCameraStatus()
    {
        bool shouldStop = ShouldStopCamera();
        bool shouldResume = ShouldResumeFollow();
        
        if (shouldStop && !isCameraStopped)
        {
            StopCamera();
        }
        else if (shouldResume && isCameraStopped)
        {
            ResumeCameraFollow();
        }
    }
    
    private bool ShouldStopCamera()
    {
        // 如果没有启用任何位置检测，不停止相机
        if (!useCameraPosition && !usePlayerPosition)
        {
            return false;
        }
        
        bool cameraInRange = false;
        bool playerInRange = false;
        
        // 检查相机位置
        if (useCameraPosition)
        {
            Vector3 cameraPos = virtualCamera.transform.position;
            float distance = Vector3.Distance(cameraPos, stopPosition);
            cameraInRange = distance < stopDistanceThreshold;
        }
        
        // 检查玩家位置
        if (usePlayerPosition && playerTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, stopPosition);
            playerInRange = distance < stopDistanceThreshold;
        }
        
        // 如果启用了相机位置检测，使用相机检测结果
        // 否则使用玩家位置检测结果
        return useCameraPosition ? cameraInRange : playerInRange;
    }
    
    private bool ShouldResumeFollow()
    {
        if (!returnToFollow || !isCameraStopped)
        {
            return false;
        }
        
        // 检查是否应该恢复跟随
        // 使用玩家位置而不是相机位置，因为相机位置被固定了
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(playerTransform.position, stopPosition);
            
            // 当玩家离开停止区域一定距离后恢复跟随
            return distance > stopDistanceThreshold * 2.0f;
        }
        
        return false;
    }
    
    private void StopCamera()
    {
        isCameraStopped = true;
        
        // 禁用相机噪声（如果有）
        if (cameraNoise != null)
        {
            cameraNoise.m_AmplitudeGain = 0f;
        }
        
        if (showDebugLogs)
        {
            Debug.Log("相机已停止");
        }
    }
    
    private void ResumeCameraFollow()
    {
        isCameraStopped = false;
        
        // 恢复相机噪声（如果有）
        if (cameraNoise != null)
        {
            cameraNoise.m_AmplitudeGain = 1f;
        }
        
        if (showDebugLogs)
        {
            Debug.Log("相机已恢复跟随");
        }
    }
    
    private void LateUpdate()
    {
        // 当相机停止时，固定特定轴向的位置
        if (isCameraStopped && virtualCamera != null)
        {
            Vector3 newPosition = virtualCamera.transform.position;
            
            if (stopXAxis)
            {
                newPosition.x = stopPosition.x;
            }
            if (stopYAxis)
            {
                newPosition.y = stopPosition.y;
            }
            if (stopZAxis)
            {
                newPosition.z = stopPosition.z;
            }
            
            // 应用固定后的位置
            virtualCamera.transform.position = newPosition;
        }
    }
    
    #region 公共方法
    /// <summary>
    /// 手动停止相机
    /// </summary>
    public void ManualStopCamera()
    {
        if (!isCameraStopped)
        {
            StopCamera();
        }
    }
    
    /// <summary>
    /// 手动恢复相机跟随
    /// </summary>
    public void ManualResumeFollow()
    {
        if (isCameraStopped)
        {
            ResumeCameraFollow();
        }
    }
    
    /// <summary>
    /// 设置新的停止位置
    /// </summary>
    /// <param name="newStopPosition">新的停止位置</param>
    public void SetStopPosition(Vector3 newStopPosition)
    {
        stopPosition = newStopPosition;
    }
    #endregion
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 更新停止位置（编辑器中也能实时更新）
        Vector3 currentStopPosition = stopPosition;
        if (stopPositionMarker != null)
        {
            currentStopPosition = stopPositionMarker.position;
        }
        else
        {
            currentStopPosition = transform.position;
        }
        
        // 绘制停止位置标记
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentStopPosition, 0.3f);
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(currentStopPosition, 0.2f);
        
        // 绘制停止位置标记对象的连线
        if (stopPositionMarker != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, stopPositionMarker.position);
            
            // 绘制标记对象的名称
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.magenta;
            style.fontSize = 10;
            UnityEditor.Handles.Label((transform.position + stopPositionMarker.position) / 2, "Stop Marker", style);
        }
        
        // 绘制停止触发区域
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Vector3 stopAreaSize = new Vector3(stopDistanceThreshold * 2, stopDistanceThreshold * 2, 1);
        Gizmos.DrawCube(currentStopPosition, stopAreaSize);
        
        // 绘制恢复跟随区域
        if (returnToFollow)
        {
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Vector3 returnAreaSize = new Vector3(stopDistanceThreshold * 4, stopDistanceThreshold * 4, 1);
            Gizmos.DrawWireCube(currentStopPosition, returnAreaSize);
        }
        
        // 绘制轴向限制指示
        float axisArrowLength = 0.5f;
        float axisArrowWidth = 0.1f;
        
        if (stopXAxis)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(currentStopPosition, Vector3.right * axisArrowLength);
            Gizmos.DrawRay(currentStopPosition, Vector3.left * axisArrowLength);
            
            // 绘制箭头
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.ArrowHandleCap(0, currentStopPosition + Vector3.right * axisArrowLength, Quaternion.LookRotation(Vector3.right), axisArrowWidth, EventType.Repaint);
            UnityEditor.Handles.ArrowHandleCap(0, currentStopPosition + Vector3.left * axisArrowLength, Quaternion.LookRotation(Vector3.left), axisArrowWidth, EventType.Repaint);
            
            // 绘制X轴标签
            GUIStyle xStyle = new GUIStyle();
            xStyle.normal.textColor = Color.blue;
            xStyle.fontSize = 10;
            UnityEditor.Handles.Label(currentStopPosition + Vector3.right * (axisArrowLength + 0.1f), "X", xStyle);
        }
        
        if (stopYAxis)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(currentStopPosition, Vector3.up * axisArrowLength);
            Gizmos.DrawRay(currentStopPosition, Vector3.down * axisArrowLength);
            
            // 绘制箭头
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.ArrowHandleCap(0, currentStopPosition + Vector3.up * axisArrowLength, Quaternion.LookRotation(Vector3.up), axisArrowWidth, EventType.Repaint);
            UnityEditor.Handles.ArrowHandleCap(0, currentStopPosition + Vector3.down * axisArrowLength, Quaternion.LookRotation(Vector3.down), axisArrowWidth, EventType.Repaint);
            
            // 绘制Y轴标签
            GUIStyle yStyle = new GUIStyle();
            yStyle.normal.textColor = Color.green;
            yStyle.fontSize = 10;
            UnityEditor.Handles.Label(currentStopPosition + Vector3.up * (axisArrowLength + 0.1f), "Y", yStyle);
        }
        
        if (stopZAxis)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(currentStopPosition, Vector3.forward * axisArrowLength);
            Gizmos.DrawRay(currentStopPosition, Vector3.back * axisArrowLength);
            
            // 绘制箭头
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.ArrowHandleCap(0, currentStopPosition + Vector3.forward * axisArrowLength, Quaternion.LookRotation(Vector3.forward), axisArrowWidth, EventType.Repaint);
            UnityEditor.Handles.ArrowHandleCap(0, currentStopPosition + Vector3.back * axisArrowLength, Quaternion.LookRotation(Vector3.back), axisArrowWidth, EventType.Repaint);
            
            // 绘制Z轴标签
            GUIStyle zStyle = new GUIStyle();
            zStyle.normal.textColor = Color.yellow;
            zStyle.fontSize = 10;
            UnityEditor.Handles.Label(currentStopPosition + Vector3.forward * (axisArrowLength + 0.1f), "Z", zStyle);
        }
        
        // 绘制相机当前位置
        if (virtualCamera != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(virtualCamera.transform.position, 0.3f);
            
            // 绘制相机到停止位置的连线
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(virtualCamera.transform.position, currentStopPosition);
            
            // 绘制相机方向
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.ArrowHandleCap(0, virtualCamera.transform.position, virtualCamera.transform.rotation, 0.5f, EventType.Repaint);
        }
        
        // 绘制玩家位置
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, 0.3f);
        }
        
        // 绘制功能标签
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 10;
        labelStyle.fontStyle = FontStyle.Bold;
        UnityEditor.Handles.Label(currentStopPosition + Vector3.up * 0.5f, "Camera Stop Point", labelStyle);
    }
#endif
}