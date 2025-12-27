using UnityEngine;

/// <summary>
/// 玩家剑气技能
/// 处理动画事件，在玩家面前生成剑气预制体
/// </summary>
public class PlayerSwordSlash : MonoBehaviour
{
    [Header("剑气设置")]
    [Tooltip("剑气预制体")]
    public GameObject swordSlashPrefab;

    [Tooltip("剑气生成位置偏移")]
    [SerializeField]
    private Vector3 spawnOffset = new Vector3(1f, 0.5f, 0f);

    [Tooltip("剑气缩放")]
    [SerializeField]
    private Vector3 slashScale = Vector3.one;

    [Tooltip("剑气生成时的旋转偏移")]
    [SerializeField]
    private Vector3 rotationOffset = Vector3.zero;

    [Header("玩家引用")]
    [Tooltip("玩家朝向标记（可选）")]
    public Transform facingDirection;

    // 玩家面向右侧的标记
    private bool isFacingRight = true;

    private void Start()
    {
        // 尝试自动查找玩家方向信息
        if (facingDirection == null)
        {
            // 查找玩家的SpriteRenderer来确定朝向
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                isFacingRight = !spriteRenderer.flipX;
            }
        }
    }

    /// <summary>
    /// 动画事件：生成剑气
    /// 从动画关键帧调用此方法
    /// </summary>
    public void SpawnSwordSlash()
    {
        if (swordSlashPrefab == null)
        {
            Debug.LogError("SwordSlashPrefab is not assigned!");
            return;
        }

        // 确定玩家朝向
        DeterminePlayerFacing();

        // 计算剑气生成位置
        Vector3 spawnPosition = CalculateSpawnPosition();

        // 计算剑气旋转
        Quaternion spawnRotation = CalculateSpawnRotation();

        // 生成剑气预制体
        GameObject slash = Instantiate(swordSlashPrefab, spawnPosition, spawnRotation);

        // 设置剑气缩放
        slash.transform.localScale = slashScale;

        // 可选：设置剑气的生命周期或其他属性
        // StartCoroutine(DestroyAfterLifetime(slash));
    }

    /// <summary>
    /// 确定玩家朝向
    /// </summary>
    private void DeterminePlayerFacing()
    {
        if (facingDirection != null)
        {
            // 使用指定的方向标记
            isFacingRight = facingDirection.localScale.x > 0;
        }
        else
        {
            // 尝试从SpriteRenderer获取朝向
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                isFacingRight = !spriteRenderer.flipX;
            }
            // 或者从Transform.localScale获取
            else if (transform.localScale.x != 0)
            {
                isFacingRight = transform.localScale.x > 0;
            }
        }
    }

    /// <summary>
    /// 计算剑气生成位置
    /// </summary>
    private Vector3 CalculateSpawnPosition()
    {
        // 基础生成位置
        Vector3 position = transform.position;

        // 应用偏移量，根据玩家朝向调整X轴偏移
        Vector3 adjustedOffset = spawnOffset;
        if (!isFacingRight)
        {
            adjustedOffset.x = -adjustedOffset.x;
        }

        // 计算最终位置
        position += adjustedOffset;

        return position;
    }

    /// <summary>
    /// 计算剑气旋转
    /// </summary>
    private Quaternion CalculateSpawnRotation()
    {
        // 基础旋转
        Quaternion rotation = Quaternion.identity;

        // 根据玩家朝向调整旋转
        if (!isFacingRight)
        {
            // 玩家面向左侧时，翻转X轴
            rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        // 应用额外的旋转偏移
        rotation *= Quaternion.Euler(rotationOffset);

        return rotation;
    }

    /// <summary>
    /// 设置玩家朝向（可由外部脚本调用）
    /// </summary>
    /// <param name="facingRight">是否面向右侧</param>
    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    /// <summary>
    /// 调整剑气生成偏移（可由外部脚本调用）
    /// </summary>
    /// <param name="offset">新的偏移量</param>
    public void SetSpawnOffset(Vector3 offset)
    {
        spawnOffset = offset;
    }

    /// <summary>
    /// 调整剑气缩放（可由外部脚本调用）
    /// </summary>
    /// <param name="scale">新的缩放值</param>
    public void SetSlashScale(Vector3 scale)
    {
        slashScale = scale;
    }

    // 可选：剑气生命周期管理
    /*
    private IEnumerator DestroyAfterLifetime(GameObject slash)
    {
        yield return new WaitForSeconds(lifetime);
        if (slash != null)
        {
            Destroy(slash);
        }
    }
    */

    // 在场景视图中可视化剑气生成位置
    private void OnDrawGizmosSelected()
    {
        if (swordSlashPrefab == null)
            return;

        // 确定玩家朝向
        DeterminePlayerFacing();

        // 计算并绘制生成位置
        Vector3 position = CalculateSpawnPosition();
        Quaternion rotation = CalculateSpawnRotation();

        // 绘制生成位置标记
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(position, 0.1f);

        // 绘制剑气方向
        Gizmos.color = Color.yellow;
        Vector3 direction = rotation * Vector3.right;
        Gizmos.DrawLine(position, position + direction * 0.5f);

        // 绘制剑气范围
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, slashScale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldMatrix;
    }
}
