using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss战事件订阅者示例
/// 展示如何订阅Boss战开始事件
/// </summary>
public class BossBattleSubscriberExample : MonoBehaviour
{
    [Header("订阅者设置")]
    public bool subscribeOnStart = true;  // 是否在Start时订阅
    public bool unsubscribeOnDestroy = true;  // 是否在销毁时取消订阅

    private void Start()
    {
        if (subscribeOnStart)
        {
            SubscribeToBossBattleEvents();
        }
    }

    private void OnDestroy()
    {
        if (unsubscribeOnDestroy)
        {
            UnsubscribeFromBossBattleEvents();
        }
    }

    // 订阅Boss战事件
    public void SubscribeToBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            // 订阅Boss战开始事件
            EventManager.Instance.OnBossBattleStart.AddListener(OnBossBattleStarted);
            // 订阅Boss战结束事件
            EventManager.Instance.OnBossBattleEnd.AddListener(OnBossBattleEnded);
            
            Debug.Log("Subscribed to Boss battle events");
        }
        else
        {
            Debug.LogError("EventManager instance not found!");
        }
    }

    // 取消订阅Boss战事件
    public void UnsubscribeFromBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            // 取消订阅Boss战开始事件
            EventManager.Instance.OnBossBattleStart.RemoveListener(OnBossBattleStarted);
            // 取消订阅Boss战结束事件
            EventManager.Instance.OnBossBattleEnd.RemoveListener(OnBossBattleEnded);
            
            Debug.Log("Unsubscribed from Boss battle events");
        }
    }

    // Boss战开始时调用的方法
    private void OnBossBattleStarted()
    {
        Debug.Log("=== BOSS BATTLE STARTED ===");
        Debug.Log("This method is called when Boss battle begins.");
        Debug.Log("Add your Boss battle logic here:");
        Debug.Log("- Change music");
        Debug.Log("- Spawn Boss");
        Debug.Log("- Lock camera");
        Debug.Log("- Show UI elements");
        Debug.Log("- etc.");
        Debug.Log("=========================");
        
        // TODO: 在这里添加Boss战开始时需要执行的逻辑
    }

    // Boss战结束时调用的方法
    private void OnBossBattleEnded()
    {
        Debug.Log("=== BOSS BATTLE ENDED ===");
        Debug.Log("This method is called when Boss battle ends.");
        Debug.Log("Add your Boss battle end logic here:");
        Debug.Log("- Restore music");
        Debug.Log("- Unlock camera");
        Debug.Log("- Hide UI elements");
        Debug.Log("- Show victory screen");
        Debug.Log("- etc.");
        Debug.Log("=========================");
        
        // TODO: 在这里添加Boss战结束时需要执行的逻辑
    }
}
