using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Town_trigger : MonoBehaviour
{
    private Enemy_town2 enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_town2>();
    }
     public void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.checkAttack.position, enemy.checkAttackRange);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // 获取玩家的PlayerStats组件
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                   
                    // 让玩家受到伤害
                    int totalDamage = Mathf.RoundToInt(enemy.attackDamage);
                    playerStats.TakeDamage(totalDamage);
                    
                    // 播放攻击效果
                    if (enemy.fx != null)
                    {
                        enemy.fx.StartCoroutine("FlashFX");
                    }
                    
                    if (enemy.showDebugLogs)
                    {
                        UnityEngine.Debug.Log("TownResident攻击到玩家，造成了 " + totalDamage + " 点伤害");
                    }
                }
            }
        }
    }
}
