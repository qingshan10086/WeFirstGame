using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMist : MonoBehaviour
{
    public System.Action onFlipped;//血条不翻转委托
    public GameObject player;
    public Rigidbody2D rb;
    [Header("移动参数")]
    [SerializeField] private float horizontalSpeed = 2f;      //水平移动速度
    [SerializeField] private float fixedJumpSpeed = 5f;        //固定跳跃速度
    [SerializeField] public float playerDetectionRange = 3f;  //玩家检测范围

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
    public EntityFX fx { get; private set; }       //用来做一些光效的类，如受到攻击变白色
    public CharacterStats stats { get; private set; }//角色数据统计
    
    private RedMistStateMachine stateMachine; // 状态机实例
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
        stats = GetComponent<CharacterStats>();
        
        // 初始化状态机
        stateMachine = new RedMistStateMachine(this);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // 启动状态机
        stateMachine.Start();
    }
    
    // Update is called once per frame
    void Update()
    {
        // 更新状态机
        stateMachine.Update();
    }
    
    // 保持水平速度
    public void MaintainHorizontalVelocity()
    {
        // 只修改水平速度，保持垂直速度不变
        rb.velocity = new Vector2(horizontalSpeed * faceDirection, rb.velocity.y);
    }
    
    // 选择状态的函数
    public void SelectState(RedMistState state)
    {
        stateMachine.ChangeState(state);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("碰撞到玩家");
        }
    }



    #region   Collider
    // 射线检测
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);  //该函数用来储存射线是否检测到了地面层
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, wallCheckDiatance, whatisGround);//该函数用来储存射线是否检测到了墙壁层

    //绘制射线 gizmos
    protected void OnDrawGizmos()      //该函数用来在Unity中画一条射线，不会在游戏场景中出现，来辅助射线检测，好确定射线的具体长度
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));//画地面检测线
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));//画墙壁检测线
    }
    #endregion


    #region 翻转
    public void Flip()     //翻转函数，每次调用可以翻转一次
    {
        faceDirection = faceDirection * -1;
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped != null)
        {
            onFlipped();
        }
    }
    #endregion
}