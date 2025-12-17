using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAnimTrigger : MonoBehaviour
{
    private Enemy_nothing enemy;
    public int spikeDamage;
    private void Awake()
    {
        spikeDamage = 10;
    }
    
    // 动画事件函数 - 触发器触发检测伤害
    public void SpikeDamageTrigger()
    {
        // 获取当前物体的碰撞体位置和尺寸
        Collider2D spikeCollider = GetComponentInParent<Collider2D>();
        if (spikeCollider == null)
        {
            Debug.LogWarning("SpikeAnimTrigger: 未找到碰撞体组件");
            return;
        }
        
        // 使用碰撞体的边界框进行伤害检测
        Bounds bounds = spikeCollider.bounds;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // 计算伤害值（可以从敌人获取或使用固定值）
                int damage = enemy != null ? Mathf.RoundToInt(enemy.attackDamage) : spikeDamage;
                
                // 获取玩家的PlayerStats组件
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    // 使用PlayerStats组件造成伤害
                    playerStats.TakeDamage(damage);
                    
                    // 播放攻击光效
                    if (enemy != null && enemy.fx != null)
                    {
                        enemy.fx.StartCoroutine("FlashFX");
                    }
                    
                    Debug.Log("Spike攻击到玩家，造成了 " + damage + " 点伤害");
                }
            }
        }
    }
    
    // 动画事件函数 - 销毁父物体
    public void DestroyParentObject()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            // 如果没有父物体，销毁自身
            Destroy(gameObject);
        }
    }
}