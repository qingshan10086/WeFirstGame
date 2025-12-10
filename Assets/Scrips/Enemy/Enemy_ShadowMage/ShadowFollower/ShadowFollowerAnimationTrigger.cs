using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowFollowerAnimationTrigger : MonoBehaviour//拿来在动画机里面判断攻击
{
    private ShadowFollower enemy=>GetComponentInParent<ShadowFollower>();

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
