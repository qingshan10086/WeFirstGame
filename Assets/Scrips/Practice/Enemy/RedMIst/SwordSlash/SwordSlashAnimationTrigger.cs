using System.Collections;
using UnityEngine;

public class SwordSlashAnimationTrigger : MonoBehaviour
{
    [Header("伤害设置")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private int damage = 30;
    [SerializeField] private Vector2 damageAreaSize = new Vector2(2f, 1f);
    [SerializeField] private Transform damageAreaTransform;
    [SerializeField] private AudioClip slashSound; // 普通剑气音效
    
    // 检测触发对玩家造成伤害的函数
    public void TriggerDamageToPlayer()
    {
        // 获取伤害检测的位置
        Vector3 damagePosition = damageAreaTransform != null ? damageAreaTransform.position : transform.position;
        Debug.Log("动画函数触发");
        // 检测触发范围内的玩家
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(damagePosition, damageAreaSize, 0f, whatIsPlayer);
        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log("检测到玩家");
            // 对玩家造成伤害
            if (collider.TryGetComponent<PlayerStats>(out PlayerStats playerStats))
            {
                Debug.Log("特殊剑气对玩家造成伤害");
                playerStats.TakeDamage(damage);
            }
        }
    }

    // 物体动画结束后销毁它的父物体的函数
    public void DestroyObjectAfterAnimation()
    {
        // 销毁当前挂载物体的父物体
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

    // 可视化伤害区域（仅在编辑器中可见）
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // 半透明红色
        // 获取伤害检测的位置
        Vector3 damagePosition = damageAreaTransform != null ? damageAreaTransform.position : transform.position;
        // 绘制实心矩形伤害区域
        Gizmos.DrawCube(damagePosition, damageAreaSize);
        
        // 绘制线框矩形伤害区域
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(damagePosition, damageAreaSize);
    }
    void PlaySoundEffect(AudioClip soundClip)
    {
        if (soundClip != null)
        {
            BloodMusicManager.Instance.PlaySoundEffect(soundClip);
        }
    }
}
