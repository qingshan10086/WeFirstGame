using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;



public class Abyss4Zhuan : MonoBehaviour
{
    public Abyss4 Abyss4;
    public GameObject Manager;
    [Header("Common")]
    [SerializeField] private GameObject player;            // 玩家引用（用于区域触发）
    [SerializeField] private GameObject Text;  // 对话框容器（整体开/关）
    [SerializeField] private GameObject[] texts;          // 每句字幕的 GameObject 数组（按索引显示）
    private int currentText ;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    

    [Header("Trigger")]
    [SerializeField] private DialogueTriggerType triggerType = DialogueTriggerType.Area;
    [Tooltip("区域触发时有效")]
    [SerializeField] private float regionMinX;
    [SerializeField] private float regionMaxX;
    [Tooltip("敌人死亡触发时有效")]
    [SerializeField] private Enemy[] enemyToWatch;
    [Tooltip("Transform有效")]
    [SerializeField] private Transform trigger;
    [SerializeField] private float triggerRadius= 3f;

    [Header("可选")]
    [SerializeField] private bool canCooldown;
    [Tooltip("能cooldown时有效")]
    [SerializeField] private float textCooldown;

    private bool canTrigger;
    private bool hasTrigger;
    private float cooldownTimer;

    private void Start()
    {
        Abyss4 = Manager.GetComponent<Abyss4>();
        currentText = 0;
        hasTrigger = false;
        canTrigger = false;
    }

    private void OnEnable()
    {
        currentText = 0;
        hasTrigger = false;
        canTrigger = true;
    }

    private void Update()
    {
        if (!Abyss4.cantriggerNext) return;
        
        if (canCooldown)
        {
            if (!canTrigger)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer < 0)
                {
                    canTrigger = true;
                    cooldownTimer = textCooldown;
                }
            }
        }
        if (!hasTrigger)
        {
            if (triggerType == DialogueTriggerType.Area)
            {
                if (player.transform.position.x > regionMinX && player.transform.position.x < regionMaxX)
                {
                    canTrigger = true;
                }
            }
            if (triggerType == DialogueTriggerType.EnemyDead)
            {
                bool allDie = true;
                foreach(Enemy e in enemyToWatch)
                {
                    if (e.stats.currentHealth > 0)
                    {
                        allDie = false;
                    }
                }
                canTrigger = allDie;
            }
            if(triggerType  == DialogueTriggerType.Transform)
            {
                float dis = Vector3.Distance(trigger.position, player.transform.position);
                if (dis <= triggerRadius)
                {
                    canTrigger = true;
                }
            }
        }
        if (canTrigger)
        {
            
                if (currentText < texts.Length)
                {
                    Debug.Log(texts[currentText]);
                    Text.SetActive(true);
                    texts[currentText].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        texts[currentText].SetActive(false);
                        currentText++;
                        if (currentText == texts.Length)
                        {
                            Text.SetActive(false);
                            canTrigger = false;
                            hasTrigger = true;
                        }
                    }
                }
            
        }
    }
}