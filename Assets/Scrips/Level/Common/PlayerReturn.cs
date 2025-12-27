using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerReturn : MonoBehaviour
{
    private PlayerStats playerStats;
    private Player player;
    [Header("Range")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
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

        if (transform.position.y < minY || transform.position.x > maxX || transform.position.x < minX || transform.position.y > maxY)
        {
            Debug.Log(transform.position);
            GameOver();
            return;
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

        // 恢复玩家生命值
        PlayerManager.instance.playerStats.currentHealth = data.currentHealth;
        player.stateMachine.ChangeState(player.idleState);
    }
    // 在场景视图中显示范围辅助线（矩形）
    private void OnDrawGizmos()
    {
        // 使用物体当前 z 作为平面 z 值（2D 游戏常用）
        float z = transform.position.z;

        Vector3 bottomLeft = new Vector3(minX, minY, z);
        Vector3 bottomRight = new Vector3(maxX, minY, z);
        Vector3 topLeft = new Vector3(minX, maxY, z);
        Vector3 topRight = new Vector3(maxX, maxY, z);

        // 边框颜色
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // 中心与大小提示
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, z);
        Vector3 size = new Vector3(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY), 0.1f);

        Gizmos.color = new Color(0f, 1f, 1f, 0.6f);
        Gizmos.DrawWireCube(center, size);

        // 当前玩家位置相对于范围的标记（若在场景中有 Player 对象，会更直观）
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}
