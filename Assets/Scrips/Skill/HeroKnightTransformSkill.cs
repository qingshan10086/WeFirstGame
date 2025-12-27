using System.Collections;
using UnityEngine;

public class HeroKnightTransformSkill : MonoBehaviour
{
    [Header("配置")]
    [SerializeField] private float transformDuration = 30f; // 变身持续时间（秒）
    [SerializeField] private float cooldown = 60f;          // 技能冷却（秒）

    [Header("目标 Animator Controller")]
    [SerializeField] private RuntimeAnimatorController transformController; // 指定要切换到的 AnimatorController（Inspector 指定）

    [Header("可选：在变身时覆盖 Player 的 Transform/Collider/判定")]
    [SerializeField] private bool overrideScale = false;
    [SerializeField] private Vector3 targetLocalScale = Vector3.one;

    [SerializeField] private bool overrideCollider = false;
    [Tooltip("目标 CapsuleCollider2D 大小 (size)")]
    [SerializeField] private Vector2 targetColliderSize = Vector2.one;
    [Tooltip("目标 CapsuleCollider2D 偏移 (offset)")]
    [SerializeField] private Vector2 targetColliderOffset = Vector2.zero;

    //wallCheck覆盖
    [SerializeField] private bool overrideWallCheck = false;
    [Tooltip("墙体检测点相对于 Player 根节点的本地位置")]
    [SerializeField] private Vector3 wallCheckLocalPosition = Vector3.zero;
    [Tooltip("墙体检测距离")]
    [SerializeField] private float wallCheckDistanceOverride;

    //groundCheck覆盖
    [SerializeField] private bool overrideGroundCheck = false;
    [Tooltip("地面检测点相对于 Player 根节点的本地位置")]
    [SerializeField] private Vector3 groundCheckLocalPosition = Vector3.zero;
    [Tooltip("地面检测距离")]
    [SerializeField] private float groundCheckDistanceOverride;

    [SerializeField] private bool overrideAttackCheck = false;
    [Tooltip("攻击判定点相对于 Player 根节点的本地位置")]
    [SerializeField] private Vector3 attackCheckLocalPosition = Vector3.zero;
    [Tooltip("攻击判定半径")]
    [SerializeField] private float attackRadiusOverride = 0.7f;

    [SerializeField] private bool overrideStats = false;
    [Tooltip("变身后覆盖伤害")]
    [SerializeField] private int transformedDamage = 20;
    [Tooltip("变身后覆盖最大血量（会把当前血量设置为该最大值）")]
    [SerializeField] private int transformedHealth = 200;

    // 内部缓存
    private RuntimeAnimatorController originalController;
    private Animator playerAnimator;
    private Player player;
    private bool isTransformed = false;
    private bool isOnCooldown = false;
    private Coroutine transformCoroutine;

    // 原始值备份（用于还原）
    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector3 originalAttackCheckLocalPos;
    private float originalAttackRadius;
    private Vector3 originalWallCheckLocalPos;
    private float originalWallCheckDistance;
    private Vector3 originalGroundCheckLocalPos;
    private float originalGroundCheckDistance;
    private CapsuleCollider2D playerCapsule;

    // 统计值备份
    private int originalDamage;
    private int originalHealth;
    private int originalMaxHealth;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("[HeroKnightTransformSkill] 未找到 Player 实例，变身将不可用。");
            return;
        }

        playerAnimator = player.GetComponentInChildren<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogWarning("[HeroKnightTransformSkill] Player 上未找到 Animator，变身将不可用。");
            return;
        }

        originalController = playerAnimator.runtimeAnimatorController;

        // 准备碰撞器引用（优先使用 Entity 提供的 cd 字段，其次尝试 GetComponent）
        playerCapsule = null;
        var entity = player as Entity;
        if (entity != null)
        {
            playerCapsule = entity.cd;
        }
        if (playerCapsule == null)
        {
            playerCapsule = player.GetComponent<CapsuleCollider2D>();
        }

        // 订阅血量变化回调，用于在变身时被打死的处理
        if (player.stats != null)
        {
            player.stats.onHealthChanged += OnPlayerHealthChanged;
        }
        else
        {
            // 保险：尝试再次获取 CharacterStats 组件 并订阅
            var cs = player.GetComponent<CharacterStats>();
            if (cs != null)
                cs.onHealthChanged += OnPlayerHealthChanged;
        }
    }

    private void OnDestroy()
    {
        // 取消订阅，防止内存泄漏或空引用
        if (player != null && player.stats != null)
        {
            player.stats.onHealthChanged -= OnPlayerHealthChanged;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TryActivateTransform();
        }
    }

    private void TryActivateTransform()
    {
        if (playerAnimator == null || transformController == null)
            return;

        if (isOnCooldown)
        {
            Debug.Log("[HeroKnightTransformSkill] 技能正在冷却中。");
            return;
        }

        if (isTransformed)
        {
            Debug.Log("[HeroKnightTransformSkill] 已处于变身状态。");
            return;
        }

        // 备份原始属性（第一次变身时）
        originalScale = player.transform.localScale;
        if (playerCapsule != null)
        {
            originalColliderSize = playerCapsule.size;
            originalColliderOffset = playerCapsule.offset;
        }
        if (player.attackCheck != null)
        {
            originalAttackCheckLocalPos = player.attackCheck.localPosition;
            originalAttackRadius = player.attackCheckRadius;
        }
        if(player.wallCheck != null)
        {
            originalWallCheckLocalPos = player.wallCheck.localPosition;
            originalWallCheckDistance = player.wallCheckDiatance;
        }
        if(player.groundCheck != null)
        {
            originalGroundCheckLocalPos = player.groundCheck.localPosition;
            originalGroundCheckDistance = player.groundCheckDistance;
        }

        // 备份数值（若需要覆盖）
        if (overrideStats && player.stats != null)
        {
            originalDamage = player.stats.damage != null ? player.stats.damage.GetValue() : 0;
            originalHealth = player.stats.currentHealth;
            originalMaxHealth = player.stats.maxHealth != null ? player.stats.maxHealth.GetValue() : player.stats.GetMaxHealthValue();
        }

        transformCoroutine = StartCoroutine(TransformRoutine());
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator TransformRoutine()
    {
        isTransformed = true;

        // 切换 AnimatorController
        playerAnimator.runtimeAnimatorController = transformController;

        // 覆盖缩放
        if (overrideScale)
        {
            player.transform.localScale = targetLocalScale;
        }

        // 覆盖碰撞器
        if (overrideCollider && playerCapsule != null)
        {
            playerCapsule.size = targetColliderSize;
            playerCapsule.offset = targetColliderOffset;
        }

        // 覆盖攻击判定点/半径
        if (overrideAttackCheck && player.attackCheck != null)
        {
            player.attackCheck.localPosition = attackCheckLocalPosition;
            player.attackCheckRadius = attackRadiusOverride;
        }

        // 覆盖墙体检测点/距离
        if (overrideWallCheck && player.wallCheck != null)
        {
            player.wallCheck.localPosition = wallCheckLocalPosition;
            player.wallCheckDiatance = wallCheckDistanceOverride;
        }

        // 覆盖地面检测点/距离
        if (overrideGroundCheck && player.groundCheck != null)
        {
            player.groundCheck.localPosition = groundCheckLocalPosition;
            player.groundCheckDistance = groundCheckDistanceOverride;
        }

        // 覆盖数值
        if (overrideStats && player.stats != null)
        {
            // 攻击：把 damage 的基础值设为 transformedDamage（使用 Stat.SetDefalutValue）
            if (player.stats.damage != null)
            {
                player.stats.damage.SetDefalutValue(transformedDamage);
            }

            // 最大血量：把 maxHealth 基础值设为 transformedHealth，继承当前血量
            if (player.stats.maxHealth != null)
            {
                player.stats.maxHealth.SetDefalutValue(transformedHealth);
            }

            player.stats.onHealthChanged?.Invoke();
        }

        // 等待变身持续时间或被强制结束
        float elapsed = 0f;
        while (elapsed < transformDuration)
        {
            if (player == null || playerAnimator == null)
                break;

            // 若在外部因死亡等原因强制结束变身，退出循环
            if (!isTransformed)
                break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 如果仍处于变身（未被 ForceEndTransform 清除），还原
        if (isTransformed)
        {
            RestoreAfterTransform();
        }

        transformCoroutine = null;
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        float elapsed = 0f;
        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        isOnCooldown = false;
    }

    // 外部强制结束变身（例如被打死时调用）
    public void ForceEndTransform()
    {
        if (transformCoroutine != null)
        {
            StopCoroutine(transformCoroutine);
            transformCoroutine = null;
        }

        if (isTransformed)
        {
            RestoreAfterTransform();
        }

        isTransformed = false;
    }

    // 将还原逻辑集中到一个方法，避免重复
    private void RestoreAfterTransform()
    {
        //位置上抬两个单位
        if (transformCoroutine != null)
        {
            Vector3 vector3 = new Vector3(0, 2, 0);
            player.transform.position += vector3;
        }

        if (playerAnimator != null && originalController != null)
        {
            playerAnimator.runtimeAnimatorController = originalController;
        }

        if (overrideScale && player != null)
        {
            player.transform.localScale = originalScale;
        }

        if (overrideCollider && playerCapsule != null)
        {
            playerCapsule.size = originalColliderSize;
            playerCapsule.offset = originalColliderOffset;
        }

        if (overrideAttackCheck && player.attackCheck != null)
        {
            player.attackCheck.localPosition = originalAttackCheckLocalPos;
            player.attackCheckRadius = originalAttackRadius;
        }

        if (overrideWallCheck && player.wallCheck != null)
        {
            player.wallCheck.localPosition = originalWallCheckLocalPos;
            player.wallCheckDiatance = originalWallCheckDistance;
        }

        if (overrideGroundCheck && player.groundCheck != null)
        {
            player.groundCheck.localPosition = originalGroundCheckLocalPos;
            player.groundCheckDistance = originalGroundCheckDistance;
        }

        // 还原数值
        if (overrideStats && player.stats != null)
        {
            if (player.stats.damage != null)
            {
                player.stats.damage.SetDefalutValue(originalDamage);
            }

            if (player.stats.maxHealth != null)
            {
                player.stats.maxHealth.SetDefalutValue(originalMaxHealth);
            }

            // 恢复当前血量到变身前的数值（若变身结束时原始血量 > 当前最大值，则取当前最大值）
            int restoredCurrent = Mathf.Min(originalHealth, player.stats.GetMaxHealthValue());
            player.stats.currentHealth = originalMaxHealth;
            player.stats.onHealthChanged?.Invoke();
        }

        isTransformed = false;
    }

    // 当玩家血量变动时调用
    private void OnPlayerHealthChanged()
    {
        if (player == null || player.stats == null) return;

        // 如果在变身期间血量降到 0 或以下，退出变身并回半血
        if (player.stats.currentHealth <= 0 && isTransformed)
        {
            // 先结束变身还原外观/判定
            ForceEndTransform();

            // 回满血并通知（调用 onHealthChanged 以便 UI 更新）
            player.stats.currentHealth = player.stats.GetMaxHealthValue()/2;
            player.stats.onHealthChanged?.Invoke();

            // 防止进入死亡状态：尝试让玩家回到空闲态（视项目状态机而定）
            if (player.stateMachine != null && player.idleState != null)
            {
                player.stateMachine.ChangeState(player.idleState);
            }

            Debug.Log("[HeroKnightTransformSkill] 变身期间被打死，已退出变身并回满血。");
        }
    }
}