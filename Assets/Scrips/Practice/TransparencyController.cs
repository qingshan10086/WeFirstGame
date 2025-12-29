using UnityEngine;

/// <summary>
/// TransparencyController脚本
/// 功能：控制物体的透明度在指定范围内来回变化
/// </summary>
public class TransparencyController : MonoBehaviour
{
    [Header("透明度控制参数")]
    [SerializeField] private float minAlpha = 0.2f; // 最小透明度
    [SerializeField] private float maxAlpha = 1.0f; // 最大透明度
    [SerializeField] private float fadeSpeed = 1.0f; // 透明度变化速度
    [SerializeField] private bool startFading = true; // 是否在开始时自动开始淡入淡出
    [SerializeField] private bool usePingPong = true; // 是否使用来回变化（true）或单向循环（false）
    
    [Header("其他设置")]
    [SerializeField] private bool affectChildren = false; // 是否影响子物体
    [SerializeField] private bool showDebugInfo = false; // 是否显示调试信息
    
    private float currentAlpha; // 当前透明度
    private int fadeDirection = 1; // 淡入淡出方向（1为增加，-1为减少）
    private Renderer[] renderers; // 所有需要控制透明度的渲染器
    private Color[] originalColors; // 原始颜色
    
    #region 生命周期方法
    private void Awake()
    {
        // 获取所有需要控制透明度的渲染器
        if (affectChildren)
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }
        else
        {
            renderers = new Renderer[] { GetComponent<Renderer>() };
        }
        
        // 保存原始颜色
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                if (renderers[i].material != null)
                {
                    originalColors[i] = renderers[i].material.color;
                }
            }
        }
        
        // 初始化当前透明度
        currentAlpha = Mathf.Clamp(originalColors[0].a, minAlpha, maxAlpha);
    }
    
    private void Start()
    {
        if (showDebugInfo)
        {
            Debug.Log("TransparencyController initialized. Controlling " + renderers.Length + " renderers.");
        }
    }
    
    private void Update()
    {
        if (startFading)
        {
            UpdateTransparency();
        }
    }
    #endregion
    
    #region 核心功能
    /// <summary>
    /// 更新透明度
    /// </summary>
    private void UpdateTransparency()
    {
        // 更新当前透明度
        currentAlpha += fadeDirection * fadeSpeed * Time.deltaTime;
        
        // 检查是否到达边界
        if (currentAlpha >= maxAlpha)
        {
            currentAlpha = maxAlpha;
            if (usePingPong)
            {
                fadeDirection = -1;
            }
            else
            {
                currentAlpha = minAlpha;
            }
        }
        else if (currentAlpha <= minAlpha)
        {
            currentAlpha = minAlpha;
            if (usePingPong)
            {
                fadeDirection = 1;
            }
            else
            {
                currentAlpha = maxAlpha;
            }
        }
        
        // 应用透明度变化
        ApplyTransparency();
    }
    
    /// <summary>
    /// 应用透明度变化到所有渲染器
    /// </summary>
    private void ApplyTransparency()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                Color newColor = originalColors[i];
                newColor.a = currentAlpha;
                renderers[i].material.color = newColor;
            }
        }
    }
    
    /// <summary>
    /// 开始淡入淡出效果
    /// </summary>
    public void StartFading()
    {
        startFading = true;
        if (showDebugInfo)
        {
            Debug.Log("TransparencyController: Started fading.");
        }
    }
    
    /// <summary>
    /// 停止淡入淡出效果
    /// </summary>
    public void StopFading()
    {
        startFading = false;
        if (showDebugInfo)
        {
            Debug.Log("TransparencyController: Stopped fading.");
        }
    }
    
    /// <summary>
    /// 设置透明度范围
    /// </summary>
    /// <param name="min">最小透明度</param>
    /// <param name="max">最大透明度</param>
    public void SetAlphaRange(float min, float max)
    {
        minAlpha = Mathf.Clamp01(min);
        maxAlpha = Mathf.Clamp01(max);
        currentAlpha = Mathf.Clamp(currentAlpha, minAlpha, maxAlpha);
    }
    
    /// <summary>
    /// 设置淡入淡出速度
    /// </summary>
    /// <param name="speed">淡入淡出速度</param>
    public void SetFadeSpeed(float speed)
    {
        fadeSpeed = Mathf.Max(0.1f, speed);
    }
    #endregion
#if UNITY_EDITOR
    #region 编辑器可视化
    private void OnDrawGizmosSelected()
    {
        // 绘制一个半透明的立方体来表示当前透明度
        Gizmos.color = new Color(1, 1, 1, currentAlpha);
        Bounds bounds = GetComponent<Renderer>().bounds;
        Gizmos.DrawCube(bounds.center, bounds.size);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        // 绘制透明度范围标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        
        string debugText = "TransparencyController";
        debugText += "\nAlpha: " + currentAlpha.ToString("F2");
        debugText += "\nRange: " + minAlpha.ToString("F2") + " - " + maxAlpha.ToString("F2");
        debugText += "\nSpeed: " + fadeSpeed.ToString("F2");
        
        UnityEditor.Handles.Label(transform.position + Vector3.up * (bounds.extents.y + 0.5f), debugText, style);
    }
    #endregion
#endif
}