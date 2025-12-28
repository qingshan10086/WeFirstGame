using UnityEngine;

/// <summary>
/// 可破碎砖块调试脚本
/// 用于诊断动画执行两遍的问题
/// </summary>
public class BreakableBrickDebug : MonoBehaviour
{
    private BreakableBrick breakableBrick;
    private BreakableBrickAnimationEvent animationEvent;
    private Animator animator;
    
    private int breakTriggerCount = 0;
    private int respawnTriggerCount = 0;
    
    private void Awake()
    {
        breakableBrick = GetComponent<BreakableBrick>();
        animationEvent = GetComponent<BreakableBrickAnimationEvent>();
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        Debug.Log("=== 可破碎砖块调试信息 ===");
        Debug.Log($"BreakableBrick 脚本存在: {breakableBrick != null}");
        Debug.Log($"BreakableBrickAnimationEvent 脚本存在: {animationEvent != null}");
        Debug.Log($"Animator 组件存在: {animator != null}");
        
        if (breakableBrick != null)
        {
            Debug.Log($"破碎动画参数名: {breakableBrick.breakBoolName}");
            Debug.Log($"复原动画参数名: {breakableBrick.respawnBoolName}");
        }
    }
    
    private void Update()
    {
        if (animator != null)
        {
            // 监控动画状态
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            bool isBreak = animator.GetBool(breakableBrick.breakBoolName);
            bool isRespawn = animator.GetBool(breakableBrick.respawnBoolName);
            
            Debug.Log($"当前动画状态: {currentState.fullPathHash} - {currentState.IsName("Break")}");
            Debug.Log($"Break 参数: {isBreak}, Respawn 参数: {isRespawn}");
            
            // 检测状态变化
            if (isBreak && breakTriggerCount == 0)
            {
                breakTriggerCount++;
                Debug.Log($"第一次触发破碎动画: 时间 {Time.time}");
            }
            else if (isBreak && breakTriggerCount > 0)
            {
                breakTriggerCount++;
                Debug.Log($"重复触发破碎动画: 第 {breakTriggerCount} 次，时间 {Time.time}");
            }
            
            if (isRespawn && respawnTriggerCount == 0)
            {
                respawnTriggerCount++;
                Debug.Log($"第一次触发复原动画: 时间 {Time.time}");
            }
            else if (isRespawn && respawnTriggerCount > 0)
            {
                respawnTriggerCount++;
                Debug.Log($"重复触发复原动画: 第 {respawnTriggerCount} 次，时间 {Time.time}");
            }
        }
    }
    
    private void OnDisable()
    {
        Debug.Log("=== 调试结束 ===");
        Debug.Log($"破碎动画总触发次数: {breakTriggerCount}");
        Debug.Log($"复原动画总触发次数: {respawnTriggerCount}");
    }
}
