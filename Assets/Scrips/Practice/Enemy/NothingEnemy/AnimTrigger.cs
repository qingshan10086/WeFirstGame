using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    private Enemy_nothing enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_nothing>();
    }
    private void AnimationTrigger()
    {
        enemy.animTriggerEvent();
    }

    private void AttackTriggerone()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
               UnityEngine.Debug.Log("1攻击到玩家");
            }
        }
    }
    private void AttackTriggerTwo()
    {
           Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.checkAttackSecond.position, enemy.checkAttackRangeSecond);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
               UnityEngine.Debug.Log("2攻击到玩家");
            }
        }
    }
    private void AttackTriggerThree()
    {
           Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.checkAttackThird.position, enemy.checkAttackRangeThird);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
               UnityEngine.Debug.Log("3攻击到玩家");
            }
        }
    }
}
