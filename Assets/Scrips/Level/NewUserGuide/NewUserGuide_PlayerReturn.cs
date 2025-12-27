using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReturn_NewGuiderUser : MonoBehaviour
{
    private PlayerStats playerStats;
    public Slider slider;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GetComponent<Player>();
        }
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
            if (playerStats == null) return;
        }

        if (transform.position.y < -20 || transform.position.x > 140)
        {
            GameOver();
            return;
        }

        if (playerStats.currentHealth <0)
        {
            GameOver();
            return; 
        }
    }

    private void GameOver()
    {
        //游戏失败UI,当前还没做以后做
        Invoke("ReloadLevel",0.5f);
    }

    private void ReloadLevel()//重新加载场景脚本
    {
        PlayData data = SaveManager.Instance.GetPlayData();

        Vector3 savedPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
        PlayerManager.instance.player.transform.position = savedPosition;

        // 恢复玩家生命值
        PlayerManager.instance.playerStats.currentHealth = data.currentHealth;
        player.stateMachine.ChangeState(player.idleState);
    }

}
