using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using System.IO;
using System;
using Newtonsoft.Json;

public class SaveAndLoad : MonoBehaviour
{
    


    public static void SaveData(float xPosition, float yPosition, float zPosition,int currentHealth,string sceneName)
    {
        var data = new PlayerData();
        data.xPosition = xPosition;
        data.yPosition = yPosition;
        data.zPosition = zPosition;
        
        data.currentHealth = currentHealth;
        data.sceneName = sceneName;



        //var jsonStr=JsonUtility.ToJson(data);
        var jsonStr = JsonConvert.SerializeObject(data);
        var filePath = Application.persistentDataPath + "/playerData.json";
        File.WriteAllText(filePath, jsonStr);
        Debug.Log(filePath);

    }



    public static PlayerData LoadData()
    {
        var filePath = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(filePath))
        {
            var jsonStr=File.ReadAllText(filePath);
            //var data = JsonUtility.FromJson<PlayerData>(jsonStr);
            var data=JsonConvert.DeserializeObject<PlayerData>(jsonStr);

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


    

    public int currentHealth;//µ±Ç°ÑªÁ¿


    public string sceneName;
}
