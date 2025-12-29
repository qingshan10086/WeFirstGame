using UnityEngine;

/// <summary>
/// 键盘禁用器
/// 功能：接收事件并禁用键盘输入
/// </summary>
public class KeyboardDisabler : MonoBehaviour
{
    [Header("核心设置")]
    [SerializeField] private TriggerEventManager triggerManager; // 触发器事件管理器
    [SerializeField] private bool reenableOnBossDeath = true; // 是否在Boss死亡时重新启用键盘
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息

    private bool isKeyboardDisabled = false; // 键盘是否被禁用

    public bool IsKeyboardDisabled => isKeyboardDisabled;

    #region 生命周期方法
    private void Start()
    {
        // 验证必要的组件
        if (triggerManager == null)
        {
            Debug.LogError("KeyboardDisabler: triggerManager is not assigned!");
            return;
        }

        // 订阅事件
        triggerManager.OnKeyboardDisabled += DisableKeyboard;

        // 如果需要在Boss死亡时重新启用键盘
        if (reenableOnBossDeath)
        {
            // 查找BossSpawner并订阅Boss死亡事件
            BossSpawner bossSpawner = FindObjectOfType<BossSpawner>();
            if (bossSpawner != null)
            {
                bossSpawner.OnBossDefeated += EnableKeyboard;
            }
        }

        if (showDebugInfo)
        {
            Debug.Log("KeyboardDisabler initialized.");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (triggerManager != null)
        {
            triggerManager.OnKeyboardDisabled -= DisableKeyboard;
        }

        // 取消订阅Boss死亡事件
        BossSpawner bossSpawner = FindObjectOfType<BossSpawner>();
        if (bossSpawner != null)
        {
            bossSpawner.OnBossDefeated -= EnableKeyboard;
        }
    }

    private void Update()
    {
        // 这里可以添加额外的逻辑来处理键盘禁用状态
    }
    #endregion

    #region 核心功能
    /// <summary>
    /// 禁用键盘输入
    /// </summary>
    public void DisableKeyboard()
    {
        isKeyboardDisabled = true;

        if (showDebugInfo)
        {
            Debug.Log("KeyboardDisabler: Keyboard disabled.");
        }
    }

    /// <summary>
    /// 启用键盘输入
    /// </summary>
    public void EnableKeyboard()
    {
        isKeyboardDisabled = false;

        if (showDebugInfo)
        {
            Debug.Log("KeyboardDisabler: Keyboard enabled.");
        }
    }

    /// <summary>
    /// 检查是否可以处理键盘输入
    /// 其他脚本应该调用这个方法来检查是否应该处理键盘输入
    /// </summary>
    /// <returns>是否可以处理键盘输入</returns>
    public bool CanProcessInput()
    {
        return !isKeyboardDisabled;
    }
    #endregion
#if UNITY_EDITOR
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制与触发器管理器的连接线
        if (triggerManager != null)
        {
            Gizmos.color = isKeyboardDisabled ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, triggerManager.transform.position);
        }

        // 绘制键盘状态标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;

        string keyboardInfo = "Keyboard Disabler";
        keyboardInfo += "\nStatus: " + (isKeyboardDisabled ? "Disabled" : "Enabled");
        keyboardInfo += "\nReenable on Boss Death: " + reenableOnBossDeath;

        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, keyboardInfo, style);
    }
    #endregion
#endif
}