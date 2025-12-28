using UnityEngine;

/// <summary>
/// 可破碎砖块动画事件脚本
/// 功能：接收并处理砖块动画事件，控制动画播放和状态更新
/// </summary>
public class BreakableBrickAnimationEvent : MonoBehaviour
{
    // 引用可破碎砖块主脚本
    private BreakableBrick breakableBrick;
    private Animator animator;

    private void Awake()
    {
        // 获取父物体或自身的可破碎砖块脚本
        breakableBrick = GetComponentInParent<BreakableBrick>();
        animator=GetComponent<Animator>();
        if (breakableBrick == null)
        {
            Debug.LogWarning("未找到BreakableBrick脚本组件！");
        }
    }

    /// <summary>
    /// 破碎动画播放完成事件
    /// 由动画事件在破碎动画最后一帧调用
    /// </summary>
    public void OnBreakAnimationComplete()
    {
        Debug.Log("破碎动画播放完成");
        
        // 使用主脚本中的配置参数名，而不是硬编码
        if (breakableBrick != null && animator != null)
        {
            animator.SetBool(breakableBrick.breakBoolName, false);
        }
        // 可以在这里添加动画完成后的额外效果
        // 例如：完全隐藏砖块、触发额外粒子效果等
    }

    /// <summary>
    /// 复原动画播放完成事件
    /// 由动画事件在复原动画最后一帧调用
    /// </summary>
    public void OnRespawnAnimationComplete()
    {
        Debug.Log("复原动画播放完成");
        
        // 使用主脚本中的配置参数名，而不是硬编码
        if (breakableBrick != null && animator != null)
        {
            animator.SetBool(breakableBrick.respawnBoolName, false);
        }
        
        // 通知主脚本复原动画完成
        if (breakableBrick != null)
        {
            breakableBrick.OnRespawnAnimationCompleted();
        }
    }

    /// <summary>
    /// 动画开始播放事件
    /// </summary>
    /// <param name="animationName">动画名称</param>
    public void OnAnimationStart(string animationName)
    {
        if (showDebugInfo)
            Debug.Log($"动画开始播放：{animationName}");
    }

    /// <summary>
    /// 动画中间关键点事件
    /// 可以在动画的特定时间点调用，用于触发效果
    /// </summary>
    /// <param name="eventType">事件类型</param>
    public void OnAnimationKeyEvent(string eventType)
    {
        if (showDebugInfo)
            Debug.Log($"动画关键事件：{eventType}");
        
        switch (eventType)
        {
            case "break_effect":
                // 播放破碎特效
                if (breakableBrick != null)
                {
                    breakableBrick.PlayBreakEffect();
                }
                break;
            case "respawn_effect":
                // 播放复原特效
                if (breakableBrick != null)
                {
                    breakableBrick.PlayRespawnEffect();
                }
                break;
        }
    }
    
    // 调试信息开关（与主脚本保持一致）
    private bool showDebugInfo
    {
        get { return breakableBrick != null ? breakableBrick.showDebugInfo : false; }
    }
}
