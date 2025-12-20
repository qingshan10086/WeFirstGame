// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Entity : MonoBehaviour      //ʵ���࣬����������Һ͹��ﹲͬ����Ϊ
// {
//     #region Component
//     public Animator anim { get; private set; }     //������ȡ������

//     public Rigidbody2D rb;                         //������ȡ�������

//     public EntityFX fx {  get; private set; }       //������һЩ��Ч���࣬���ܵ��������ɫ

//     public CharacterStats stats { get; private set; }//��ɫ����ͳ��
//     public CapsuleCollider2D cd { get; private set; }//������ײ��
//     #endregion


//     public System.Action onFlipped;//Ѫ������תί��


//     [Header("Knockback info")]                           //�������
//     [SerializeField] protected Vector2 knockbackDirection;   //�����˷���
//     protected bool isKnocked;                                //�ж��Ƿ񱻻���
//     [SerializeField] protected float knockbackDuration;      //���˳���ʱ��


//     [Header("Collision info")]                       //��ײ������ݣ������湥�������
//     public Transform attackCheck;                    //��ȡ��ҹ�������λ����Ϣ���ᵥ����Unity������һ��������
//     public float attackCheckRadius;                  //�������İ뾶

//     [SerializeField] protected Transform groundCheck;         //��ȡ�������λ����Ϣ���ᵥ����Unity������һ��������
//     [SerializeField] protected float groundCheckDistance;     //������ľ���
//     [SerializeField] protected Transform wallCheck;           //��ȡǽ�ڼ���λ����Ϣ���ᵥ����Unity������һ��������
//     [SerializeField] protected float wallCheckDiatance;       //ǽ�ڼ��ľ���
//     [SerializeField] protected LayerMask whatisGround;        //����ǽ�ڲ���������Ϣ�����ж�����һ��


//     public int faceDirection { get; private set; } = 1;       //��Է��򣬳�ʼĬ������
//     protected bool faceRight = true;                          //�ж��Ƿ��泯�ұ�

//     protected virtual void Awake()                 
//     {

//     }

//     protected virtual void Start()
//     {
//         fx = GetComponentInChildren<EntityFX>();            //��ȡ��������صĹ�Ч�ű�
//         anim = GetComponentInChildren<Animator>();          //��ȡ��������صĶ��������
//         rb = GetComponent<Rigidbody2D>();                   //��ȡ�������
//         stats = GetComponent<CharacterStats>();     //��ȡ�������
//         cd = GetComponent<CapsuleCollider2D>();
//     }

//     protected virtual void Update()
//     {

//     }


//     #region   Collider 
//     //���߼��
//     public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);  //�ú����������������Ƿ��⵽�˵����
//     public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, wallCheckDiatance, whatisGround);//�ú����������������Ƿ��⵽��ǽ�ڲ�
//     protected virtual void OnDrawGizmos()      //�ú���������Unity�л�һ�����ߣ���������Ϸ�����г��֣����������߼�⣬��ȷ�����ߵľ��峤��
//     {
//         Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));//����������
//         Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDiatance * faceDirection, wallCheck.position.y));//��ǽ�ڼ����
//         Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);  //���������Բ

//     }
//     #endregion


//     #region ��ת
//     public virtual void Flip()     //��ת������ÿ�ε��ÿ��Է�תһ��
//     {
//         faceDirection = faceDirection * -1;
//         faceRight = !faceRight;
//         transform.Rotate(0, 180, 0);
//         if (onFlipped!= null)
//         { 
//             onFlipped();
//         }
//     }

//     public virtual void FlipController(float _x)   //��ת������
//     {
//         if (_x > 0 && !faceRight)                   
//         {
//             Flip();
//         }
//         if (_x < 0 && faceRight)
//         {
//             Flip();
//         }
//     }
//     #endregion


//     public void SetVelocity(float _xVelocity, float _yVelocity)    //�����ٶȵĺ�����ǰһ������Ϊx���ٶȣ���һ��Ϊy���ٶ�
//     {
//         if (isKnocked)                                          //
//         {
//             return;
//         }

//         rb.velocity = new Vector2(_xVelocity, _yVelocity);

//         FlipController(rb.velocity.x);
//     }

//     public void ZeroVelocity()              //����0�ٶȵĺ���
//     {
//         if (isKnocked)
//         {
//             return;
//         }

//         rb.velocity = new Vector2(0, 0);
//     }

//     public virtual void DamageEffect()                        //����Ч����������
//     {
//         fx.StartCoroutine("FlashFX");                   //��ʼ��ЧЭ��
//         StartCoroutine("HitKnockback");                 //��ʼ����Э��
        
//     }

//     protected virtual IEnumerator HitKnockback()        //����Э�̹���
//     {
//         isKnocked = true;

//         rb.velocity=new Vector2(knockbackDirection.x*-faceDirection,knockbackDirection.y);

//         yield return new WaitForSeconds(knockbackDuration);
//         isKnocked = false;

//     }

//     public virtual void Die()
//     {

//     }
// }
