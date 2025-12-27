using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Player player;
    public PlayerStats playerStats;
    
    

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            
        }
        else
        {
            instance = this;
            
        }
    }

    private void Start()
    {
        PlayData data = SaveManager.Instance.GetPlayData();
        if (SceneManager.GetActiveScene().name == data.sceneName)
        {
            // 恢复玩家位置
            Vector3 savedPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
            PlayerManager.instance.player.transform.position = savedPosition;

            // 恢复玩家生命值
            PlayerManager.instance.playerStats.currentHealth = data.currentHealth;
            Debug.Log("使用了二方法");

        }
    }
}
