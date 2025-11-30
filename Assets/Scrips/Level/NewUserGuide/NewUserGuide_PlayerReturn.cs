using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReturn_NewGuiderUser : MonoBehaviour
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

        if (transform.position.y < -20 || transform.position.x > 140)
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
        Invoke("ReloadLevel",1.2f);
    }

    private void ReloadLevel()
    {
        playerStats.currentHealth = playerStats.GetMaxHealthValue();
        this.transform.position = new Vector2(-1.4f, -2.8f);
        slider.value = playerStats.currentHealth;//血条当前值
    }

}
