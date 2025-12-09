using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Enemy_town2 : MonoBehaviour
{
    
    public System.Action onFlipped;//血条不翻转委托
    private GameObject player;
    private Rigidbody2D rb;
    private Animator anim;
    [Header("移动参数")]
    [SerializeField] private float horizontalSpeed = 2f;      //水平移动速度
    
    [Header("检查参数")]
    [SerializeField] protected Transform groundCheck;         //获取地面检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected Transform wallCheck;           //获取墙壁检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected Transform playerCheck;         //获取玩家检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float groundCheckDistance;     //地面检测的距离
    [SerializeField] protected float wallCheckDiatance;       //墙壁检测的距离
    [SerializeField] protected float playerCheckDistance;     //玩家检测的距离
    [SerializeField] public float attackRange;             //攻击范围
    [SerializeField] protected LayerMask whatisGround;        //储存墙壁层与地面层信息，来判断是那一层
    [SerializeField] protected LayerMask whatisPlayer;        //储存玩家层信息，来判断是否检测到玩家
    [Header("攻击参数")]
    [SerializeField] public Transform checkAttack;         //获取攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] public float checkAttackRange;        //攻击检测的范围  



    public int faceDirection { get; private set; } = 1;       //面对方向，初始默认向右
    protected bool faceRight = true;                          //判断是否面朝右边
    public EntityFX fx {  get; private set; }       //用来做一些光效的类，如受到攻击变白色

    public CharacterStats stats { get; private set; }//角色数据统计
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
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
        
        // 检测玩家
        GameObject detectedPlayer = DetectPlayer();
        
        // 根据检测结果调整移动方向
        if (detectedPlayer != null)
        {
            // 检测到玩家，靠近玩家行走
            float playerDirection = detectedPlayer.transform.position.x - transform.position.x > 0 ? 1 : -1;
            
            // 确保敌人面向玩家
            if (playerDirection > 0 && !faceRight)
            {
                Flip();
            }
            else if (playerDirection < 0 && faceRight)
            {
                Flip();
            }
            
            // 如果玩家在攻击范围内，准备攻击
            if (Mathf.Abs(detectedPlayer.transform.position.x - transform.position.x) <= attackRange)
            {
                // 玩家在攻击范围内，这里可以调用攻击逻辑
                ZeroVelocity();
                anim.SetBool("attack", true);

            }
            else
            {
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && 
    !anim.IsInTransition(0))
                // 玩家不在攻击范围内，靠近玩家
                rb.velocity = new Vector2(horizontalSpeed * playerDirection, rb.velocity.y);
                anim.SetBool("attack", false);
            }
        }
        else
        {
            // 未检测到玩家，正常行走
            rb.velocity = new Vector2(horizontalSpeed * faceDirection, rb.velocity.y);
        }
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
    
    // 检测玩家的射线方法
    public GameObject DetectPlayer()
    {
        // 向当前面对方向发射射线检测玩家
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * faceDirection, playerCheckDistance, whatisPlayer);
        
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return hit.collider.gameObject;
        }
        
        return null;
    }
    
    protected  void OnDrawGizmos()      //该函数用来在Unity中画一条射线，不会在游戏场景中出现，来辅助射线检测，好确定射线的具体长度
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));//画地面检测线
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));//画墙壁检测线
        
        // 画玩家检测射线
        if (playerCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance * faceDirection, playerCheck.position.y));
        }
        
        // 画攻击范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //画攻击范围
         Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkAttack.position, checkAttackRange);
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

    public void ZeroVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }

   

   
   

    public void Die()
    {

    }
}
