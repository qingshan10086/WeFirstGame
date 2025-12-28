using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BloodFields_PlayerReturn : MonoBehaviour
{
    private PlayerStats playerStats;
    private Player player;
    //public EnemyStats RedMistStats;
    //public Enemy RedMist;
    //public GameObject TomMaliHealthBar;
    //public GameObject RedMistHealthBar;
    //public MonsterDeathDetector MonsterDeathDetector;
    //public CinemachineVirtualCamera normalVirtualCamera;  // 普通虚拟相机
    //public CinemachineVirtualCamera bossBattleVirtualCamera;  // Boss战虚拟相机
    //public Transform BossPosition;//Boss初始位置    
    //public Transform BossCurrentPosition;
    //public Transform doorPosition1;//门初始位置
    //public Transform doorCurrentPosition1;
    //public Transform doorPosition2;
    //public Transform doorCurrentPosition2;
    //// Start is called before the first frame update
    //public GameObject CM;//摄像机


    //[Header("Door")]
    //public doormoveup doorMoveUp1;
    //public doormoveup doorMoveUp2;
    //[Header("UI Elements")]
    //public GameObject bossBattleUIPanel;
    
    //[Header("Second Phase UI")]
    //public GameObject phase2Canvas1; // 第二阶段第一张画布
    //public GameObject phase2Canvas2; // 第二阶段第二张画布

    //[Header("Settings")]
    //public string bossName = "Red Mist Boss";
    //public float fadeDuration = 1f;
    //public float phaseTransitionDuration = 1.0f; // 两个阶段之间的过渡时间

    //private CanvasGroup canvasGroup;
    //private CanvasGroup phase2Canvas1Group;
    //private CanvasGroup phase2Canvas2Group;
    //[Header("EyeDescription")]
    //public SpiritFaderBoss[] spiritFaderBoss;
    //public CanvasFaderBoss canvasFaderBoss;
    //[Header("触发器设置")]
    //public BoxCollider2D BossTrigger;
    //public BossBattleTrigger BossBattleTrigger2;
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
        //BossPosition = BossCurrentPosition;
        //doorPosition1 = doorCurrentPosition1;
        //doorPosition2 = doorCurrentPosition2;
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
        //PlayData data = SaveManager.Instance.GetPlayData();
        SceneManager.LoadScene("BloodFields");
        Debug.Log("重新加载场景");
    //    Vector3 savedPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
    //    PlayerManager.instance.player.transform.position = savedPosition;

     
    //    // 恢复玩家生命值
    //    PlayerManager.instance.playerStats.currentHealth = data.currentHealth;
    //    if (normalVirtualCamera != null)
    //    {
    //        // 先禁用所有相机
    //        if (bossBattleVirtualCamera != null && bossBattleVirtualCamera != normalVirtualCamera)
    //        {
    //            bossBattleVirtualCamera.gameObject.SetActive(false);
    //        }

    //        // 恢复普通相机的跟随功能
    //        CinemachineTransposer transposer = normalVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    //        if (transposer != null)
    //        {
    //            // 恢复默认阻尼值（可以根据项目需要调整）
    //            transposer.m_XDamping = 1f;
    //            transposer.m_YDamping = 1f;
    //            transposer.m_ZDamping = 1f;
    //        }

    //        // 再启用普通相机
    //        normalVirtualCamera.gameObject.SetActive(true);


    //        Debug.Log("Cinemachine normal camera activated");
    //    }
    //    else
    //    {
    //        Debug.LogError("NormalVirtualCamera not assigned!");
    //    }
    //    CM.transform.position = savedPosition;

    //    player.stateMachine.ChangeState(player.idleState);
    //    TomMaliHealthBar.SetActive(false);
    //    RedMistHealthBar.SetActive(false);
    //    RedMistStats.currentHealth=RedMistStats.GetMaxHealthValue();
    //    foreach(var monster in MonsterDeathDetector.detectedMonsters)
    //        {
    //        Destroy(monster);
    //    } 
       
       
    //    // 首先淡出Boss战主UI面板
    //    if (bossBattleUIPanel != null)
    //    {
           
    //        bossBattleUIPanel.SetActive(false);
    //    }
       
    //    // 淡出第二阶段画布
    //    if (phase2Canvas1 != null && phase2Canvas1.activeSelf)
    //    {
           
    //        phase2Canvas1.SetActive(false);
    //    }

    //    if (phase2Canvas2 != null && phase2Canvas2.activeSelf)
    //    {
           
    //        phase2Canvas2.SetActive(false);
    //    }
    //    foreach(var spirit in spiritFaderBoss)
    //    {
    //        SpriteRenderer sr = spirit.GetComponentInChildren<SpriteRenderer>();
    //        Color color= sr.color;
    //        color.a = 0;
    //        sr.color = color;
    //    }
    //    RedMist.enabled = false;
    //    RedMist.transform.position = BossPosition.position;
    //    BossBattleTrigger2.isTriggered = false;
    //    if (BossTrigger != null)
    //    {
    //        BossTrigger.enabled = true;
    //    }
    //    Debug.Log("Boss battle trigger reset");
    //    doorCurrentPosition1.position = BossPosition.position;
    //    doorCurrentPosition2.position = BossPosition.position;
    }


    
}

