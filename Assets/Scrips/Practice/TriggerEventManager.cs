using UnityEngine;

/// <summary>
/// 触发器事件管理器
/// 功能：检测玩家进入触发器并触发一系列事件
/// </summary>
public class TriggerEventManager : MonoBehaviour
{
    [Header("核心设置")]
    [SerializeField] private string playerTag = "Player"; // 玩家标签
    [SerializeField] private bool triggerOnlyOnce = true; // 是否只触发一次
    [SerializeField] private float delayBetweenEvents = 0.5f; // 事件之间的延迟时间
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息

    [Header("目标对象")]
    [SerializeField] private GameObject targetObject; // 要操作的物体
    [SerializeField] private Renderer targetImageRenderer; // 要操作的图片渲染器
    [SerializeField] private BossSpawner bossSpawner; // Boss生成器

    private bool hasTriggered = false; // 是否已触发
    private BoxCollider2D triggerCollider; // 触发器碰撞体

    // 定义事件
    public System.Action OnTriggerActivated; // 触发器被激活事件
    public System.Action OnKeyboardDisabled; // 键盘被禁用事件
    public System.Action<GameObject, Renderer> OnTransparencyEnabled; // 透明度被启用事件
    public System.Action OnCanvasFadeIn; // Canvas淡入事件
    public System.Action OnBossSpawned; // Boss生成事件

    #region 生命周期方法
    private void Awake()
    {
        // 获取或创建触发器碰撞体
        triggerCollider = GetComponent<BoxCollider2D>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        triggerCollider.isTrigger = true;
    }

    private void Start()
    {
        // 验证必要的组件
        if (targetObject == null)
        {
            Debug.LogWarning("TriggerEventManager: targetObject is not assigned!");
        }

        if (targetImageRenderer == null)
        {
            Debug.LogWarning("TriggerEventManager: targetImageRenderer is not assigned!");
        }

        if (bossSpawner != null)
        {
            // 订阅自己的OnBossSpawned事件，触发Boss生成
            OnBossSpawned += bossSpawner.SpawnBossFromEvent;
        }
        else
        {
            Debug.LogWarning("TriggerEventManager: bossSpawner is not assigned!");
        }

        if (showDebugInfo)
        {
            Debug.Log("TriggerEventManager initialized and waiting for player.");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件，避免内存泄漏
        if (bossSpawner != null)
        {
            OnBossSpawned -= bossSpawner.SpawnBossFromEvent;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            if (showDebugInfo)
            {
                Debug.Log("TriggerEventManager: Player entered trigger area.");
            }

            // 标记为已触发
            hasTriggered = triggerOnlyOnce;

            // 触发事件序列
            StartCoroutine(TriggerEventSequence());
        }
    }
    #endregion

    #region 核心功能
    /// <summary>
    /// 触发事件序列
    /// </summary>
    private System.Collections.IEnumerator TriggerEventSequence()
    {
        // 触发触发器激活事件
        OnTriggerActivated?.Invoke();
        yield return new WaitForSeconds(delayBetweenEvents);

        // 触发键盘禁用事件
        OnKeyboardDisabled?.Invoke();
        yield return new WaitForSeconds(delayBetweenEvents);

        // 触发透明度启用事件
        OnTransparencyEnabled?.Invoke(targetObject, targetImageRenderer);
        yield return new WaitForSeconds(delayBetweenEvents);

        // 触发Canvas淡入事件
        OnCanvasFadeIn?.Invoke();
        yield return new WaitForSeconds(delayBetweenEvents);

        // 触发Boss生成事件
        OnBossSpawned?.Invoke();
    }

    /// <summary>
    /// 重置触发器
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
    #endregion
#if UNITY_EDITOR
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制触发器范围
        if (triggerCollider != null)
        {
            Gizmos.color = hasTriggered ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 0, 0.5f);
            Vector3 size = new Vector3(triggerCollider.size.x * transform.localScale.x, 
                                      triggerCollider.size.y * transform.localScale.y, 0.1f);
            Gizmos.DrawCube(transform.position, size);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, size);
        }

        // 绘制目标对象连接线
        if (targetObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetObject.transform.position);
        }

        if (targetImageRenderer != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetImageRenderer.transform.position);
        }

        // 绘制触发器信息
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;

        string triggerInfo = "Trigger Event Manager";
        triggerInfo += "\nPlayer Tag: " + playerTag;
        triggerInfo += "\nTrigger Once: " + triggerOnlyOnce;
        triggerInfo += "\nHas Triggered: " + hasTriggered;

        Vector3 labelPosition = transform.position + Vector3.up * (triggerCollider != null ? triggerCollider.size.y / 2 + 0.5f : 1f);
        UnityEditor.Handles.Label(labelPosition, triggerInfo, style);
    }
    #endregion
#endif
}