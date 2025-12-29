using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
/// <summary>
/// 可破碎砖块脚本
/// 功能：当玩家踩上去后，经过一段时间会破碎
/// </summary>
public class BreakableBrick : MonoBehaviour
{
    #region 配置参数
    [Header("破碎设置")]
    [SerializeField] private float breakDelay = 1.5f; // 踩上去到破碎的延迟时间
    [SerializeField] private float respawnDelay = 5.0f; // 破碎后到复原的延迟时间
    [SerializeField] public bool showDebugInfo = true; // 是否显示调试信息

    [Header("破碎效果")]
    [SerializeField] private Animator animator; // 动画组件（可选，用于播放破碎动画）
    [SerializeField] public string breakBoolName = "Break";
    [SerializeField] public string respawnBoolName = "Respawn"; // 复原动画Bool参数名称
    [SerializeField] private ParticleSystem breakParticles; // 破碎粒子效果（可选）
    [SerializeField] private AudioSource breakAudio; // 破碎音效（可选）
    [SerializeField] private AudioSource respawnAudio; // 复原音效（可选）
    #endregion

    #region 内部状态
    private bool isPlayerOnBrick = false; // 玩家是否在砖块上
    private bool isBreaking = false; // 是否正在破碎过程中
    private bool isRespawning = false; // 是否正在复原过程中
    private Collider2D brickCollider; // 砖块的碰撞器组件
    private SpriteRenderer spriteRenderer; // 砖块的精灵渲染器
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 初始化组件引用
        if (brickCollider == null) brickCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        animator=GetComponentInChildren<Animator>();

        // 确保碰撞器是实体碰撞体
        if (brickCollider != null)
        {
            brickCollider.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查是否有玩家踩上来
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家踩上砖块");
            PlayerSteppedOnBrick();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 检查玩家是否离开砖块
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerLeftBrick();
        }
    }
    #endregion

    #region 玩家交互处理
    /// <summary>
    /// 玩家踩上砖块时调用
    /// </summary>
    private void PlayerSteppedOnBrick()
    {
        if (isBreaking || isPlayerOnBrick) return;

        isPlayerOnBrick = true;
        if (showDebugInfo) Debug.Log($"玩家踩上砖块，{breakDelay}秒后破碎");

        // 开始破碎倒计时
        StartCoroutine(BreakCountdown());
    }

    /// <summary>
    /// 玩家离开砖块时调用
    /// </summary>
    private void PlayerLeftBrick()
    {
        if (!isPlayerOnBrick) return;

        isPlayerOnBrick = false;
        if (showDebugInfo) Debug.Log("玩家离开砖块");

        // 不再停止协程，即使玩家离开，砖块也会继续完成破碎过程
    }
    #endregion

    #region 破碎逻辑
    /// <summary>
    /// 破碎倒计时协程
    /// </summary>
    private System.Collections.IEnumerator BreakCountdown()
    {
        // 等待指定的延迟时间
        yield return new WaitForSeconds(breakDelay);

        // 执行破碎
        BreakBrick();
    }

    /// <summary>
    /// 执行砖块破碎
    /// </summary>
    private void BreakBrick()
    {
        if (isBreaking) return;

        isBreaking = true;
        if (showDebugInfo) Debug.Log("砖块破碎！");

        // 播放破碎动画（如果有）
        if (animator != null && !string.IsNullOrEmpty(breakBoolName))
        {
            animator.SetBool(breakBoolName, true);
        }

        // 播放破碎音效（如果有）
        if (breakAudio != null)
        {
            breakAudio.Play();
        }

        // 禁用碰撞器
        if (brickCollider != null)
        {
            brickCollider.enabled = false;
        }

        // 禁用精灵渲染器（可选，或在动画中处理）
        if (spriteRenderer != null)
        {
            // 如果没有动画，可以直接禁用渲染器
            if (animator == null)
            {
                spriteRenderer.enabled = false;
                // 如果没有动画，直接播放破碎粒子
                if (breakParticles != null)
                {
                    breakParticles.Play();
                }
            }
        }

        // 一段时间后复原砖块对象
        StartCoroutine(RespawnAfterBreak());
    }

    /// <summary>
    /// 破碎后延迟复原砖块
    /// </summary>
    private System.Collections.IEnumerator RespawnAfterBreak()
    {
        // 不再等待动画播放完成，直接等待复原延迟时间
        // 动画事件会控制动画的实际播放和结束
        if (showDebugInfo) Debug.Log("砖块等待复原...");
        yield return new WaitForSeconds(respawnDelay);

        // 执行复原
        RespawnBrick();
    }

    /// <summary>
    /// 执行砖块复原
    /// </summary>
    private void RespawnBrick()
    {
        if (showDebugInfo) Debug.Log("砖块开始复原");

        isRespawning = true;

        // 播放复原动画（如果有）
        if (animator != null && !string.IsNullOrEmpty(respawnBoolName))
        {
            animator.SetBool(breakBoolName, false);
            animator.SetBool(respawnBoolName, true);
        }
        // 如果没有复原动画，直接显示砖块
        else if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // 播放复原音效（如果有）
        if (respawnAudio != null)
        {
            respawnAudio.Play();
        }

        // 重新启用碰撞器
        if (brickCollider != null)
        {
            brickCollider.enabled = true;
        }

        // 重置状态
        isPlayerOnBrick = false;
        isBreaking = false;
        
        // 不再使用协程等待，改为通过动画事件通知
        // 动画事件脚本会在动画完成时调用OnRespawnAnimationCompleted方法
    }
    
    /// <summary>
    /// 等待复原动画完成的协程
    /// 备用方法，如果不使用动画事件可以调用此方法
    /// 注意：此方法已弃用，建议使用动画事件控制动画结束
    /// </summary>
    [System.Obsolete("请使用动画事件控制动画结束，而不是协程等待")]
    private System.Collections.IEnumerator WaitForRespawnAnimation()
    {
        // 不再使用动画长度作为延迟时间
        // 直接使用固定延迟（仅作为备用）
        float respawnDelayTime = 0.5f;
        
        yield return new WaitForSeconds(respawnDelayTime);
        
        // 重置动画参数和状态
        CompleteRespawnAnimation();
    }
    
    /// <summary>
    /// 完成复原动画（由动画事件调用）
    /// </summary>
    public void OnRespawnAnimationCompleted()
    {
        CompleteRespawnAnimation();
    }
    
    /// <summary>
    /// 完成复原动画的实际处理
    /// </summary>
    private void CompleteRespawnAnimation()
    {
        // 重置动画参数和状态
        if (animator != null && !string.IsNullOrEmpty(respawnBoolName))
        {
            animator.SetBool(respawnBoolName, false);
        }
        
        isRespawning = false;
        if (showDebugInfo) Debug.Log("砖块已复原");
    }
    
    /// <summary>
    /// 播放破碎特效（由动画事件调用）
    /// </summary>
    public void PlayBreakEffect()
    {
        // 播放破碎粒子效果（如果有）
        if (breakParticles != null)
        {
            breakParticles.Play();
        }
    }
    
    /// <summary>
    /// 播放复原特效（由动画事件调用）
    /// </summary>
    public void PlayRespawnEffect()
    {
        // 可以在这里添加复原特效
    }

    // 已删除依赖动画长度的方法，现在由动画事件控制动画结束
    #endregion
#if UNITY_EDITOR
    #region 调试辅助

    private void OnDrawGizmosSelected()
    {
        // 根据状态设置不同颜色
        if (isBreaking)
            Gizmos.color = Color.red;
        else if (isRespawning)
            Gizmos.color = Color.blue;
        else if (isPlayerOnBrick)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.green;

        // 绘制砖块范围
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(transform.position, boxCollider.size);
            }
        }

        // 显示调试文本
        if (showDebugInfo)
        {
            string state = isBreaking ? "破碎中" : 
                         (isRespawning ? "复原中" : 
                         (isPlayerOnBrick ? "玩家在上面" : "空闲"));
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
                $"状态: {state}\n破碎延迟: {breakDelay}s\n复原延迟: {respawnDelay}s");
        }
    }
#endregion
#endif
}
