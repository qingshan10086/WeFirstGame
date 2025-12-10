using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Death : Enemy
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public BattleState battleState { get; private set; }
    public AttackState attackState { get; private set; }
    public EnhancedAttackState enhancedAttackState { get; private set; }
    public CastState castState { get; private set; }
    public SummonState summonState { get; private set; }
    public StunnedState stunnedState { get; private set; }
    public DeadState deadState { get; private set; }
    public ControlState controlState { get; private set; } // 新增控制状态

    [Header("Boss_Death Info")]
    public int defaultarmor = 10;         // 默认护甲值

    [Header("Cast Info")]
    public GameObject castPrefab;          // Inspector 指定要生成的预制体
    public float castSpawnHeight = 4f;     // 预制体出现在 player 之上多少单位
    public float castTriggerDistance = 8f; // 当 Boss 与 player 距离 > 此值时有概率触发施法
    public float castTriggerDistance2 = 12f; // 当 Boss 与 player 距离 < 此值施法
    [Range(0f, 1f)]
    public float castProbability = 0.25f;  // 触发概率（0-1）
    public float castCooldown = 6f;        // 施法冷却
    [HideInInspector] public float lastCastTime = -100f;
    public float castDuration = 1.0f;      // 施法状态持续时间（可由动画触发改成按 trigger 切换）

    [Header("Enhanced Attack Info")]
    public float enhancedDamageAdd = 20;
    private int normalAttackCount = 0;
    private int enhancedAttackRemaining = 0;
    private int originalDamageValue = 0;
    private bool enhancedBuffActive = false;

    [Header("Summon Info")]
    public List<GameObject> minionPrefabs = new List<GameObject>(); // 在 Inspector 放 5 个小怪预制体
    public int minionPerSummon = 3;           // 每次召唤数量（随机选）
    public float summonRadius = 2f;           // 小怪生成半径（Boss 四周）
    public float summonCooldown = 60f;        // 召唤冷却（秒）
    private float summonCooldown2 = 300f;     // 第二轮召唤冷却
    [HideInInspector] public float lastSummonTime = -100f;
    [HideInInspector] public float lastSummonTime2 = -300f;
    [Range(0f, 1f)]
    public float summonHpThreshold = 0.75f;   // 低于该比例开始可召唤（0.75 = 75%）
    public float summonHpThreshold2 = 0.4f;

    [Header("Control Info")]
    public GameObject controlPrefab;      // 控制状态下的特效预制体
    public float controlDuration = 5f; // 控制玩家的时长
    public float controlHpThreshold = 0.3f; // 低于该血量比例时触发控制
    public float controlCooldown = 30f; // 控制技能冷却时间
    [HideInInspector] public float lastControlTime = -100f;


    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState(this, stateMachine, "Idle", this);
        moveState = new MoveState(this, stateMachine, "Move", this);
        battleState = new BattleState(this, stateMachine, "Move", this);
        attackState = new AttackState(this, stateMachine, "Attack", this);
        enhancedAttackState = new EnhancedAttackState(this, stateMachine, "EAttack", this);
        castState = new CastState(this, stateMachine, "Cast", this);
        summonState = new SummonState(this, stateMachine, "Summon", this);
        stunnedState = new StunnedState(this, stateMachine, "Stunned", this);
        deadState = new DeadState(this, stateMachine, "Die", this);
        controlState = new ControlState(this, stateMachine, "Control", this);

        // 尝试记录初始伤害（如果 stats 存在）
        if (stats != null && stats.damage != null)
        {
            originalDamageValue = stats.damage.GetValue();
        }
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);//初始化状态

        this.faceDirection = -1;//Boss始终面向左侧
        this.faceRight = false;

        // 设置护甲值
        if (stats != null && stats.armor != null)
        {
            stats.armor.SetDefalutValue(defaultarmor); // 设置护甲值为 defaultarmor
        }
    }

    protected override void Update()
    {
        base.Update();

        
    }

    public override bool CanBeStunned()//被弹反状态优先级较高
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    // 普攻计数：普通攻击执行完时调用
    public void RegisterNormalAttack()
    {
        normalAttackCount++;
        if (normalAttackCount >= 3)
        {
            normalAttackCount = 0;
            enhancedAttackRemaining = 2; // 触发两次强化普攻
        }
    }

    public bool HasEnhancedAttackPending()
    {
        return enhancedAttackRemaining > 0;
    }

    // 在要进行强化普攻时调用，确保伤害提升只设置一次
    public void StartEnhancedBuffIfNeeded()
    {
        if (!enhancedBuffActive && enhancedAttackRemaining > 0 && stats != null && stats.damage != null)
        {
            // 记录并设置提升后的伤害（加值）
            originalDamageValue = stats.damage.GetValue();
            int increased = Mathf.CeilToInt(originalDamageValue + enhancedDamageAdd);
            stats.damage.SetDefalutValue(increased);
            stats.armor.SetDefalutValue(0);
            enhancedBuffActive = true;
        }
    }

    // 每完成一次强化普攻调用：减少计数，最后一次恢复原伤害
    public void ConsumeEnhancedAttack()
    {
        if (enhancedAttackRemaining > 0)
            enhancedAttackRemaining--;

        if (enhancedAttackRemaining <= 0 && enhancedBuffActive)
        {
            if (stats != null && stats.damage != null)
            {
                stats.damage.SetDefalutValue(originalDamageValue);
                stats.armor.SetDefalutValue(defaultarmor);
            }
            enhancedBuffActive = false;
        }
    }

    // 施法触发判定：冷却、距离和概率
    public bool CanStartCast()
    {
        // 冷却检测
        if (Time.time < lastCastTime + castCooldown) return false;

        // player 存在性检查
        if (PlayerManager.instance == null || PlayerManager.instance.player == null) return false;
        float dist = Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position);

        // 只在距离超过阈值时才有机会施法
        if (dist <= castTriggerDistance || dist >= castTriggerDistance2) return false;

        // 概率判定
        if (Random.value <= castProbability)
            return true;

        return false;
    }

    // 召唤判定
    public bool CanStartSummon()
    {
        if (minionPrefabs == null || minionPrefabs.Count == 0) return false;
        if (stats == null) return false;

        int maxHp = stats.GetMaxHealthValue();
        if (maxHp <= 0) return false;

        if (stats.currentHealth <= maxHp * summonHpThreshold &&
            Time.time >= lastSummonTime + summonCooldown)
        {
            return true;
        }

        //如果第一次低于第二个血量阈值，直接进行召唤
        if (stats.currentHealth <= maxHp * summonHpThreshold2 &&
            Time.time >= lastSummonTime2 + summonCooldown2)
        {
            return true;
        }
        

        return false;
    }

    // 召唤执行
    public void DoSummon()
    {
        if (minionPrefabs == null || minionPrefabs.Count == 0) return;

        for (int i = 0; i < minionPerSummon; i++)
        {
            GameObject prefab = minionPrefabs[Random.Range(0, minionPrefabs.Count)];
            float angle = Random.Range(0f, Mathf.PI);
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * summonRadius;
            Instantiate(prefab, pos, Quaternion.identity);
        }

        // 根据当前血量决定更新哪个冷却时间戳
        if (stats != null)
        {
            int maxHp = stats.GetMaxHealthValue();
            if (maxHp > 0 && stats.currentHealth <= maxHp * summonHpThreshold2)
            {
                // 触发第二级召唤，更新两个时间戳以防止被第一级条件立刻再触发
                lastSummonTime2 = Time.time;
                lastSummonTime = Time.time;
            }
            else
            {
                // 普通召唤，只更新普通冷却
                lastSummonTime = Time.time;
            }
        }
        else
        {
            // 兜底：至少更新普通冷却
            lastSummonTime = Time.time;
        }

        Debug.Log($"[Boss_Death] Summoned {minionPerSummon} minions at {Time.time} (HP { (stats!=null ? stats.currentHealth : -1) })");
    }

    public bool CanStartControl()
    {
        if (Time.time < lastControlTime + controlCooldown) return false;
        if (stats == null) return false;

        int maxHp = stats.GetMaxHealthValue();
        if (maxHp <= 0) return false;
        if (stats.currentHealth <= maxHp * controlHpThreshold)
        {
            return true;
        }
        return false;
    }

    
}