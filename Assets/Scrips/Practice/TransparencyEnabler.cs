using UnityEngine;

/// <summary>
/// 透明度启用器
/// 功能：接收事件并将物体和图片透明度调为1
/// </summary>
public class TransparencyEnabler : MonoBehaviour
{
    [Header("核心设置")]
    [SerializeField] private TriggerEventManager triggerManager; // 触发器事件管理器
    [SerializeField] private float fadeInSpeed = 1f; // 淡入速度
    [SerializeField] private bool affectChildren = true; // 是否影响子物体
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息

    private bool hasEnabledTransparency = false; // 是否已启用透明度

    #region 生命周期方法
    private void Start()
    {
        // 验证必要的组件
        if (triggerManager == null)
        {
            Debug.LogError("TransparencyEnabler: triggerManager is not assigned!");
            return;
        }

        // 订阅事件
        triggerManager.OnTransparencyEnabled += EnableTransparency;

        if (showDebugInfo)
        {
            Debug.Log("TransparencyEnabler initialized.");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (triggerManager != null)
        {
            triggerManager.OnTransparencyEnabled -= EnableTransparency;
        }
    }
    #endregion

    #region 核心功能
    /// <summary>
    /// 启用透明度
    /// </summary>
    /// <param name="targetObject">目标物体</param>
    /// <param name="targetImage">目标图片渲染器</param>
    public void EnableTransparency(GameObject targetObject, Renderer targetImage)
    {
        if (hasEnabledTransparency)
        {
            return;
        }

        hasEnabledTransparency = true;

        // 处理目标物体
        if (targetObject != null)
        {
            Renderer[] renderers;
            if (affectChildren)
            {
                renderers = targetObject.GetComponentsInChildren<Renderer>(true);
            }
            else
            {
                Renderer renderer = targetObject.GetComponent<Renderer>();
                renderers = renderer != null ? new Renderer[] { renderer } : new Renderer[0];
            }

            StartCoroutine(FadeInRenderers(renderers));
        }

        // 处理目标图片
        if (targetImage != null)
        {
            StartCoroutine(FadeInRenderer(targetImage));
        }

        if (showDebugInfo)
        {
            Debug.Log("TransparencyEnabler: Transparency enabled for target objects.");
        }
    }

    /// <summary>
    /// 淡入多个渲染器
    /// </summary>
    private System.Collections.IEnumerator FadeInRenderers(Renderer[] renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            yield return StartCoroutine(FadeInRenderer(renderer));
        }
    }

    /// <summary>
    /// 淡入单个渲染器
    /// </summary>
    private System.Collections.IEnumerator FadeInRenderer(Renderer renderer)
    {
        if (renderer == null || renderer.material == null)
        {
            yield break;
        }

        // 获取原始颜色
        Color originalColor = renderer.material.color;
        originalColor.a = 0f;
        renderer.material.color = originalColor;
        renderer.enabled = true;

        // 淡入效果
        float currentTime = 0f;
        while (currentTime < 1f)
        {
            currentTime += Time.deltaTime * fadeInSpeed;
            currentTime = Mathf.Clamp01(currentTime);

            Color newColor = originalColor;
            newColor.a = currentTime;
            renderer.material.color = newColor;

            yield return null;
        }
    }
    #endregion

    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制与触发器管理器的连接线
        if (triggerManager != null)
        {
            Gizmos.color = hasEnabledTransparency ? Color.blue : Color.white;
            Gizmos.DrawLine(transform.position, triggerManager.transform.position);
        }

        // 绘制透明度启用器信息
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;

        string transparencyInfo = "Transparency Enabler";
        transparencyInfo += "\nHas Enabled: " + hasEnabledTransparency;
        transparencyInfo += "\nFade Speed: " + fadeInSpeed;
        transparencyInfo += "\nAffect Children: " + affectChildren;

        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, transparencyInfo, style);
    }
    #endregion
}