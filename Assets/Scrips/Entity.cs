using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Component
    public Animator anim { get; private set; }

    public Rigidbody2D rb;
    #endregion



    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDiatance;
    [SerializeField] protected LayerMask whatisGround;


    public int faceDirection { get; private set; } = 1;
    protected bool faceRight = true;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {

    }


    #region   Collider 
    //ÉäÏß¼ì²â
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, wallCheckDiatance, whatisGround);
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);

    }
    #endregion


    #region Flip
    public virtual void Flip()
    {
        faceDirection = faceDirection * -1;
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
    }

    public virtual void FlipController(float _x)
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


    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);

        FlipController(rb.velocity.x);
    }

    public void ZeroVelocity() => rb.velocity = new Vector2(0, 0);


    public virtual void Damage()
    {
        Debug.Log(gameObject.name)
    }
}
