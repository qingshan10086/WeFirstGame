using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class TomMaliTrigger : MonoBehaviour
{
    private TomMali enemy;
    private PlayerStats playerStats;
    
    // 技能伤害冷却字典，记录对每个玩家的冷却时间
    private Dictionary<GameObject, float> skill1DamageCooldowns = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> skill3DamageCooldowns = new Dictionary<GameObject, float>();

    private void Awake()
    {
        enemy = GetComponentInParent<TomMali>();
        // 获取PlayerStats组件
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("找到玩家");
            playerStats = player.GetComponent<PlayerStats>();
        }
    }
    
    // 技能1攻击检测 - 黑色圆形
    public void Skill1AttackTrigger()
    {
        if (enemy.skill1Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.skill1Attack.position, enemy.skill1AttackRange, enemy.whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                GameObject player = collider.gameObject;
                // 检查技能1冷却时间
                if (CanApplySkill1Damage(player))
                {
                    UnityEngine.Debug.Log("TomMali技能1攻击到玩家");
                    playerStats = collider.GetComponent<PlayerStats>();
                    // 应用技能1伤害
                    ApplyDamage(enemy.skill1Damage);
                    // 更新技能1冷却时间
                    UpdateSkill1Cooldown(player);
                }
            }
        }
    }
    
    // 技能2攻击检测 - 红色圆形
    public void Skill2AttackTrigger()
    {
        if (enemy.skill2Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.skill2Attack.position, enemy.skill2AttackRange, enemy.whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                UnityEngine.Debug.Log("TomMali技能2攻击到玩家");
                 playerStats = collider.GetComponent<PlayerStats>();
                // 应用技能2伤害
                ApplyDamage(enemy.skill2Damage);
            }
        }
    }
    
    // 技能3攻击检测 - 蓝色圆形
    public void Skill3AttackTrigger()
    {
        if (enemy.skill3Attack == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.skill3Attack.position, enemy.skill3AttackRange, enemy.whatisPlayer);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                GameObject player = collider.gameObject;
                // 检查技能3冷却时间
                if (CanApplySkill3Damage(player))
                {
                    UnityEngine.Debug.Log("TomMali技能3攻击到玩家");
                    playerStats = collider.GetComponent<PlayerStats>();
                    // 应用技能3伤害
                    ApplyDamage(enemy.skill3Damage);
                    // 更新技能3冷却时间
                    UpdateSkill3Cooldown(player);
                }
            }
        }
    }
    
    // 应用伤害的辅助方法
    private void ApplyDamage(float damage)
    {
        if (playerStats != null)
        {
            playerStats.TakeDamage(Mathf.RoundToInt(damage));
            UnityEngine.Debug.Log("TomMali对玩家造成了 " + Mathf.RoundToInt(damage) + " 点伤害");
            
            // 播放攻击光效
            if (enemy.fx != null)
            {
                enemy.fx.StartCoroutine("FlashFX");
            }
        }
        else
        {
            // 如果没有找到PlayerStats组件，尝试动态获取
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(Mathf.RoundToInt(damage));
                    UnityEngine.Debug.Log("TomMali对玩家造成了 " + Mathf.RoundToInt(damage) + " 点伤害");
                    
                    // 播放攻击光效
                    if (enemy.fx != null)
                    {
                        enemy.fx.StartCoroutine("FlashFX");
                    }
                }
            }
        }
    }
    
    // 检查技能1是否可以对玩家造成伤害
    private bool CanApplySkill1Damage(GameObject player)
    {
        float currentTime = Time.time;
        if (skill1DamageCooldowns.ContainsKey(player))
        {
            return currentTime > skill1DamageCooldowns[player];
        }
        return true;
    }
    
    // 检查技能3是否可以对玩家造成伤害
    private bool CanApplySkill3Damage(GameObject player)
    {
        float currentTime = Time.time;
        if (skill3DamageCooldowns.ContainsKey(player))
        {
            return currentTime > skill3DamageCooldowns[player];
        }
        return true;
    }
    
    // 更新技能1的冷却时间
    private void UpdateSkill1Cooldown(GameObject player)
    {
        skill1DamageCooldowns[player] = Time.time + enemy.skillDamageCooldown;
    }
    
    // 更新技能3的冷却时间
    private void UpdateSkill3Cooldown(GameObject player)
    {
        skill3DamageCooldowns[player] = Time.time + enemy.skillDamageCooldown;
    }
}