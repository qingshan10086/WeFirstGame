using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameSaveData
{   
    public string saveName = "存档1";//存档名字
    public DateTime saveTime;//存档时间，用于UI

    
    ///玩家数据
    public PlayData PlayData = new PlayData();
    /// <summary>
    /// Key为场景名称，Value为场景保存数据，暂时只有机关
    /// </summary>
    public Dictionary<string, SceneSaveData> sceneStates = new Dictionary<string, SceneSaveData>();
    /// <summary>
    /// 全局机关集合，存储格式：“场景名_机关ID"
    /// </summary>
    public HashSet<string> globalTriggeredMechanisms = new HashSet<string>();

    public int currentLevel = 1;//当前关卡
    public float playTime = 0f;//游戏时间
    
    public GameSaveData(string name = "存档1")
    {
        saveName = name;
        saveTime = DateTime.Now;
    }
    /// <summary>
    /// 场景数据管理
    /// 如果场景数据不存在就创建新的场景数据
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public SceneSaveData GetSceneData(string sceneName)
    {
        if (!sceneStates.ContainsKey(sceneName))
        {
            sceneStates[sceneName]= new SceneSaveData();
        }
        return sceneStates[sceneName];
    }

    /// <summary>
    /// 标记机关为已触发状态
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="mechanismID">机关唯一标识</param>
    /// <param name="isGlobal">是否为全局机关</param>
    public void MarkMechanismTriggered(string sceneName,string mechanismID,bool isGlobal = false)
    {
        if (isGlobal)
        {
            globalTriggeredMechanisms.Add($"{sceneName}_{mechanismID}");
        }
        else
        {
            GetSceneData(sceneName).MarkMechanismTriggered(mechanismID);
        }
    }
    /// <summary>
    /// 检查机关是否已触发
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="mechanismID">机关唯一标识符</param>
    /// <param name="isGlobal">是否为全局机关</param>
    /// <returns></returns>
    public bool IsMechanismTriggered(string sceneName,string mechanismID,bool isGlobal = false)
    {
        if (isGlobal)
        {
            return globalTriggeredMechanisms.Contains($"{sceneName}_{mechanismID}");
        }

        if (sceneStates.ContainsKey(sceneName))
        {
            return sceneStates[sceneName].IsMechanismTriggered(mechanismID);
        }

        return false;//场景不存在默认机关未触发
    }

}


[System.Serializable]
public class PlayData
{
    public float xPosition;
    public float yPosition;
    public float zPosition;
    public int currentHealth;
    public string sceneName = "NewUserGuider";
}

[System.Serializable]
public class SceneSaveData
{
    /// <summary>
    /// 已经触发的机关ID列表
    /// </summary>
    public List<string> triggeredMechanisms=new List<string>();

    /// <summary>
    /// 标记机关已触发
    /// </summary>
    /// <param name="mechanismID">机关唯一标识符</param>
    public void MarkMechanismTriggered(string mechanismID)
    {
        if (!triggeredMechanisms.Contains(mechanismID))
        {
            triggeredMechanisms.Add(mechanismID);
        }
    }
    /// <summary>
    /// 检查机关是否已经触发
    /// </summary>
    /// <param name="mechanismID"></param>
    /// <returns></returns>
    public bool IsMechanismTriggered(string mechanismID)
    {
        return triggeredMechanisms.Contains(mechanismID);
    }
}