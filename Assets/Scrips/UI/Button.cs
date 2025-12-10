using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Button : MonoBehaviour
{
    
   public void SaveGame()
    {
        if (PlayerManager.instance == null ||
            PlayerManager.instance.player == null ||
            PlayerManager.instance.playerStats == null)
        {
            Debug.LogError("PlayerManager组件不完整");
            return;
        }
      
        //玩家位置
        var xPosition = PlayerManager.instance.player.transform.position.x;
        var yPosition = PlayerManager.instance.player.transform.position.y;
        var zPosition = PlayerManager.instance.player.transform.position.z;
        //玩家数据
        
        var currentHealth = PlayerManager.instance.playerStats.currentHealth;


        var sceneName = SceneManager.GetActiveScene().name;

      
        SaveAndLoad.SaveData(xPosition, yPosition, zPosition,currentHealth,sceneName);
    }

   public void LoadGame()
    {
        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        var data = SaveAndLoad.LoadData();
    
        if (data == null)
        {
            Debug.LogError("加载存档失败");
            yield break;
        }


        if (SceneManager.GetActiveScene().name != data.sceneName)//如果场景名字不一样，就切换场景
        {
            SceneManager.LoadScene(data.sceneName);
            yield return null;

            while (SceneManager.GetActiveScene().name != data.sceneName)
            {
                yield return null;
            }

            yield return null;
        }


        yield return WaitForPlayerInstance();

        ApplyLoadData(data);

    }

    private IEnumerator WaitForPlayerInstance()
    {
        float timeout = 3f;//3秒超时
        float timer = 0f;

        while (PlayerManager.instance == null)
        {
            timer += Time.deltaTime;
            if (timer > timeout)
            {
                Debug.LogError("等待PlayerManager超时");
                yield break;
            }
            yield return null;
        }

       
    }

    private  void ApplyLoadData(PlayerData data)
    {
        if (PlayerManager.instance == null ||
            PlayerManager.instance.player == null ||
            PlayerManager.instance.playerStats == null)
        {
            Debug.LogError("PlayerManager组件不完整");
            return;
        }
      
        
        //玩家位置
        PlayerManager.instance.player.transform.position = new Vector3(data.xPosition, data.yPosition, data.zPosition);

        //玩家数据
       
        PlayerManager.instance.playerStats.currentHealth = data.currentHealth;
      
    }
}
