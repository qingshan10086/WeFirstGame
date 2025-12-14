using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private Transform player;//获取玩家位置
    [SerializeField] private float goalRaidus;//检测玩家是否到达的半径
    private float saveCooldown = 5f;
    private float saveCooldownTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        saveCooldownTimer -= Time.deltaTime;
        if (saveCooldownTimer <= 0f)
        {
            if (CanSavePoint())
            {
                AudioManager.instance.PlaySFX(9, null);//播放回血音效
                saveCooldownTimer = saveCooldown;
            }
            
            
        }
    }

    private bool CanSavePoint()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < goalRaidus)
        {
            if (PlayerManager.instance == null ||
             PlayerManager.instance.player == null ||
             PlayerManager.instance.playerStats == null)
            {
                Debug.LogError("PlayerManager组件不完整");
                return false;
            }

            //玩家位置
            var xPosition = PlayerManager.instance.player.transform.position.x;
            var yPosition = PlayerManager.instance.player.transform.position.y;
            var zPosition = PlayerManager.instance.player.transform.position.z;
            //玩家数据

            var currentHealth = PlayerManager.instance.playerStats.currentHealth;


            var sceneName = SceneManager.GetActiveScene().name;


            SaveAndLoad.SaveData(xPosition, yPosition, zPosition, currentHealth, sceneName);
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, goalRaidus);
    }
}
