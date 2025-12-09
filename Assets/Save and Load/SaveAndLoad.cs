using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System.IO;

public class SaveAndLoad : MonoBehaviour
{
    public static void SaveData(float xPosition, float yPosition, float zPosition,
    Stat strength, Stat agility, Stat intellegence, Stat vitality, Stat magic, Stat damage,Stat critChange,Stat critPower,Stat critEquipMent,Stat MaxHealth,Stat armor,Stat evasion,int currentHealth)
    {
        var data = new PlayerData();
        data.xPosition = xPosition;
        data.yPosition = yPosition;
        data.zPosition = zPosition;
        data.strength = strength;
        data.agility = agility;
        data.intellegence = intellegence;
        data.vitality = vitality;
        data.magic = magic;
        data.evasion =evasion;
        data.critChange = critChange;
        data.critPower = critPower;
        data.armor = armor;
        data.evasion = evasion;
        data.currentHealth = currentHealth;


        var jsonStr=JsonUtility.ToJson(data);
        var filePath = Application.persistentDataPath + "/playerData.json";
        File.WriteAllText(filePath, jsonStr);
        Debug.Log(filePath);

    }

    public static PlayerData LoadData()
    {
        var filePath = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(filePath))
        {
            var data = JsonUtility.FromJson<PlayerData>(File.ReadAllText(filePath));
            return data;
        }
        else
        {
            return null;
        }
    }
}

public class PlayerData
{
    public float xPosition;
    public float yPosition;
    public float zPosition;


    
    public Stat strength;//力量，负责伤害增加，爆伤
    public Stat agility;//敏捷，负责闪避
    public Stat intellegence;//智力，魔法伤害相关
    public Stat vitality;//生命力，与血量相关,每点生命力加五滴血
    public Stat magic;//魔力，与蓝条相关
    public Stat damage;//伤害
    public Stat critChange;//暴击机率
    public Stat critPower;//暴伤
    public Stat critEquiptment;//可以增加暴击概率的装备
    public Stat maxHealth;//最大血量
    public Stat armor;//护甲，与防御力相关
    public Stat evasion;//闪避
    public int currentHealth;//当前血量
}