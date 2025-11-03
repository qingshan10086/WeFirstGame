using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour//角色数据
{
    public Stat strength;
    public Stat damage;//伤害
    public Stat maxHealth;//最大血量



    [SerializeField] private int currentHealth;//当前血量

    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
    }

    public virtual void DoDamage(CharacterStats _targetStats)//进行攻击数据的计算，然后输出
    {
        int totalDamage = damage.GetValue() + strength.GetValue();

        _targetStats.TakeDamage(totalDamage);
    }



    public virtual void TakeDamage(int _damage)//造成伤害
    {
        currentHealth-=_damage;

        Debug.Log(_damage);

        if (currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        throw new NotImplementedException();
    }
}
