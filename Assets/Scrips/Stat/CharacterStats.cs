using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour//角色数据
{
    [Header("Major stats")]
    public Stat strength;//力量，负责伤害增加，爆伤
    public Stat agility;//敏捷，负责闪避
    public Stat intellegence;//智力，魔法伤害相关
    public Stat vitality;//生命力，与血量相关,每点生命力加五滴血
    public Stat magic;//魔力，与蓝条相关

    [Header("Offensive stats")]
    public Stat damage;//伤害
    public Stat critChange;//暴击机率
    public Stat critPower;//暴伤
    public Stat critEquiptment;//可以增加暴击概率的装备


    [Header("Defensive stats")]
    public Stat maxHealth;//最大血量
    public Stat armor;//护甲，与防御力相关
    public Stat evasion;//闪避


    public System.Action onHealthChanged;//血条更新委托
    



    public  int currentHealth;//当前血量

    protected virtual void Start()
    {
        critPower.SetDefalutValue(150);
        currentHealth = GetMaxHealthValue();//初始化血量

       
    }


    public virtual void DoDamage(CharacterStats _targetStats)//进行攻击数据的计算，然后输出
    {
        if (CanAvoidAttack(_targetStats))//计算是否能闪避
        {
            return;
        }


        int totalDamage = damage.GetValue() + strength.GetValue();//计算初始伤害

        if (CanCrit())
        {
            totalDamage=CalculteCriticalDamage(totalDamage);
        }

        totalDamage = CheckAromor(_targetStats, totalDamage);//计算护甲减免后的伤害

        _targetStats.TakeDamage(totalDamage);//造成最终伤害
    }


    private int CheckAromor(CharacterStats _targetStats, int totalDamage)//检查护甲，使伤害减少
    {
        totalDamage -= _targetStats.armor.GetValue();
        if (totalDamage <= 0)//防止伤害为负数
        {
            totalDamage = 1;
        }

        return totalDamage;
    }

    private bool CanAvoidAttack(CharacterStats _targetStats)//检测攻击能否被闪避
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();//闪避值为闪避加敏捷

        if (Random.Range(0,100) < totalEvasion)
        {
            Debug.Log("攻击被闪避");
            return true;
        }
        return false;
    }


    public virtual void RecoverHP()//回复血量
    {
        if (onHealthChanged == null)
        {
            Debug.Log("空引用");
        }
        if (onHealthChanged != null)//防空
        {
            onHealthChanged();
            Debug.Log("调用");
        }
    }



    public virtual void TakeDamage(int _damage)//造成伤害
    {
        currentHealth-=_damage;

      

        if(onHealthChanged != null)//防空
        {
            onHealthChanged();
        }


        if (currentHealth < 0)
        {
            Die();//死亡
        }
    }

    protected virtual void Die()
    {
        
    }


    private bool CanCrit()//判断是否暴击
    {
        int totalCritcalChance=critChange.GetValue()+critEquiptment.GetValue();

        if (Random.Range(0, 100) < totalCritcalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculteCriticalDamage(int _damage)//计算暴击最终伤害
    {
        float totalCritPower=(critPower.GetValue()+strength.GetValue())*0.01f;

        float critDamage=_damage*totalCritPower;

        return Mathf.RoundToInt(critDamage);//四舍五入

    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
}
