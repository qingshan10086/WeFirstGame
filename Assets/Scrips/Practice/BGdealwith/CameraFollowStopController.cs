


using Cinemachine;
using UnityEngine;

/// <summary>
/// 简单的相机跟随与停止控制器
/// 功能：玩家进入指定范围时，相机固定在特定位置
/// 支持多个控制器共存，通过优先级系统协调控制
/// </summary>
public class CameraFollowStopController : MonoBehaviour
{
    #region 静态变量（用于多控制器协调）
    private static CameraFollowStopController currentActiveController; // 当前正在控制相机的控制器
    private static CameraFollowStopController defaultController;        // 默认控制器（当没有控制器处于激活状态时）
    #endregion

    #region 核心参数
    [Header("核心参数")]
    [SerializeField] private Transform playerTransform;  // 玩家位置
    [SerializeField] private Camera mainCamera;          // 主相机
    [SerializeField] private float followSpeed = 5f;     // 相机跟随速度
    [SerializeField] private float fixedMoveSpeed = 3f;  // 相机移动到固定位置的速度
    [SerializeField] private Vector3 followOffset = new Vector3(0, 2, -10); // 跟随偏移量
    [SerializeField] private Vector2 stopRange = new Vector2(3f, 2f); // 停止范围（x宽度, y高度）
    [SerializeField] private Vector2 returnRange = new Vector2(5f, 4f); // 返回跟随范围（x宽度, y高度）
    [SerializeField] private Transform detectionRangeCenter; // 检测范围中心（如果未设置，将使用stopPositionTransform）
    [SerializeField] private Transform stopRangeCenter; // 停止范围中心（如果未设置，将使用detectionRangeCenter或stopPositionTransform）
    [SerializeField] private Transform returnRangeCenter; // 返回范围中心（如果未设置，将使用stopRangeCenter、detectionRangeCenter或stopPositionTransform）
    [SerializeField] private int priority = 0;           // 控制器优先级（数值越大优先级越高）
    #endregion
    [Header("停止位置")]
    [SerializeField] private Transform stopPositionTransform; // 相机固定位置
    [SerializeField] private bool lockXAxis = true; // 是否固定X轴位置
    [SerializeField] private bool lockYAxis = true; // 是否固定Y轴位置
    [Header("高级机器")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // 虚拟相机
    [Header("多控制器设置")]
    [SerializeField] private bool isDefaultController = false; // 是否为默认控制器（当没有控制器处于激活状态时）

    #region 状态参数
    public bool isCameraFixed = false; // 相机是否固定
    #endregion

    #region 生命周期方法
    private void Awake()
    {
        // 自动获取组件
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // 设置默认控制器
        if (isDefaultController)
        {
            defaultController = this;
        }
    }

    private void OnDestroy()
    {
        // 如果当前控制器被销毁，重置当前控制器
        if (currentActiveController == this)
        {
            currentActiveController = null;
        }

        // 如果默认控制器被销毁，重置默认控制器
        if (defaultController == this)
        {
            defaultController = null;
        }
    }

    private void Update()
    {
        if (playerTransform == null || mainCamera == null || stopPositionTransform == null)
        {
            return;
        }

        CheckPlayerPosition();

        // 控制相机逻辑
        bool shouldControlCamera = false;
        
        if (currentActiveController == this)
        {
            // 当前控制器正在控制相机
            shouldControlCamera = true;
        }
        else if (currentActiveController == null)
        {
            // 没有当前控制器，检查是否应该获取控制权
            if (isCameraFixed || ShouldTakeControl())
            {
                shouldControlCamera = true;
            }
            // 如果没有控制器控制且相机未固定，所有控制器都可以尝试跟随
            else if (!isCameraFixed)
            {
                // 只有默认控制器或没有其他控制器时才跟随
                shouldControlCamera = (defaultController == this) || (defaultController == null);
            }
        }

        if (shouldControlCamera)
        {
            if (isCameraFixed)
            {
                // 固定相机位置（根据选择的轴）
                Vector3 stopPos = stopPositionTransform.position;
                
                // 获取当前相机位置，用于非固定轴的位置保持
                Vector3 currentCameraPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
                if (virtualCamera != null && mainCamera == null)
                {
                    currentCameraPos = virtualCamera.transform.position;
                }
                
                // 构建目标停止位置
                float xPos = lockXAxis ? stopPos.x : currentCameraPos.x;
                float yPos = lockYAxis ? stopPos.y : currentCameraPos.y;
                float zPos = currentCameraPos.z; // 保持Z轴位置不变
                Vector3 targetStopPosition = new Vector3(xPos, yPos, zPos);
                
                if (virtualCamera != null)
                {
                    // 如果使用虚拟相机，应该使用虚拟相机的方法来控制位置
                    virtualCamera.Follow = null;
                    virtualCamera.LookAt = null;
                    // 平滑移动虚拟相机到目标位置（使用专门的固定移动速度）
                    virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, targetStopPosition, fixedMoveSpeed * Time.deltaTime);
                }
                
                if (mainCamera != null)
                {
                    // 平滑移动主相机到目标位置（使用专门的固定移动速度）
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetStopPosition, fixedMoveSpeed * Time.deltaTime);
                }
                
                // Debug.Log("相机固定在停止位置:"+ (mainCamera != null ? mainCamera.transform.position.ToString() : "相机未找到"));
            }
            else
            {
                // 跟随玩家
                FollowPlayer();
            }
        }
    }
    #endregion

    #region 核心功能
    private void CheckPlayerPosition()
    {
        // 计算停止范围中心：returnRangeCenter → stopRangeCenter → detectionRangeCenter → stopPositionTransform
        Transform stopCenter = stopRangeCenter != null ? stopRangeCenter : 
                               (detectionRangeCenter != null ? detectionRangeCenter : stopPositionTransform);
        
        // 计算返回范围中心：returnRangeCenter → stopRangeCenter → detectionRangeCenter → stopPositionTransform
        Transform returnCenter = returnRangeCenter != null ? returnRangeCenter : 
                                 (stopRangeCenter != null ? stopRangeCenter : 
                                 (detectionRangeCenter != null ? detectionRangeCenter : stopPositionTransform));
        
        Vector3 playerPos = playerTransform.position;
        
        // 检查玩家是否在停止范围内
        Vector3 stopCenterPos = stopCenter.position;
        float stopXDistance = Mathf.Abs(playerPos.x - stopCenterPos.x);
        float stopYDistance = Mathf.Abs(playerPos.y - stopCenterPos.y);
        bool inStopRange = stopXDistance <= stopRange.x / 2f && stopYDistance <= stopRange.y / 2f;
        
        // 检查玩家是否在返回范围外
        Vector3 returnCenterPos = returnCenter.position;
        float returnXDistance = Mathf.Abs(playerPos.x - returnCenterPos.x);
        float returnYDistance = Mathf.Abs(playerPos.y - returnCenterPos.y);
        bool outsideReturnRange = returnXDistance > returnRange.x / 2f || returnYDistance > returnRange.y / 2f;
        
        if (!isCameraFixed && inStopRange)
        {
            // 检查是否应该获取控制权
            if (ShouldTakeControl())
            {
                // 玩家进入停止范围，固定相机
                isCameraFixed = true;
                TakeControl(); // 获取控制权
            }
        }
       
        else if (isCameraFixed && outsideReturnRange)
        {
            // 玩家离开返回范围，恢复跟随
            isCameraFixed = false;
            ReleaseControl(); // 释放控制权
        }
    }

    /// <summary>
    /// 检查当前控制器是否应该获取相机控制权
    /// </summary>
    /// <returns>是否应该获取控制权</returns>
    private bool ShouldTakeControl()
    {
        // 如果没有当前控制器，或者当前控制器的优先级低于此控制器，则应该获取控制权
        return currentActiveController == null || 
               currentActiveController.priority < this.priority;
    }

    /// <summary>
    /// 获取相机控制权
    /// </summary>
    private void TakeControl()
    {
        if (currentActiveController != null)
        {
            // 如果当前有其他控制器正在控制，先让它释放控制权
            currentActiveController.ReleaseControl();
        }

        // 设置当前控制器为自己
        currentActiveController = this;
    }

    /// <summary>
    /// 释放相机控制权
    /// </summary>
    private void ReleaseControl()
    {
        if (currentActiveController == this)
        {
            currentActiveController = null;
        }

        // 重置当前控制器的状态
        isCameraFixed = false;
    }

    private void FollowPlayer()
    {
        // 计算目标位置
        Vector3 targetPosition = playerTransform.position + followOffset;
        
        // 平滑移动相机
        if (mainCamera != null)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        
        // 设置虚拟相机跟随（如果使用）
        if (virtualCamera != null)
        {
            if (virtualCamera.Follow == null)
            {
                virtualCamera.Follow = playerTransform;
            }
        }
    }
    #endregion

    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        if (stopPositionTransform == null)
        {
            return;
        }

        // 计算各范围中心
        Transform detectionCenter = detectionRangeCenter != null ? detectionRangeCenter : stopPositionTransform;
        Transform stopCenter = stopRangeCenter != null ? stopRangeCenter : detectionCenter;
        Transform returnCenter = returnRangeCenter != null ? returnRangeCenter : stopCenter;
        
        Vector3 detectionPos = detectionCenter.position;
        Vector3 stopCenterPos = stopCenter.position;
        Vector3 returnCenterPos = returnCenter.position;
        Vector3 stopPos = stopPositionTransform.position;

        // 绘制固定相机位置（红色）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(stopPos, 0.5f);
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(stopPos, 0.3f);
        
        // 绘制检测范围中心（蓝色）
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(detectionPos, 0.4f);
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(detectionPos, 0.2f);
        
        // 绘制停止范围中心（紫色）
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(stopCenterPos, 0.4f);
        Gizmos.color = new Color(1, 0, 1, 0.3f);
        Gizmos.DrawSphere(stopCenterPos, 0.2f);
        
        // 绘制返回范围中心（绿色）
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(returnCenterPos, 0.4f);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(returnCenterPos, 0.2f);
        
        // 绘制中心之间的连接线
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(detectionPos, stopCenterPos);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(stopCenterPos, returnCenterPos);
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(stopCenterPos, stopPos);

        // 绘制矩形范围
        float zRange = 0.1f; // 可视化时z轴方向的厚度
        
        // 绘制停止范围（红色矩形，围绕停止范围中心）
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Vector3 stopRangeSize = new Vector3(stopRange.x, stopRange.y, zRange);
        Gizmos.DrawCube(stopCenterPos, stopRangeSize);
        
        // 绘制停止范围边框
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(stopCenterPos, stopRangeSize);

        // 绘制返回范围（绿色矩形，围绕返回范围中心）
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Vector3 returnRangeSize = new Vector3(returnRange.x, returnRange.y, zRange);
        Gizmos.DrawCube(returnCenterPos, returnRangeSize);
        
        // 绘制返回范围边框
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(returnCenterPos, returnRangeSize);

        // 绘制x轴和y轴指示线
        // 停止范围轴线
        Gizmos.color = Color.red;
        float halfStopX = stopRange.x / 2f;
        float halfStopY = stopRange.y / 2f;
        Gizmos.DrawLine(stopCenterPos - new Vector3(halfStopX, 0, 0), stopCenterPos + new Vector3(halfStopX, 0, 0));
        Gizmos.DrawLine(stopCenterPos - new Vector3(0, halfStopY, 0), stopCenterPos + new Vector3(0, halfStopY, 0));
        
        // 返回范围轴线
        Gizmos.color = Color.green;
        float halfReturnX = returnRange.x / 2f;
        float halfReturnY = returnRange.y / 2f;
        Gizmos.DrawLine(returnCenterPos - new Vector3(halfReturnX, 0, 0), returnCenterPos + new Vector3(halfReturnX, 0, 0));
        Gizmos.DrawLine(returnCenterPos - new Vector3(0, halfReturnY, 0), returnCenterPos + new Vector3(0, halfReturnY, 0));

        // 绘制功能标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 10;
        style.fontStyle = FontStyle.Bold;

        // 绘制范围中心标签
        UnityEditor.Handles.Label(detectionPos + Vector3.up * 0.6f, "Detection Center", style);
        UnityEditor.Handles.Label(stopCenterPos + Vector3.up * 0.6f, "Stop Range Center", style);
        UnityEditor.Handles.Label(returnCenterPos + Vector3.up * 0.6f, "Return Range Center", style);
        
        // 绘制控制器信息标签
        string controllerInfo = "Camera Fixed Point";
        controllerInfo += "\nPriority: " + priority;
        controllerInfo += "\nDefault: " + isDefaultController;
        controllerInfo += "\nLocked Axes: " + (lockXAxis ? "X" : "") + (lockXAxis && lockYAxis ? "+" : "") + (lockYAxis ? "Y" : "");
        
        // 添加当前控制器状态信息
        #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            if (currentActiveController == this)
            {
                controllerInfo += "\n[ACTIVE]";
            }
        }
        #endif

        UnityEditor.Handles.Label(stopPos + Vector3.up * 1.5f, controllerInfo, style);

        // 绘制玩家与相机位置（如果存在）
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, 0.3f);
            UnityEditor.Handles.Label(playerTransform.position + Vector3.up * 0.6f, "Player", style);
        }

        if (mainCamera != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(mainCamera.transform.position, 0.3f);
            UnityEditor.Handles.Label(mainCamera.transform.position + Vector3.up * 0.6f, "Camera", style);
        }
    }
    #endregion
}