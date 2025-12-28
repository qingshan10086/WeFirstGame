using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity//���е��˵ĸ���
{
    [SerializeField] public LayerMask whatIsPlayer;//��ȡ������ڲ�

    [Header("Stunned info")]//�������������
    public float stunDuration;//�ɱ���������ʱ��
    public Vector2 stunDirection;//�ɱ���������
    protected bool canBeStunned;//�ܷ񱻵���
    [SerializeField] protected GameObject counterImage;//������Ч


    [Header("Move info")]//�ƶ�����
    public float moveSpeed;//�ƶ��ٶ�
    public float idleTime;//վ��ʱ��
    public float battleTime;//ս��״̬����ʱ��

    [Header("Attack info")]//��������
    public float attackDistance;//��������
    public float attackCooldown;//������ȴ
    [HideInInspector]public float lastTimeAttacked;//��һ�ι���ʱ��


    public EnemyStateMachine stateMachine { get; private set; }//��������״̬��


    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();//��ȡ����״̬��
    }
    
    


 
    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Updata();

        
    }

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * faceDirection, 50, whatIsPlayer);//�����Ƿ��⵽��ҵ����߽��

    public virtual void AnimationFinishTrigger()=>stateMachine.currentState.AnimationFinishTrigger();//���ܶ����Ƿ���ɴ����ĺ���


    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();//���ո���Ļ��ߣ�������⣬ǽ�ڼ��

        Gizmos.color= Color.yellow;//������
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x + attackDistance * faceDirection, transform.position.y));//����������ߵĳ���
    }

    public virtual bool CanBeStunned()//�ж��ܷ񱻵����ĺ���
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    public virtual void OpenCounterAttackWindow()//�򿪿��Ա�����������һ��С���
    {
        counterImage.SetActive(true);
        canBeStunned = true;
    }
    public virtual void CloseCounterAttackWindow()//�رտ��Ա�������ʧ��һ��С���
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }


}
