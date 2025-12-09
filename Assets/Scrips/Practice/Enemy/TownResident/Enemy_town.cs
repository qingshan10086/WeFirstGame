using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_town : MonoBehaviour
{
    public System.Action onFlipped;//血条不翻转委托
    private GameObject player;
    private Rigidbody2D rb;
    [Header("移动参数")]
    [SerializeField] private float horizontalSpeed = 2f;      //水平移动速度
    [SerializeField] private float fixedJumpSpeed = 5f;        //固定跳跃速度
    [SerializeField] private float playerDetectionRange = 3f;  //玩家检测范围
    
    [Header("检查参数")]
    [SerializeField] protected Transform groundCheck;         //获取地面检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected Transform wallCheck;           //获取墙壁检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float groundCheckDistance;     //地面检测的距离
    [SerializeField] protected float wallCheckDiatance;       //墙壁检测的距离
    [SerializeField] protected LayerMask whatisGround;        //储存墙壁层与地面层信息，来判断是那一层
    [SerializeField] protected LayerMask whatisPlayer;        //储存玩家层信息，来判断是否检测到玩家
    [Header("攻击参数")]
    [SerializeField] protected Transform checkAttack;         //获取攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float checkAttackRange;        //攻击检测的范围  



    public int faceDirection { get; private set; } = 1;       //面对方向，初始默认向右
    protected bool faceRight = true;                          //判断是否面朝右边
    public EntityFX fx {  get; private set; }       //用来做一些光效的类，如受到攻击变白色

    public CharacterStats stats { get; private set; }//角色数据统计
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 墙壁检测，碰到墙回头
        if (IsWallDetected())
        {
            Flip();
        }
        
        // 地面检测，一落地就跳跃
        if(IsGroundDetected())
        {
            Jump();
        }
        
        // 保持水平速度
        MaintainHorizontalVelocity();
    }
    
    // 跳跃逻辑
    private void Jump()
    {
        float jumpSpeed = fixedJumpSpeed;
        
        // 如果玩家在检测范围内，计算刚好砸到玩家的跳跃速度
        if (IsPlayerClose())
        {
            jumpSpeed = CalculateJumpSpeedToPlayer();
        }
        
        // 应用跳跃速度
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }
    
    // 保持水平速度
    private void MaintainHorizontalVelocity()
    {
        // 只修改水平速度，保持垂直速度不变
        rb.velocity = new Vector2(horizontalSpeed * faceDirection, rb.velocity.y);
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
        // 假设敌人保持水平速度移动，计算到达玩家位置所需的时间
        float timeToReachPlayer = horizontalDistance / horizontalSpeed;
        
        // 计算垂直方向需要的速度，使得敌人在timeToReachPlayer时间后到达玩家高度
        // 使用公式：y = v0 * t - 0.5 * g * t^2
        // 假设敌人和玩家在同一高度（或忽略高度差）
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float requiredJumpSpeed = 0.5f * gravity * timeToReachPlayer;
        
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
    public  bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);  //该函数用来储存射线是否检测到了地面层
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, wallCheckDiatance, whatisGround);//该函数用来储存射线是否检测到了墙壁层
    protected  void OnDrawGizmos()      //该函数用来在Unity中画一条射线，不会在游戏场景中出现，来辅助射线检测，好确定射线的具体长度
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));//画地面检测线
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));//画墙壁检测线
       
      

    }
    #endregion


    #region 翻转
    public  void Flip()     //翻转函数，每次调用可以翻转一次
    {
        faceDirection = faceDirection * -1;
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped!= null)
        { 
            onFlipped();
        }
    }

    public  void FlipController(float _x)   //翻转管理器
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


   

   
   

    public void Die()
    {

    }
}



