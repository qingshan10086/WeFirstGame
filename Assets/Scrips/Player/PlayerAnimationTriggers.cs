using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour//动画机触发类，其内部函数都是在Unity动画animation中调用，其挂载在子物体Animator上
{
    private Player player => GetComponentInParent<Player>();//获取父物体
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }



    private void RecoverHP()
    {
        player.stats.RecoverHP();
    }


    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);//攻击检测触发和其范围

        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                player.stats.DoDamage(_target);

               
                
            }
        }
    }
}
