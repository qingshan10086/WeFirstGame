using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomMaliTrigger : MonoBehaviour
{
    private TomMali enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<TomMali>();
    }
    
    // 技能1攻击检测 - 黑色圆形
    public void Skill1AttackTrigger()
    {
        if (enemy.skill1Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.skill1Attack.position, enemy.skill1AttackRange, enemy.whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                UnityEngine.Debug.Log("TomMali技能1攻击到玩家");
                // 这里可以添加技能1的伤害逻辑
            }
        }
    }
    
    // 技能2攻击检测 - 红色圆形
    public void Skill2AttackTrigger()
    {
        if (enemy.skill2Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.skill2Attack.position, enemy.skill2AttackRange, enemy.whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                UnityEngine.Debug.Log("TomMali技能2攻击到玩家");
                // 这里可以添加技能2的伤害逻辑
            }
        }
    }
    
    // 技能3攻击检测 - 蓝色圆形
    public void Skill3AttackTrigger()
    {
        if (enemy.skill3Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.skill3Attack.position, enemy.skill3AttackRange, enemy.whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                UnityEngine.Debug.Log("TomMali技能3攻击到玩家");
                // 这里可以添加技能3的伤害逻辑
            }
        }
    }
}