using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    // 创建单例模式
    private static EventManager instance;
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
    public UnityEvent OnBossBattleEnd = new UnityEvent();
    public UnityEvent OnBossDefeated = new UnityEvent();

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
}
