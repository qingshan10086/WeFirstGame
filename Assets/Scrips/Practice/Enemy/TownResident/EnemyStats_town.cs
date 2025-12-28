using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats_town : CharacterStats//城镇敌人的属性类
{
    private Enemy_town enemy_town;
    private Enemy_town2 enemy_town2;

    protected override void Start()
    {
        base.Start();
        
        // 尝试获取Enemy_town或Enemy_town2组件
        enemy_town = GetComponent<Enemy_town>();
        enemy_town2 = GetComponent<Enemy_town2>();
    }

    public override void TakeDamage(int _damage)//受到伤害
    {
        base.TakeDamage(_damage);

        // 调用对应的伤害效果
        if (enemy_town != null)
        {
            enemy_town.DamageEffect();
        }
        else if (enemy_town2 != null)
        {
            enemy_town2.DamageEffect();
        }
    }

    protected override void Die()
    {
        base.Die();
        
        // 调用对应的死亡逻辑
        if (enemy_town != null)
        {
            enemy_town.Die();
        }
        else if (enemy_town2 != null)
        {
            enemy_town2.Die();
        }
    }
}
