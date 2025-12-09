using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town_trigger : MonoBehaviour
{
    private Enemy_town2 enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_town2>();
    }
     private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.checkAttack.position,     enemy.checkAttackRange);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
               UnityEngine.Debug.Log("TownResident攻击到玩家");
            }
        }
    }
}
