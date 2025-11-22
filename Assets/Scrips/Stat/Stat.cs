using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]//相当于{get;private set;}不过更易调Bug
public class Stat //记录各种游戏战斗数据的类，如攻击伤害
{
    [SerializeField] private int baseValue;//基础值

    public List<int> modifiers;//修改器数列，储存各种额外值，如Buff


    public int GetValue()//使外界获取值,输出最终值
    {
        int finalValue = baseValue;

        foreach(int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefalutValue(int _value)
    {
        baseValue= _value;
    }


    public void AddModifier(int _modifier)//数据输入
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)//数据移出
    {
        modifiers.Remove(_modifier);
    }

}
