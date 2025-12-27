using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossReturn : MonoBehaviour
{
    private PlayerStats playerStats;
    private Player player;

    private string SceneName;
    void Start()
    {
        SceneName = SceneManager.GetActiveScene().name;
        if (player == null)
        {
            player = GetComponent<Player>();
        }
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
    }

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

        if (playerStats.currentHealth < 0)
        {
            GameOver();
            return;
        }
    }

    private void GameOver()
    {
        //游戏失败UI,当前还没做以后做
        Invoke("ReloadLevel", 0.5f);
    }

    private void ReloadLevel()
    {
        PlayData data = SaveManager.Instance.GetPlayData();
        if (SceneName == data.sceneName)
        {
            Vector3 savedPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
            PlayerManager.instance.player.transform.position = savedPosition;
        }
        else
        {
            SceneManager.LoadScene(SceneName);
            Vector3 savedPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
            PlayerManager.instance.player.transform.position = savedPosition;
        }

        // 恢复玩家生命值
        PlayerManager.instance.playerStats.currentHealth = data.currentHealth;
        player.stateMachine.ChangeState(player.idleState);
    }
}
