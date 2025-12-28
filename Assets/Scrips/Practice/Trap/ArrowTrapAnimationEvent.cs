using UnityEngine;

/// <summary>
/// 射箭陷阱动画事件处理脚本
/// 功能：处理动画事件并触发箭的发射
/// 用法：挂载到Trap_arrow的子物体上
/// </summary>
public class ArrowTrapAnimationEvent : MonoBehaviour
{
    #region 组件引用
    [SerializeField] private ArrowTrap arrowTrap;
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 如果没有手动设置arrowTrap引用，尝试从父物体获取
        if (arrowTrap == null)
        {
            arrowTrap = GetComponentInParent<ArrowTrap>();
            if (arrowTrap == null)
            {
                Debug.LogError("未找到ArrowTrap组件！请确保该脚本挂载到Trap_arrow的子物体上。");
            }
        }
    }
    #endregion

    #region 动画事件函数
    /// <summary>
    /// 发射箭的动画事件方法
    /// 用于在动画的特定帧调用，触发箭的发射
    /// </summary>
    public void OnFireArrow()
    {
        if (arrowTrap != null)
        {
            arrowTrap.FireArrow();
        }
        else
        {
            Debug.LogError("ArrowTrap引用为空，无法发射箭！");
        }
    }

    /// <summary>
    /// 陷阱准备动画事件方法
    /// 用于在动画的特定帧调用，可以添加准备音效或效果
    /// </summary>
    public void OnTrapPrepare()
    {
        // 可以在这里添加陷阱准备的音效或视觉效果
        // Debug.Log("陷阱准备发射！");
    }

    /// <summary>
    /// 陷阱重置动画事件方法
    /// 用于在动画的特定帧调用，可以重置陷阱状态
    /// </summary>
    public void OnTrapReset()
    {
        // 可以在这里添加陷阱重置的逻辑
        // Debug.Log("陷阱已重置！");
    }
    #endregion
}
