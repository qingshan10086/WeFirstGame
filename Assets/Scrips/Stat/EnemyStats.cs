using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats :CharacterStats//敌人的数据
{
    private Enemy enemy;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);


        enemy.DamageEffect();//攻击效果
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();
    }
}
