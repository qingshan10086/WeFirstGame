using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// doorTown脚本
/// 功能：订阅Boss死亡事件，实现门的打开动画
/// </summary>
public class doorTown : MonoBehaviour
{
    [Header("Boss事件订阅")]
    [SerializeField] private BossSpawner bossSpawner; // Boss生成器引用
    
    [Header("门类型")]
    [SerializeField] private DoorType doorType = DoorType.Slide; // 门的类型
    
    [Header("滑动门设置")]
    [SerializeField] private Vector3 slideOffset; // 滑动偏移量（相对于初始位置）
    [SerializeField] private float slideSpeed = 2f; // 滑动速度
    
    [Header("旋转门设置")]
    [SerializeField] private Vector3 rotationEndEulerAngles; // 旋转结束角度
    [SerializeField] private float rotationSpeed = 90f; // 旋转速度（度/秒）
    
    [Header("缩放门设置")]
    [SerializeField] private Vector3 scaleEndSize; // 缩放结束大小
    [SerializeField] private float scaleSpeed = 2f; // 缩放速度
    
    [Header("其他设置")]
    [SerializeField] private bool disableColliderOnOpen = true; // 开门后是否禁用碰撞器
    [SerializeField] private float openDelay = 0.5f; // 开门延迟时间
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    private enum DoorType
    {
        Slide,   // 滑动门
        Rotate,  // 旋转门
        Scale,   // 缩放门
        Disable  // 直接禁用
    }
    
    private Vector3 initialPosition; // 门的初始位置
    private Quaternion initialRotation; // 门的初始旋转
    private Vector3 initialScale; // 门的初始缩放
    private bool isOpening = false; // 是否正在开门
    private bool isOpen = false; // 是否已经完全打开
    private float openTimer = 0f; // 开门延迟计时器
    private Collider2D doorCollider; // 门的碰撞器组件
    public OneTimeMechanism oneTimeMechanism;
    
    #region 生命周期方法
    private void Awake()
    {
        // 记录门的初始状态
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        
        // 获取门的碰撞器组件
        doorCollider = GetComponent<Collider2D>();
        oneTimeMechanism=GetComponent<OneTimeMechanism>();
    }
    
    private void Start()
    {
        // 验证必要的组件
        if (bossSpawner == null)
        {
            Debug.LogError("doorTown: BossSpawner is not assigned!");
            return;
        }
        
        // 订阅Boss被击败事件
        bossSpawner.OnBossDefeated += HandleBossDefeated;
        
        if (showDebugInfo)
        {
            Debug.Log("doorTown initialized and waiting for Boss defeat event.");
        }
        
    }
    
    private void Update()
    {
        // 如果门正在打开，执行开门动画
        if (isOpening && !isOpen)
        {
            // 检查开门延迟
            if (openTimer < openDelay)
            {
                openTimer += Time.deltaTime;
                return;
            }
            
            // 根据门的类型执行不同的开门动画
            switch (doorType)
            {
                case DoorType.Slide:
                    AnimateSlideDoor();
                    break;
                case DoorType.Rotate:
                    AnimateRotateDoor();
                    break;
                case DoorType.Scale:
                    AnimateScaleDoor();
                    break;
                case DoorType.Disable:
                    DisableDoor();
                    break;
            }
           
        }
        if (oneTimeMechanism.isTriggered)
        {
            this.gameObject.SetActive(false);
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
    
    #region 核心功能
    /// <summary>
    /// 处理Boss被击败事件
    /// </summary>
    private void HandleBossDefeated()
    {
        if (isOpening || isOpen)
        {
            return; // 避免重复处理
        }
        
        if (showDebugInfo)
        {
            Debug.Log("doorTown: Boss defeated, starting door opening animation.");
        }
        
        isOpening = true;
        oneTimeMechanism.isTriggered = true;
        // 如果是直接禁用类型，立即执行
        if (doorType == DoorType.Disable)
        {
            DisableDoor();
        }
    }
    
    /// <summary>
    /// 滑动门动画
    /// </summary>
    private void AnimateSlideDoor()
    {
        // 计算目标位置（初始位置 + 偏移量）
        Vector3 targetPosition = initialPosition + slideOffset;
        
        // 平滑移动门到目标位置
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);
        
        // 检查是否到达目标位置
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            CompleteDoorOpening();
        }
    }
    
    /// <summary>
    /// 旋转门动画
    /// </summary>
    private void AnimateRotateDoor()
    {
        // 计算目标旋转
        Quaternion targetRotation = Quaternion.Euler(rotationEndEulerAngles);
        
        // 平滑旋转门到目标角度
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        // 检查是否到达目标角度
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            CompleteDoorOpening();
        }
    }
    
    /// <summary>
    /// 缩放门动画
    /// </summary>
    private void AnimateScaleDoor()
    {
        // 平滑缩放到目标大小
        transform.localScale = Vector3.MoveTowards(transform.localScale, scaleEndSize, scaleSpeed * Time.deltaTime);
        
        // 检查是否到达目标大小
        if (Vector3.Distance(transform.localScale, scaleEndSize) < 0.01f)
        {
            CompleteDoorOpening();
        }
    }
    
    /// <summary>
    /// 直接禁用门
    /// </summary>
    private void DisableDoor()
    {
        // 禁用门的碰撞器
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        
        // 禁用门的渲染
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // 禁用门的所有子渲染器
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer childRenderer in childRenderers)
        {
            childRenderer.enabled = false;
        }
        
        CompleteDoorOpening();
    }
    
    /// <summary>
    /// 完成门的打开
    /// </summary>
    private void CompleteDoorOpening()
    {
        isOpen = true;
        isOpening = false;
        
        // 如果设置了禁用碰撞器，禁用门的碰撞器
        if (disableColliderOnOpen && doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        
        if (showDebugInfo)
        {
            Debug.Log("doorTown: Door has been successfully opened.");
        }
    }
    
    /// <summary>
    /// 重置门的状态
    /// </summary>
    public void ResetDoor()
    {
        // 重置位置、旋转和缩放
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        
        // 重置状态
        isOpening = false;
        isOpen = false;
        openTimer = 0f;
        
        // 启用碰撞器
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        
        // 启用渲染器
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        // 启用所有子渲染器
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer childRenderer in childRenderers)
        {
            childRenderer.enabled = true;
        }
        
        if (showDebugInfo)
        {
            Debug.Log("doorTown: Door has been reset to initial state.");
        }
    }
    #endregion
    
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 确保在非运行时也能正确获取初始位置
        Vector3 startPos = Application.isPlaying ? initialPosition : transform.position;
        Vector3 doorScale = transform.localScale;
        
        // 绘制门的初始位置
        Gizmos.color = new Color(0, 1, 0, 0.8f); // 半透明绿色
        Gizmos.DrawWireCube(startPos, doorScale);
        
        // 根据门的类型绘制不同的可视化效果
        switch (doorType)
        {
            case DoorType.Slide:
                // 计算终点位置（初始位置 + 偏移量）
                Vector3 endPos = startPos + slideOffset;
                
                // 绘制滑动路径
                Gizmos.color = new Color(0, 0, 1, 0.8f); // 半透明蓝色
                Gizmos.DrawLine(startPos, endPos);
                Gizmos.DrawWireCube(endPos, doorScale);
                
                // 绘制路径标记点
                Gizmos.DrawSphere(startPos, 0.2f);
                Gizmos.DrawSphere(endPos, 0.2f);
                break;
            case DoorType.Rotate:
                // 绘制旋转效果
                Gizmos.color = new Color(1, 1, 0, 0.8f); // 半透明黄色
                Gizmos.DrawWireCube(startPos, doorScale);
                
                // 绘制旋转轴
                Gizmos.color = new Color(1, 0, 1, 0.8f); // 半透明紫色
                Gizmos.DrawLine(startPos - Vector3.forward * 2, startPos + Vector3.forward * 2);
                break;
            case DoorType.Scale:
                // 绘制缩放效果
                Gizmos.color = new Color(1, 0.5f, 0, 0.8f); // 半透明橙色
                Gizmos.DrawWireCube(startPos, scaleEndSize);
                break;
            case DoorType.Disable:
                // 绘制禁用效果
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // 半透明灰色
                Gizmos.DrawWireCube(startPos, doorScale);
                break;
        }
        
        // 绘制门信息标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        style.normal.background = MakeTex(200, 20, new Color(0, 0, 0, 0.7f));
        
        string doorInfo = "doorTown";
        doorInfo += "\nType: " + doorType;
        doorInfo += "\nOpen: " + isOpen;
        
        if (doorType == DoorType.Slide)
        {
            // 计算终点位置（初始位置 + 偏移量）
            Vector3 endPos = startPos + slideOffset;
            doorInfo += "\nSlide Distance: " + Vector3.Distance(startPos, endPos).ToString("F2");
        }
        
        Vector3 labelPos = startPos + Vector3.up * (doorScale.y / 2 + 1.5f);
        UnityEditor.Handles.Label(labelPos, doorInfo, style);
    }
    
    /// <summary>
    /// 创建一个简单的纹理用于GUI背景
    /// </summary>
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    #endregion
}
