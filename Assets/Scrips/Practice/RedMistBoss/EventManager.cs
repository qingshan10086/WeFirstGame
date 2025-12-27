using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    // 创建单例模式
    private static EventManager instance;
    public Enemy Boss;
    public Player Player;
    private bool defeatedalreadyTriggered = false;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EventManager");
                    instance = obj.AddComponent<EventManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    // Boss战事件定义
    [Header("Boss战事件")]
    public UnityEvent OnBossBattleStart = new UnityEvent();
    public UnityEvent OnBossBattleSecondPhase = new UnityEvent();
    public UnityEvent OnBossBattleEnd = new UnityEvent();
    public UnityEvent OnBossDefeated = new UnityEvent();
    [Header("50%标志")]
    public bool hasTriggeredSecondPhase=false;
    public int initializedHealth=500;
    private void Awake()
    {
        // 确保单例唯一
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        if (Boss != null && Boss.GetComponent<EnemyStats>() != null)
        {
            initializedHealth = Boss.GetComponent<EnemyStats>().currentHealth;
            Debug.Log($"EventManager: Boss初始血量设置为 {initializedHealth}");
        }
        else
        {
            initializedHealth = 500;
            Debug.LogWarning("EventManager: Boss或EnemyStats组件未找到，使用默认血量500");
        }
    }
    
    public void Update()
    {
        if (Boss != null && Boss.GetComponent<EnemyStats>() != null&&!defeatedalreadyTriggered)
        {
            if (Boss.GetComponent<EnemyStats>().currentHealth <= 0)
            {
                TriggerBossDefeated();
                defeatedalreadyTriggered = true;
            }
            else if (Boss.GetComponent<EnemyStats>().currentHealth <= initializedHealth / 2 && !hasTriggeredSecondPhase)
            {
                TriggerBossBattleSecondPhase();
                hasTriggeredSecondPhase = true;
            }
        }
    }
    // 发布Boss战开始事件
    public void TriggerBossBattleStart()
    {
        OnBossBattleStart.Invoke();
        Debug.Log("Boss battle event triggered!");
    }

    // 发布Boss战结束事件
    public void TriggerBossBattleEnd()
    {
        OnBossBattleEnd.Invoke();
        Debug.Log("Boss battle end event triggered!");
    }

    // 发布Boss被击败事件
    public void TriggerBossDefeated()
    {
        OnBossDefeated.Invoke();
        Debug.Log("Boss defeated event triggered!");
    }
    
    // 发布Boss战第二阶段事件
    public void TriggerBossBattleSecondPhase()
    {
        OnBossBattleSecondPhase.Invoke();
        Debug.Log("Boss battle second phase event triggered!");
    }
    public void activateBoss()
    {
        Boss.enabled = true;
    }
}
