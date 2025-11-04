using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour      //实体类，用来储存玩家和怪物共同的行为
{
    #region Component
    public Animator anim { get; private set; }     //用来获取动画机

    public Rigidbody2D rb;                         //用来获取重力组件

    public EntityFX fx {  get; private set; }       //用来做一些光效的类，如受到攻击变白色

    public CharacterStats stats { get; private set; }//角色数据统计
    public CapsuleCollider2D cd { get; private set; }
    #endregion


    public System.Action onFlipped;


    [Header("Knockback info")]                           //击退相关
    [SerializeField] protected Vector2 knockbackDirection;   //被击退方向
    protected bool isKnocked;                                //判断是否被击退
    [SerializeField] protected float knockbackDuration;      //击退持续时间


    [Header("Collision info")]                       //碰撞检测数据，来储存攻击方面的
    public Transform attackCheck;                    //获取玩家攻击检测的位置信息，会单独在Unity中设置一个子物体
    public float attackCheckRadius;                  //攻击检测的半径

    [SerializeField] protected Transform groundCheck;         //获取地面检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float groundCheckDistance;     //地面检测的距离
    [SerializeField] protected Transform wallCheck;           //获取墙壁检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] protected float wallCheckDiatance;       //墙壁检测的距离
    [SerializeField] protected LayerMask whatisGround;        //储存墙壁层与地面层信息，来判断是那一层


    public int faceDirection { get; private set; } = 1;       //面对方向，初始默认向右
    protected bool faceRight = true;                          //辅助是否翻转的数据

    protected virtual void Awake()                 
    {

    }

    protected virtual void Start()
    {
        fx = GetComponentInChildren<EntityFX>();            //获取子物体挂载的光效脚本
        anim = GetComponentInChildren<Animator>();          //获取子物体挂载的动画机组件
        rb = GetComponent<Rigidbody2D>();                   //获取重力组件
        stats = GetComponent<CharacterStats>();     //获取数据组件
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }


    #region   Collider 
    //射线检测
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);  //该函数用来储存射线是否检测到了地面层
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, wallCheckDiatance, whatisGround);//该函数用来储存射线是否检测到了墙壁层
    protected virtual void OnDrawGizmos()      //该函数用来在Unity中画一条射线，不会在游戏场景中出现，来辅助射线检测，好确定射线的具体长度
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));//画地面检测线
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));//画墙壁检测线
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);  //画攻击检测圆

    }
    #endregion


    #region 翻转
    public virtual void Flip()     //翻转函数，每次调用可以翻转一次
    {
        faceDirection = faceDirection * -1;
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped!= null)
        { 
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)   //翻转管理器
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


    public void SetVelocity(float _xVelocity, float _yVelocity)    //设置速度的函数，前一个参数为x轴速度，后一个为y轴速度
    {
        if (isKnocked)                                          //
        {
            return;
        }

        rb.velocity = new Vector2(_xVelocity, _yVelocity);

        FlipController(rb.velocity.x);
    }

    public void ZeroVelocity()              //设置0速度的函数
    {
        if (isKnocked)
        {
            return;
        }

        rb.velocity = new Vector2(0, 0);
    }

    public virtual void DamageEffect()                        //攻击效果管理函数
    {
        fx.StartCoroutine("FlashFX");                   //开始光效协程
        StartCoroutine("HitKnockback");                 //开始击退协程
        
    }

    protected virtual IEnumerator HitKnockback()        //击退协程管理
    {
        isKnocked = true;

        rb.velocity=new Vector2(knockbackDirection.x*-faceDirection,knockbackDirection.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;

    }

    public virtual void Die()
    {

    }
}
