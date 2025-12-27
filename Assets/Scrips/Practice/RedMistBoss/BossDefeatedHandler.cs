using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss被击败处理器
/// 订阅Boss被击败事件并执行相应操作
/// 基本框架已搭建，用户可在此基础上编写自己的逻辑
/// </summary>
public class BossDefeated : MonoBehaviour
{
    [Header("事件订阅设置")]
    public bool subscribeOnStart = true;  // 是否在Start时订阅事件
    public bool unsubscribeOnDestroy = true;  // 是否在销毁时取消订阅
    
    private void Start()
    {
        // 订阅事件
        if (subscribeOnStart)
        {
            SubscribeToEvents();
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (unsubscribeOnDestroy)
        {
            UnsubscribeFromEvents();
        }
    }
    private void Update()
    {

    }
    /// <summary>
    /// 订阅Boss战相关事件
    /// </summary>
    private void SubscribeToEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossDefeated.AddListener(OnBossDefeated);
            Debug.Log("BossDefeatedHandler subscribed to OnBossDefeated event");
        }
        else
        {
            Debug.LogError("EventManager instance not found! Cannot subscribe to events.");
        }
    }

    /// <summary>
    /// 取消订阅Boss战相关事件
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossDefeated.RemoveListener(OnBossDefeated);
            Debug.Log("BossDefeatedHandler unsubscribed from OnBossDefeated event");
        }
    }

    /// <summary>
    /// Boss被击败时的处理函数
    /// 用户可以在此处编写自己的逻辑
    /// </summary>
    private void OnBossDefeated()
    {
        Debug.Log("Boss has been defeated! User logic should be implemented here.");
        
        // -----------------------------
        // 用户自定义逻辑区域
        // 请在此处添加Boss被击败后的处理代码
        // -----------------------------
        
        // 示例：调用用户自定义的胜利逻辑
        // UserVictoryLogic();
    }

    // -----------------------------
    // 用户可以在下方添加自己的方法
    // -----------------------------
    
    /*
    /// <summary>
    /// 用户自定义的胜利逻辑
    /// </summary>
    private void UserVictoryLogic()
    {
        // 用户逻辑代码
    }
    */
}