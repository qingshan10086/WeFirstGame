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
            if (SaveManager.Instance == null)
            {
                Debug.LogError("存档管理器未初始化");
                return false;
            }

            if (PlayerManager.instance == null ||
                PlayerManager.instance.player == null ||
                PlayerManager.instance.playerStats == null)
            {
                Debug.LogError("PlayerManager组件不完整");
                return false;
            }

            SaveManager.Instance.UpdateGameScene(SceneManager.GetActiveScene().name);
            SaveManager.Instance.UpdatePlayerPosition(PlayerManager.instance.player.transform.position);
            SaveManager.Instance.UpdatePlayerHealth(PlayerManager.instance.playerStats.currentHealth);

            SaveManager.Instance.SaveGame();
            Debug.Log("游戏已保存，玩家数据和机关状态已保存");
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, goalRaidus);
    }
}
