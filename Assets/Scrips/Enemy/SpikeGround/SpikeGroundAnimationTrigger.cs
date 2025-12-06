using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGroundAnimationTrigger : MonoBehaviour
{
    private SpikeGround enemy => GetComponentInParent<SpikeGround>();//获取父物体骷髅兵类

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
