using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
[System.Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class Stat
{
    [JsonProperty]
    [SerializeField] private int baseValue=0;//基础值

    [JsonProperty]
    public List<int> modifiers=new List<int>();//修改器数列，储存各种额外值，如Buff


    public int GetValue()//使外界获取值,输出最终值
    {
        int finalValue = baseValue;

        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public void SetDefalutValue(int _value)//设置基础值
    {
        baseValue = _value;
    }


    public void AddModifier(int _modifier)//数据输入
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)//数据移出
    {
        modifiers.Remove(_modifier);
    }

    public void ClearModifiers()//数据清除
    {
        modifiers.Clear();
    }


    public List<int> GetModifiers()=>new List<int>(modifiers);

    public void CopyFrom(Stat other)
    {
        if (other == null) return;
        baseValue = other.baseValue;
        modifiers.Clear ();
        modifiers.AddRange(other.modifiers);
    }
}
