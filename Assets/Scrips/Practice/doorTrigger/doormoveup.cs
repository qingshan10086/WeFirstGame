using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doormoveup : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("默认移动速度（单位/秒）")]
    public float defaultSpeed = 5f;
    
    [Header("移动目标标记设置")]
    [Tooltip("是否在场景中显示移动目标位置")]
    public bool showMoveTarget = true;
    
    [Tooltip("目标标记颜色")]
    public Color targetMarkerColor = Color.green;
    
    [Tooltip("移动路径颜色")]
    public Color pathColor = Color.yellow;
    
    [Tooltip("目标标记大小")]
    public float markerSize = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Coroutine currentMoveCoroutine = null;
    
    /// <summary>
    /// 让物体向上移动指定距离（瞬间移动）
    /// </summary>
    /// <param name="distance">向上移动的距离</param>
    public void MoveUp(float distance)
    {
        if (showMoveTarget)
        {
            // 计算目标位置并显示标记
            Vector3 targetPosition = transform.position + Vector3.up * distance;
            DrawTargetMarker(transform.position, targetPosition, distance);
        }
        
        transform.position += Vector3.up * distance;
    }
    
    /// <summary>
    /// 让物体缓慢向上移动指定距离（平滑移动）
    /// </summary>
    /// <param name="distance">向上移动的距离</param>
    /// <param name="speed">移动速度（单位/秒），如果不指定则使用默认值</param>
    public void MoveUpSmooth(float distance, float? speed = null)
    {
        if (showMoveTarget)
        {
            // 计算目标位置并显示标记
            Vector3 targetPosition = transform.position + Vector3.up * distance;
            DrawTargetMarker(transform.position, targetPosition, distance);
        }
        
        // 如果当前有移动协程正在运行，停止它
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }
        
        // 使用指定的speed或默认值
        float moveSpeed = speed.HasValue ? speed.Value : defaultSpeed;
        
        // 启动新的移动协程
        currentMoveCoroutine = StartCoroutine(MoveUpCoroutine(distance, moveSpeed));
    }
    
    /// <summary>
    /// 向上移动的协程
    /// </summary>
    /// <param name="distance">向上移动的距离</param>
    /// <param name="speed">移动速度（单位/秒）</param>
    private IEnumerator MoveUpCoroutine(float distance, float speed)
    {
        if (speed <= 0f)
        {
            Debug.LogWarning("移动速度必须大于0");
            yield break;
        }
        
        float remainingDistance = distance;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * distance;
        
        // 计算移动所需的总时间
        float totalTime = distance / speed;
        float elapsedTime = 0f;
        
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / totalTime);
            
            // 使用线性插值实现平滑移动
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            
            yield return null;
        }
        
        // 确保物体精确到达目标位置
        transform.position = targetPosition;
        Debug.Log($"物体缓慢向上移动了 {distance} 个单位，速度为 {speed} 单位/秒");
        
        // 重置协程引用
        currentMoveCoroutine = null;
    }
    
    /// <summary>
    /// 绘制移动目标标记（仅在Scene视图中显示）
    /// </summary>
    /// <param name="startPosition">起始位置</param>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="distance">移动距离</param>
    private void DrawTargetMarker(Vector3 startPosition, Vector3 targetPosition, float distance)
    {
        if (!showMoveTarget) return;
        
        Debug.Log($"移动目标: 从 {startPosition} 向上移动 {distance} 单位到 {targetPosition}");
        
        // 在运行时也可以显示标记（可选）
        #if UNITY_EDITOR
        // 在Scene视图中绘制标记
        UnityEditor.EditorApplication.delayCall += () => {
            UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                sceneView.Repaint();
            }
        };
        #endif
    }
    
    /// <summary>
    /// 在Scene视图中绘制移动路径和目标标记
    /// </summary>
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showMoveTarget) return;
        
        // 绘制从当前位置向上的示例路径（使用默认距离1单位作为示例）
        Vector3 currentPosition = transform.position;
        Vector3 exampleTarget = currentPosition + Vector3.up * 1f;
        
        // 绘制移动路径
        Gizmos.color = pathColor;
        Gizmos.DrawLine(currentPosition, exampleTarget);
        
        // 绘制起点
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(currentPosition, markerSize * 0.5f);
        
        // 绘制终点（示例）
        Gizmos.color = targetMarkerColor;
        Gizmos.DrawWireSphere(exampleTarget, markerSize);
        
        // 添加标签
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(currentPosition + Vector3.up * 0.3f, "起点");
        UnityEditor.Handles.Label(exampleTarget + Vector3.up * 0.3f, "向上移动目标点\n(示例: +1单位)");
        #endif
    }
    
    /// <summary>
    /// 当物体被选中时显示更详细的标记
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showMoveTarget) return;
        
        Vector3 currentPosition = transform.position;
        
        // 绘制多个距离示例
        float[] exampleDistances = { 1f, 2f, 5f, 10f };
        Color[] exampleColors = { Color.green, Color.yellow, Color.blue, Color.red };
        
        for (int i = 0; i < exampleDistances.Length; i++)
        {
            Vector3 targetPosition = currentPosition + Vector3.up * exampleDistances[i];
            
            // 绘制路径
            Gizmos.color = exampleColors[i];
            Gizmos.DrawLine(currentPosition, targetPosition);
            
            // 绘制目标点
            Gizmos.DrawWireSphere(targetPosition, markerSize * 0.8f);
            
            // 添加距离标签
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(targetPosition + Vector3.right * 0.3f + Vector3.up * 0.2f, 
                $"+{exampleDistances[i]}单位");
            #endif
        }
    }
#endif
}
