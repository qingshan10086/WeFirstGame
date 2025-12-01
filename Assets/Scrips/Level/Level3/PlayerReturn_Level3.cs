using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReturn_Level3 : MonoBehaviour
{
    private PlayerStats playerStats;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
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

        if (transform.position.y < -100 || transform.position.x <10||transform.position.y>72)
        {
            GameOver();
            return;
        }

        if (playerStats.currentHealth <= 0)
        {
            GameOver();
            return;
        }
    }

    private void GameOver()
    {
        //游戏失败UI,当前还没做以后做
        Invoke("ReloadLevel", 1.2f);
    }

    private void ReloadLevel()
    {
        playerStats.currentHealth = playerStats.GetMaxHealthValue();
        this.transform.position = new Vector2(15f, 1.05f);
        slider.value = playerStats.currentHealth;//血条当前值
    }
}
