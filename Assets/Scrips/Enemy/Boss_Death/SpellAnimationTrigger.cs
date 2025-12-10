using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAnimationTrigger : MonoBehaviour
{
    private SpriteRenderer sr;//精灵渲染组件
    private Animator anim;

    private float colorLoosingSpeed = 0.1f;//颜色消失速度
    private float cloneTimer;//辅助计算克隆持续时间的

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();//获取精灵渲染器
        anim = GetComponent<Animator>();//获取动画组件
    }
    private Spell enemy => GetComponentInParent<Spell>();

    private void AnimationTrigger()//动画触发完成函数
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger()//攻击触发函数
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);//获取攻击范围内部的碰撞器

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)//如果有玩家则造成相应攻击光效
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target);


            }

        }
    }

}
