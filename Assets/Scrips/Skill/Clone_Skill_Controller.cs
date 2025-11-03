using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour//克隆技能管理器
{
    private SpriteRenderer sr;//精灵渲染组件
    private Animator anim;

    [SerializeField] private float colorLoosingSpeed;//颜色消失速度


    private float cloneTimer;//辅助计算克隆持续时间的
    [SerializeField] private Transform attackCheck;                    //获取玩家攻击检测的位置信息，会单独在Unity中设置一个子物体
    [SerializeField] private float attackCheckRadius;                  //攻击检测的半径
    private Transform closestEnemy;//最近的敌人位置


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();//获取精灵渲染器
        anim = GetComponent<Animator>();//获取动画组件
    }

    private void Update()
    {
        cloneTimer-= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));//颜色逐渐变淡
        }

        if (sr.color.a <= 0)
        {
            Destroy(gameObject);
        }

    }

    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack)
    {
        if (_canAttack)
        {
            anim.SetInteger("AttackNumber", Random.Range(1, 4));//使克隆体随机攻击
        }

        transform.position = _newTransform.position;//获取玩家位置
        cloneTimer = _cloneDuration;

        FaceClosestTarget();//使克隆体面向最近的敌人
    }

    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;
        
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);//攻击检测触发和其范围

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Enemy>().DamageEffect();//攻击敌人
            }
        }
    }

    private void FaceClosestTarget()//面向敌人的函数
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);//检测范围

        float closestDistance = Mathf.Infinity;//

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy=hit.transform;
                }
            }
        }


        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                transform.Rotate(0, 180, 0);
            }
        }
    }



}
