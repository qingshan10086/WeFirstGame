using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class Button : MonoBehaviour
{
   public void SaveGame()
    {
        //玩家位置
        var xPosition = PlayerManager.instance.player.transform.position.x;
        var yPosition = PlayerManager.instance.player.transform.position.y;
        var zPosition = PlayerManager.instance.player.transform.position.z;
        //玩家数据
        var strength = PlayerManager.instance.playerStats.strength;
        var agility=PlayerManager.instance.playerStats.agility;
        var intellegence=PlayerManager.instance.playerStats.intellegence;
        var vitality=PlayerManager.instance.playerStats.vitality;
        var magic=PlayerManager.instance.playerStats.magic;
        var damage=PlayerManager.instance.playerStats.damage;
        var critChange=PlayerManager.instance.playerStats.critChange;
        var critPower=PlayerManager.instance.playerStats.critPower;
        var critEquipMent=PlayerManager.instance.playerStats.critEquiptment;
        var MaxHealth=PlayerManager.instance.playerStats.maxHealth;
        var armor=PlayerManager.instance.playerStats.armor;
        var evasion=PlayerManager.instance.playerStats.evasion;
        var currentHealth = PlayerManager.instance.playerStats.currentHealth;



        SaveAndLoad.SaveData(xPosition, yPosition, zPosition,
        strength,agility,intellegence,vitality,magic,damage,critChange,critPower,critEquipMent,MaxHealth,armor,evasion,currentHealth);
    }

   public void LoadGame()
    {
        var data = SaveAndLoad.LoadData();
        //玩家位置
        PlayerManager.instance.player.transform.position=new Vector3( data.xPosition, data.yPosition, data.zPosition );
        //玩家数据
        PlayerManager.instance.playerStats.strength=data.strength;
        PlayerManager.instance.playerStats.agility=data.agility;
        PlayerManager.instance.playerStats.intellegence=data.intellegence;
        PlayerManager.instance.playerStats.vitality=data.vitality;
        PlayerManager.instance.playerStats.magic=data.magic;
        PlayerManager.instance.playerStats.damage=data.damage;
        PlayerManager.instance.playerStats.critChange=data.critChange;
        PlayerManager.instance.playerStats.critPower=data.critPower;
        PlayerManager.instance.playerStats.critEquiptment=data.critEquiptment;
        PlayerManager.instance.playerStats.maxHealth=data.maxHealth;
        PlayerManager.instance.playerStats.armor=data.armor;
        PlayerManager.instance.playerStats.evasion=data.evasion;
        PlayerManager.instance.playerStats.currentHealth=data.currentHealth;
    }
}
