using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour//角色数据
{
    public Stat damage;//伤害
    public Stat maxHealth;//最大血量



    [SerializeField] private int currentHealth;//当前血量

    private void Start()
    {
        currentHealth = maxHealth.GetValue();
    }


    public virtual void TakeDamage(int _damage)//造成伤害
    {
        currentHealth-=_damage;

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
