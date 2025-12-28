using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas淡入器
/// 功能：接收事件并实现Canvas的淡入效果
/// </summary>
public class CanvasFader : MonoBehaviour
{
    [Header("核心设置")]
    [SerializeField] private TriggerEventManager triggerManager; // 触发器事件管理器
    [SerializeField] private Canvas targetCanvas; // 目标Canvas
    [SerializeField] private float fadeInSpeed = 1f; // 淡入速度
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息

    private CanvasGroup canvasGroup; // Canvas组组件
    private bool isFading = false; // 是否正在淡入
    private bool hasFadedIn = false; // 是否已完成淡入

    #region 生命周期方法
    private void Start()
    {
        // 验证必要的组件
        if (triggerManager == null)
        {
            Debug.LogError("CanvasFader: triggerManager is not assigned!");
            return;
        }

        if (targetCanvas == null)
        {
            Debug.LogError("CanvasFader: targetCanvas is not assigned!");
            return;
        }

        // 获取或创建CanvasGroup
        canvasGroup = targetCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetCanvas.gameObject.AddComponent<CanvasGroup>();
        }

        // 初始状态：隐藏Canvas
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // 订阅事件
        triggerManager.OnCanvasFadeIn += StartFadeIn;

        if (showDebugInfo)
        {
            Debug.Log("CanvasFader initialized.");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (triggerManager != null)
        {
            triggerManager.OnCanvasFadeIn -= StartFadeIn;
        }
    }
    #endregion

    #region 核心功能
    /// <summary>
    /// 开始淡入效果
    /// </summary>
    public void StartFadeIn()
    {
        if (isFading || hasFadedIn)
        {
            return;
        }

        isFading = true;
        targetCanvas.gameObject.SetActive(true);

        StartCoroutine(FadeInCoroutine());

        if (showDebugInfo)
        {
            Debug.Log("CanvasFader: Starting fade-in effect.");
        }
    }

    /// <summary>
    /// 淡入协程
    /// </summary>
    private System.Collections.IEnumerator FadeInCoroutine()
    {
        float currentTime = 0f;

        while (currentTime < 1f)
        {
            currentTime += Time.deltaTime * fadeInSpeed;
            currentTime = Mathf.Clamp01(currentTime);

            // 更新Canvas透明度
            canvasGroup.alpha = currentTime;

            // 当透明度达到一定阈值时，启用交互
            if (currentTime > 0.5f && !canvasGroup.interactable)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            yield return null;
        }

        // 完成淡入
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        isFading = false;
        hasFadedIn = true;

        if (showDebugInfo)
        {
            Debug.Log("CanvasFader: Fade-in effect completed.");
        }
    }

    /// <summary>
    /// 重置Canvas状态
    /// </summary>
    public void ResetCanvas()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isFading = false;
        hasFadedIn = false;

        targetCanvas.gameObject.SetActive(false);
    }
    #endregion

    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制与触发器管理器的连接线
        if (triggerManager != null)
        {
            Gizmos.color = hasFadedIn ? Color.magenta : Color.cyan;
            Gizmos.DrawLine(transform.position, triggerManager.transform.position);
        }

        // 绘制Canvas信息
        if (targetCanvas != null)
        {
            Gizmos.color = new Color(1, 1, 1, canvasGroup != null ? canvasGroup.alpha : 0f);
            Vector3 canvasPosition = targetCanvas.transform.position;
            Vector3 canvasSize = new Vector3(2f, 2f, 0.1f); // 可视化大小
            Gizmos.DrawCube(canvasPosition, canvasSize);
        }

        // 绘制标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;

        string canvasInfo = "Canvas Fader";
        canvasInfo += "\nStatus: " + (hasFadedIn ? "Faded In" : (isFading ? "Fading" : "Ready"));
        canvasInfo += "\nSpeed: " + fadeInSpeed;

        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, canvasInfo, style);
    }
    #endregion
}