using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{
    [Header("触发设置")]
    public Player player;                     // 玩家对象
    public BoxCollider2D triggerCollider;     // 触发器碰撞体
    public bool isTriggered = false;          // 是否已触发
    public bool isOneTimeUse = true;          // 是否一次性使用

    [Header("事件设置")]
    public bool triggerOnEnter = true;        // 进入时触发
    public bool triggerOnExit = false;        // 退出时触发

    private void Start()
    {
        // 如果没有指定碰撞体，尝试获取自身的BoxCollider2D
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<BoxCollider2D>();
            if (triggerCollider != null)
            {
                // 设置为触发器
                triggerCollider.isTrigger = true;
            }
            else
            {
                Debug.LogError("No BoxCollider2D found! Please assign one.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查是否是玩家，并且还没有触发过
        if (collision.gameObject == player.gameObject && !isTriggered && triggerOnEnter)
        {
            TriggerBossBattle();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 检查是否是玩家，并且还没有触发过
        if (collision.gameObject == player.gameObject && !isTriggered && triggerOnExit)
        {
            TriggerBossBattle();
        }
    }

    // 触发Boss战
    private void TriggerBossBattle()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerBossBattleStart();
            isTriggered = true;
            
            // 如果是一次性使用，禁用触发器
            if (isOneTimeUse)
            {
                if (triggerCollider != null)
                {
                    triggerCollider.enabled = false;
                }
                Debug.Log("Boss battle trigger disabled (one-time use)");
            }
        }
        else
        {
            Debug.LogError("EventManager instance not found!");
        }
    }

    // 重置触发器
    public void ResetTrigger()
    {
        isTriggered = false;
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
        }
        Debug.Log("Boss battle trigger reset");
    }

    // 在编辑器中可视化触发器范围
    private void OnDrawGizmosSelected()
    {
        if (triggerCollider != null)
        {
            // 设置Gizmo颜色
            Gizmos.color = isTriggered ? new Color(1, 0, 0, 0.3f) : new Color(0, 1, 1, 0.3f);
            
            // 获取碰撞体的位置和大小
            Vector3 center = triggerCollider.bounds.center;
            Vector3 size = triggerCollider.bounds.size;
            
            // 绘制触发器范围
            Gizmos.DrawCube(center, size);
            
            // 绘制边框
            Gizmos.color = isTriggered ? Color.red : Color.cyan;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
