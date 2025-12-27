
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class TomMali : Enemy
{
     public System.Action onFlipped;//血条不翻转委托
    private GameObject player;
    
    [Header("移动参数")]
    [SerializeField] private float horizontalSpeed = 2f;      //水平移动速度
    [SerializeField] private float fixedJumpSpeed = 5f;        //固定跳跃速度
    [SerializeField] private float playerDetectionRange = 3f;  //玩家检测范围
    
    [Header("检查参数")]
    [SerializeField] protected Transform playerCheck;         //获取玩家检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float playerCheckDistance;     //玩家检测的距离
    [SerializeField] public LayerMask whatisPlayer;        //储存玩家层信息，来判断是否检测到玩家
    [Header("攻击参数")]
    [SerializeField] protected Transform checkAttack;         //获取攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float checkAttackRange;        //攻击检测的范围  
    [SerializeField] public float attackRange;                //攻击范围
    [SerializeField] public float attackDamage = 10f;      //基础攻击伤害
    [SerializeField] public float skill1Damage = 15f;      //技能1伤害
    [SerializeField] public float skill2Damage = 20f;      //技能2伤害
    [SerializeField] public float skill3Damage = 25f;      //技能3伤害
    
    [Header("技能攻击范围")]
    // 技能1攻击范围 - 黑色圆形
     [SerializeField] public Transform skill1Attack ;
    [SerializeField] public float skill1AttackRange = 2f;  // 技能1攻击半径
   
    
    // 技能2攻击范围 - 红色圆形
    [SerializeField] public Transform skill2Attack;
    [SerializeField] public float skill2AttackRange = 3f;  // 技能2攻击半径
    
    // 技能3攻击范围 - 蓝色圆形
    [SerializeField] public Transform skill3Attack;
    [SerializeField] public float skill3AttackRange = 2.5f;  // 技能3攻击半径
    
    [Header("技能4配置 - 召唤小怪")]
    // 技能4 - 召唤敌人预制体
    [SerializeField] public GameObject enemyTownPrefab;  // Enemy_town预制体
    [SerializeField] public GameObject enemyTown2Prefab; // Enemy_town2预制体
    [SerializeField] public int minSummonCount = 2;      // 最小召唤数量
    [SerializeField] public int maxSummonCount = 5;      // 最大召唤数量
    [SerializeField] public float summonRange = 3f;      // 召唤范围
    [SerializeField] public float skill4Cooldown = 60f;  // 技能4冷却时间（1分钟）
    [SerializeField] private float skill4Probability = 0.2f; // 技能4选择概率

    public EnemyStats TomStats;
    [SerializeField] private bool isJumpingToPlayer = false;                  //是否正在向玩家跳跃
    
    // 技能随机释放相关变量
    private bool isPerformingSkill = false;                 //是否正在执行技能
    private bool isRestingAfterSkill = false;               //技能执行后是否正在休息
    [Header("技能配置")]
    [SerializeField] private float skillRestDuration = 2f;   //技能执行后的休息时间
    [SerializeField] public float skillDamageCooldown = 0.5f; //技能伤害冷却时间，避免帧伤
    private float currentRestTimer;                          //当前技能休息计时器
    private bool skillOneSelected = false;                 //是否选择了技能一
    private bool skillThreeSelected = false;               //是否选择了技能三
    private bool skillSecondSelected = false;                 //是否选择了技能二
    private bool skillOneDirectionSet = false; // 标记技能一是否已经确定朝向
    private bool skillFourSelected = false;                //是否选择了技能四
    private float skill4CooldownTimer = 0f;  // 技能4冷却计时器
    private bool diesign = false;
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        TomStats = GetComponent<EnemyStats>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(diesign)
        {
            return;
        }
        if (TomStats.currentHealth <= 0)
        {
            Die();
            diesign = true;
        }
        // 不调用base.Update()，避免Enemy类中的状态机空引用错误
        // 处理技能休息时间
        if (isRestingAfterSkill)
        {
            currentRestTimer -= Time.deltaTime;
            if (currentRestTimer <= 0)
            {
                isRestingAfterSkill = false;
            }
        }
        
        // 技能随机选择和冷却逻辑
        UpdateSkillSelection();
        
        // 执行选择的技能
        if (isPerformingSkill)
        {
            Debug.Log("正在执行技能: 技能1=" + skillOneSelected + ", 技能2=" + skillSecondSelected + ", 技能3=" + skillThreeSelected + ", 技能4=" + skillFourSelected);
            if(skill4CooldownTimer <= 0)
            {
                PerformSkillFour();
            }
            else if (skillOneSelected)
            {
                PerformSkillOne();
            }
            else if (skillThreeSelected)
            {
                PerformSkillThree();
            }
            else if (skillSecondSelected)
            {
                Debug.Log("执行技能2");
                PerformSkillTwo();
            }
        }
        else
        {
            // 没有执行技能时，保持静止
            ZeroVelocity();
        }
        
        //技能四
        //召唤小怪
        // 技能移动逻辑将在这里实现
        // 目前只保留墙壁检测功能
        // 更新技能4冷却计时器
       skill4CooldownTimer-=Time.deltaTime;
        
        if (IsWallDetected())
        {
            Flip();
        }
    }
    
    // 更新技能选择
    private void UpdateSkillSelection()
    {
        // 如果已经在执行技能，不进行新的技能选择
        if (isPerformingSkill)
        {
            return;
        }
        
        // 如果正在休息，继续计时
        if (isRestingAfterSkill)
        {
            currentRestTimer -= Time.deltaTime;
            if (currentRestTimer <= 0)
            {
                isRestingAfterSkill = false;
            }
            return;
        }
        
        // 检测玩家是否在攻击范围内
        GameObject detectedPlayer = DetectPlayer();
        bool isPlayerInAttackRange = false;
        
        if (detectedPlayer != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, detectedPlayer.transform.position);
            isPlayerInAttackRange = distanceToPlayer <= attackRange;
            Debug.Log("检测到玩家，距离: " + distanceToPlayer + "，攻击范围: " + attackRange + "，是否在攻击范围内: " + isPlayerInAttackRange);
        }
        
        // 生成随机数
        float randomChoice = Random.value;
        Debug.Log("随机选择值: " + randomChoice + "，玩家是否存在: " + (detectedPlayer != null) + "，是否在攻击范围内: " + isPlayerInAttackRange);
        
        // 检查是否可以选择技能4（冷却完成）
        bool canUseSkill4 = skill4CooldownTimer >= skill4Cooldown;
        
        // 调整技能选择概率，确保只有在攻击范围内才能选择技能2
        if (detectedPlayer == null)
        {
            // 检测不到玩家时：只能选择技能1或技能3
            Debug.Log("检测不到玩家，准备选择技能1或技能3");
            if (randomChoice < 0.5f)
            {
                // 50%概率选择技能一
                skillOneSelected = true;
                skillThreeSelected = false;
                skillSecondSelected = false;
        skillFourSelected = false;
                Debug.Log("选择了技能1");
            }
            else
            {
                // 50%概率选择技能三
                skillOneSelected = false;
                skillThreeSelected = true;
                skillSecondSelected = false;
                Debug.Log("选择了技能3");
            }
        }
        else if (isPlayerInAttackRange)
        {
            // 检测到玩家且在攻击范围内：只能选择技能1或技能2，技能3不可能被选择
            Debug.Log("检测到玩家且在攻击范围内，准备选择技能1或技能2");
            if (randomChoice < 0.2f)
            {
                // 20%概率选择技能一
                skillOneSelected = true;
                skillThreeSelected = false;
                skillSecondSelected = false;
                Debug.Log("选择了技能1");
            }
            else
            {
                // 80%概率选择技能二
                skillOneSelected = false;
                skillThreeSelected = false;
                skillSecondSelected = true;
                Debug.Log("选择了技能2");
            }
        }
        else
        {
            // 检测到玩家但不在攻击范围内：只能选择技能1或技能3
            Debug.Log("检测到玩家但不在攻击范围内，准备选择技能1或技能3");
            if (randomChoice < 0.6f)
            {
                // 60%概率选择技能一
                skillOneSelected = true;
                skillThreeSelected = false;
                skillSecondSelected = false;
                Debug.Log("选择了技能1");
            }
            else
            {
                // 40%概率选择技能三
                skillOneSelected = false;
                skillThreeSelected = true;
                skillSecondSelected = false;
                Debug.Log("选择了技能3");
            }
        }
        
        // 开始执行技能
        isPerformingSkill = true;
    }
    
    // 执行技能一：向面对方向冲刺
    private void PerformSkillOne()
    {
        // 检查是否在地面且竖直方向速度为0，否则无法执行技能一
        if (!IsGroundDetected() || Mathf.Abs(rb.velocity.y) > 0.01f)
        {
            // 不满足条件，取消技能执行
            EndSkill();
            return;
        }
        
        // 设置动画状态
        anim.SetBool("skill1", true);
        
        // 仅在技能一开始时确定朝向
        if (!skillOneDirectionSet)
        {
            // 检测玩家是否在攻击范围内，如果在，则优先面向玩家
            GameObject detectedPlayer = DetectPlayer();
            if (detectedPlayer != null)
            {
                float playerDirection = detectedPlayer.transform.position.x - transform.position.x > 0 ? 1 : -1;
                FlipController(playerDirection);
            }
            // 标记朝向已确定
            skillOneDirectionSet = true;
        }
        
        // 向确定的方向冲刺
        if (IsGroundDetected() && !IsWallDetected())
        {
            SetVelocity(10 * faceDirection, rb.velocity.y);
        }
        else if(IsWallDetected())
        {
            EndSkill();
        }
        
        // 技能1攻击检测 - 黑色圆形
        CheckSkill1Attack();
    }
    
    // 技能1攻击检测
    private void CheckSkill1Attack()
    {
        if (skill1Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(skill1Attack.position, skill1AttackRange, whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("技能1攻击到玩家");
                // 在这里可以添加伤害逻辑
            }
        }
    }
    
    // 结束技能并进入休息状态
    private void EndSkill()
    {
        ZeroVelocity();
        anim.SetBool("skill1", false);
        anim.SetBool("skill2", false);
        anim.SetBool("skill3", false);
        anim.SetBool("skill4", false);
        isPerformingSkill = false;
        skillOneSelected = false;
        skillThreeSelected = false;
        skillSecondSelected = false;
        
        // 重置技能一的朝向标记
        skillOneDirectionSet = false;
        
        // 开始技能后休息
        isRestingAfterSkill = true;
        currentRestTimer = skillRestDuration;
    }
    
    // 执行技能三：向玩家跳跃
    private void PerformSkillThree()
    {
        Skill3();
    }
    
    // 执行技能二：突刺攻击
    private void PerformSkillTwo()
    {
        Skill2();
    }

    // 技能2：检测玩家并发起攻击
    private void Skill2()
    {
        // 检测玩家
        GameObject detectedPlayer = DetectPlayer();
        
        if (detectedPlayer != null)
        {
            // 确保敌人面向玩家
            float playerDirection = detectedPlayer.transform.position.x - transform.position.x > 0 ? 1 : -1;
            FlipController(playerDirection);
            
            // 如果玩家在攻击范围内，准备攻击
            if (Vector2.Distance(transform.position, detectedPlayer.transform.position) <= attackRange)
            {
                // 玩家在攻击范围内，这里可以调用攻击逻辑
                Debug.Log("玩家在攻击范围内,skill2已准备");
                ZeroVelocity();
                anim.SetBool("skill2", true);
                Debug.Log("发起攻击");
                
                // 技能2攻击检测 - 红色圆形
                CheckSkill2Attack();
                
                // 检查动画是否播放完成，如果完成则结束技能
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("skill2") && 
                   anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && 
                   !anim.IsInTransition(0))
                {
                    EndSkill();
                }
            }
            else
            {
                // 玩家不在攻击范围内，取消技能
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && 
    !anim.IsInTransition(0))
                EndSkill();
            }
        }
        else
        {
            // 玩家不存在，取消技能
            EndSkill();
        }
    }
    
    // 技能2攻击检测
    private void CheckSkill2Attack()
    {
        if (skill2Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(skill2Attack.position, skill2AttackRange, whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("技能2攻击到玩家");
                // 在这里可以添加伤害逻辑
            }
        }
    }
    
    // 技能3：向玩家跳跃
    private void Skill3()
    {
        // 设置技能3动画参数
        anim.SetBool("skill3", true);
        
        if (IsGroundDetected() && !isJumpingToPlayer)
        {
            if (player != null)
            {
                // 如果玩家存在，向玩家跳跃
                JumpToPlayer();
                isJumpingToPlayer = true;
            }
            else
            {
                // 如果玩家不存在，随机向一个方向跳跃
                float randomDirection = Random.value > 0.5f ? 1 : -1;
                float jumpSpeed = fixedJumpSpeed;
                float horizontalVelocity = horizontalSpeed * randomDirection * 2;
                SetVelocity(horizontalVelocity, jumpSpeed);
                isJumpingToPlayer = true;
            }
        }
        
        // 如果正在跳跃且落地了，静止
        if (isJumpingToPlayer && IsGroundDetected() && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            ZeroVelocity();
            isJumpingToPlayer = false;
            
            // 技能3攻击检测 - 蓝色圆形
            CheckSkill3Attack();
            
            // 结束技能并进入休息状态
            EndSkill();
        }
    }
    
    // 技能3攻击检测
    private void CheckSkill3Attack()
    {
        if (skill3Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(skill3Attack.position, skill3AttackRange, whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("技能3攻击到玩家");
                // 在这里可以添加伤害逻辑
            }
        }
    }
    
    // 检测玩家是否在近距离范围内
    private bool IsPlayerClose()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return false;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= playerDetectionRange;
    }
    
    // 跳跃到玩家位置
    private void JumpToPlayer()
    {
        if (player == null) return;
        
        float jumpSpeed = fixedJumpSpeed;
        jumpSpeed = CalculateJumpSpeedToPlayer();
        
        // 计算水平方向速度
        // 假设玩家静止，使用固定跳跃时间计算水平速度
        float horizontalDistance = player.transform.position.x - transform.position.x;
        float jumpTime = 1.0f; // 与CalculateJumpSpeedToPlayer中使用的固定跳跃时间一致
        float horizontalVelocity = horizontalDistance / jumpTime;
        
        // 应用速度
        SetVelocity(horizontalVelocity, jumpSpeed);
    }
    
    // 计算刚好砸到玩家的跳跃速度
    private float CalculateJumpSpeedToPlayer()
    {
        if (player == null) return fixedJumpSpeed;
        
        // 获取玩家和敌人的位置
        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.transform.position;
        
        // 计算水平距离
        float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);
        
        // 使用物理公式计算所需的跳跃速度
        // 假设玩家静止，计算到达玩家位置所需的跳跃速度
        // 简化计算：使用固定的跳跃时间
        float jumpTime = 1.0f; // 固定跳跃时间
        
        // 计算垂直方向需要的速度
        // 使用公式：y = v0 * t - 0.5 * g * t^2
        // 假设敌人和玩家在同一高度（或忽略高度差）
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float requiredJumpSpeed = 0.5f * gravity * jumpTime;
        
        // 确保跳跃速度至少为固定跳跃速度
        return Mathf.Max(requiredJumpSpeed, fixedJumpSpeed);
    }

    // 检测怪物触发器与玩家碰撞体的接触
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("碰撞到玩家");
        }
    }


     #region   Collider 
    //射线检测
    public override bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, wallCheckDiatance, whatisGround);//该函数用来储存射线是否检测到了墙壁层
    
    // 检测玩家的射线方法（双向检测 + 球形检测）
    public GameObject DetectPlayer()
    {
        // 同时向左右两个方向发射射线检测玩家
        RaycastHit2D hitRight = Physics2D.Raycast(playerCheck.position, Vector2.right, playerCheckDistance, whatisPlayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(playerCheck.position, Vector2.left, playerCheckDistance, whatisPlayer);
        
        // 检查右侧射线是否命中玩家
        if (hitRight.collider != null && hitRight.collider.CompareTag("Player"))
        {
            return hitRight.collider.gameObject;
        }
        
        // 检查左侧射线是否命中玩家
        if (hitLeft.collider != null && hitLeft.collider.CompareTag("Player"))
        {
            return hitLeft.collider.gameObject;
        }
        
        // 如果射线检测失败，使用球形检测作为补充
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, playerCheckDistance, whatisPlayer);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return collider.gameObject;
            }
        }
        
        return null;
    }
    
    

    // 执行技能四：召唤Enemy_town和Enemy_town2预制体
    private void PerformSkillFour()
    {
        // 设置技能4动画状态
        anim.SetBool("skill4", true);
        skill4CooldownTimer = skill4Cooldown;
        // 计算召唤数量（在minSummonCount和maxSummonCount之间随机）
        int summonCount = Random.Range(minSummonCount, maxSummonCount + 1);
        Debug.Log("技能4：召唤" + summonCount + "个敌人");

        // 召唤指定数量的敌人
        for (int i = 0; i < summonCount; i++)
        {
            // 随机选择要召唤的敌人类型
            GameObject enemyPrefab = Random.value > 0.5f ? enemyTownPrefab : enemyTown2Prefab;

            // 生成随机位置（在召唤范围内）
            Vector2 randomOffset = new Vector2(
                Random.Range(-summonRange, summonRange),
                Random.Range(-summonRange, summonRange)
            );
            Vector2 summonPosition = (Vector2)transform.position + randomOffset;

            // 实例化敌人预制体
            if (enemyPrefab != null)
            {
                Object.Instantiate(enemyPrefab, summonPosition, Quaternion.identity);
                Debug.Log("成功召唤" + enemyPrefab.name + "到位置：" + summonPosition);
            }
        }

        // 技能4执行完成后直接结束技能
        EndSkill();
    }
    protected override void OnDrawGizmos()
    {
        
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));//画墙壁检测线
          // 画玩家检测射线（双向）
        if (playerCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance, playerCheck.position.y));
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x - playerCheckDistance, playerCheck.position.y));
        }
        
        // 画攻击范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 画玩家检测范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        
        // 画技能1攻击范围 - 黑色圆形
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(skill1Attack.position, skill1AttackRange);
        
        // 画技能2攻击范围 - 红色圆形
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(skill2Attack.position, skill2AttackRange);
        
        // 画技能3攻击范围 - 蓝色圆形
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(skill3Attack.position, skill3AttackRange);
    
    }
        
      
    #endregion


    #region 翻转
    public override void Flip()     //翻转函数，每次调用可以翻转一次
    {
       base.Flip();
    }

    public override void FlipController(float _x)   //翻转管理器
    {
        if (_x > 0 && !faceRight)                    
        {
            Flip();
        }
        if (_x < 0 && faceRight)
        {
            Flip();
        }
    }
    #endregion
    
    #region 移动控制
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    
    public void ZeroVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }
    #endregion


    public override void Die()
    {
        base.Die();
        ZeroVelocity();
        rb.bodyType = RigidbodyType2D.Static; // 死亡后禁用物理
        cd.enabled = false; // 死亡后禁用碰撞器
        
        // 禁用所有子物体的碰撞器（如果有）
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in childColliders)
        {
            collider.enabled = false;
        }
        
        // 延迟销毁物体，确保死亡动画播放完成
        StartCoroutine(DestroyAfterDelay(2f));
    }
    



private IEnumerator DestroyAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    Object.Destroy(this.gameObject);
}
}

