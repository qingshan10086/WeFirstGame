using UnityEngine;

/// <summary>
/// 射箭陷阱脚本
/// 功能：通过动画事件触发发射箭，支持顺序发射（上一支箭消失后才发射下一支）
/// </summary>
public class ArrowTrap : MonoBehaviour
{
    #region 组件引用
    [SerializeField] private Animator animator;
    [SerializeField] private Transform arrowSpawnPoint;
    #endregion

    #region 箭参数
    [Header("箭参数")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float arrowLifetime = 5f;
    #endregion

    #region 发射控制
    [Header("发射控制")]
    [SerializeField] private bool isSequentialFiring = true; // 是否顺序发射（上一支消失后才发射下一支）
    private bool canFireNextArrow = true; // 是否可以发射下一支箭
    #endregion

    #region 生命周期方法
    private void Start()
    {
        // 初始化组件引用
        if (animator == null) animator = GetComponent<Animator>();
        
        // 如果没有设置生成点，默认使用自身位置
        if (arrowSpawnPoint == null)
        {
            arrowSpawnPoint = transform;
        }
    }
    #endregion

    #region 动画事件函数
    /// <summary>
    /// 发射箭的方法 - 用于动画事件调用
    /// </summary>
    public void FireArrow()
    {
        // 如果是顺序发射且当前不能发射，则返回
        if (isSequentialFiring && !canFireNextArrow)
        {
            Debug.Log("当前箭未消失，无法发射下一支。");
            return;
        }
        
        if (arrowPrefab == null)
        {
            Debug.LogError("箭预制体未设置！");
            return;
        }

        // 基于发射器的旋转角度确定发射方向
        // 旋转角度为0时，发射器面朝下
        float rotationAngle = transform.eulerAngles.z;
        
        // 将旋转角度转换为方向向量：角度0时向下(0,-1)，90度时向右(1,0)，180度时向上(0,1)，270度时向左(-1,0)
        float radians = rotationAngle * Mathf.Deg2Rad;
        Vector2 launchDirection = new Vector2(Mathf.Sin(radians), -Mathf.Cos(radians));
        
        // 计算箭的初始旋转角度：角度0时箭头指向下
        float arrowAngle = rotationAngle - 90f;
        
        // 实例化箭并设置初始旋转，确保箭头朝向发射方向
        // 箭旋转角度为0时，箭头指向下
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.AngleAxis(arrowAngle, Vector3.forward));
        
        // 设置箭的层级为trap
        arrow.layer = LayerMask.NameToLayer("trap");
        
        // 获取箭的刚体组件
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            // 确保刚体类型是Dynamic
            if (arrowRb.bodyType != RigidbodyType2D.Dynamic)
            {
                arrowRb.bodyType = RigidbodyType2D.Dynamic;
                Debug.LogWarning("箭预制体的刚体类型不是Dynamic，已自动修正。");
            }
            
            // 关闭重力
            arrowRb.gravityScale = 0f;
            
            // 设置箭的速度
            Vector2 finalVelocity = launchDirection.normalized * arrowSpeed;
            arrowRb.velocity = finalVelocity;
            Debug.Log("箭已发射，速度：" + finalVelocity);
        }
        else
        {
            Debug.LogError("箭预制体没有Rigidbody2D组件！");
        }
        
        // 获取箭组件并注册消失事件
        Arrow arrowComponent = arrow.GetComponent<Arrow>();
        if (arrowComponent != null)
        {
            arrowComponent.OnArrowDestroyed += OnArrowDestroyed;
        }
        
        // 设置箭的生命周期
        Destroy(arrow, arrowLifetime);
        
        // 如果是顺序发射，设置当前不能发射
        if (isSequentialFiring)
        {
            canFireNextArrow = false;
        }
    }
    
    /// <summary>
    /// 当箭被销毁时调用的方法
    /// </summary>
    private void OnArrowDestroyed()
    {
        canFireNextArrow = true;
        Debug.Log("箭已消失，可以发射下一支。");
    }
    #endregion

    #region 手动控制方法
    /// <summary>
    /// 手动触发发射箭（可用于测试或其他逻辑）
    /// </summary>
    public void TriggerTrap()
    {
        // 如果是顺序发射且当前不能发射，则返回
        if (isSequentialFiring && !canFireNextArrow)
        {
            Debug.Log("当前箭未消失，无法发射下一支。");
            return;
        }
        
        // 播放发射动画
        if (animator != null)
        {
            animator.SetTrigger("Fire");
        }
        else
        {
            // 如果没有动画组件，直接发射箭
            FireArrow();
        }
    }
    #endregion

    #region 编辑器辅助
    private void OnDrawGizmosSelected()
    {
        // 绘制箭生成点
        if (arrowSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(arrowSpawnPoint.position, 0.1f);
        }
        
        // 绘制发射方向
        Vector2 startPos = arrowSpawnPoint != null ? arrowSpawnPoint.position : transform.position;
        
        // 基于发射器的旋转角度确定发射方向（与FireArrow方法中一致）
        float currentRotationAngle = transform.eulerAngles.z;
        float currentRadians = currentRotationAngle * Mathf.Deg2Rad;
        Vector2 currentLaunchDirection = new Vector2(Mathf.Sin(currentRadians), -Mathf.Cos(currentRadians));
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos, startPos + currentLaunchDirection.normalized);
        
        // 绘制箭头指示器
        Vector2 arrowTip = startPos + currentLaunchDirection.normalized;
        float angle = currentRotationAngle;
        Vector2 arrowLeft = arrowTip + (Vector2)(Quaternion.Euler(0, 0, angle + 135) * Vector2.right * 0.2f);
        Vector2 arrowRight = arrowTip + (Vector2)(Quaternion.Euler(0, 0, angle - 135) * Vector2.right * 0.2f);
        Gizmos.DrawLine(arrowTip, arrowLeft);
        Gizmos.DrawLine(arrowTip, arrowRight);
    }
    #endregion
}