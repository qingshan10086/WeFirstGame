using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private CharacterStats ownerStats;
    private Rigidbody2D rb;
    public float lifeTime = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 初始化由生成者调用，设置方向、速度和归属的 stats（用于造成伤害）
    public void Initialize(Vector2 dir, float spd, CharacterStats owner)
    {
        direction = dir.normalized;
        speed = spd;
        ownerStats = owner;

        if (rb != null)
            rb.velocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 碰到地面或墙体就销毁（根据工程标签/图层调整）
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        // 兼容碰撞体在子物体或父物体上的情况
        CharacterStats targetStats = other.GetComponent<CharacterStats>();
        if (targetStats == null)
            targetStats = other.GetComponentInParent<CharacterStats>();
        if (targetStats == null)
            targetStats = other.GetComponentInChildren<CharacterStats>();

        if (targetStats != null)
        {
            // 避免攻击到发射者自己
            if (ownerStats != null && targetStats == ownerStats)
            {
                return;
            }

            Debug.Log($"ArrowProjectile: hit '{targetStats.gameObject.name}'. Owner: {(ownerStats!=null?ownerStats.gameObject.name:"null")}");

            if (ownerStats != null)
            {
                // 调用发射者的伤害逻辑（保留暴击/护甲等逻辑）
                ownerStats.DoDamage(targetStats);
            }

            Debug.Log($"ArrowProjectile: target health after hit = {targetStats.currentHealth}");
            Destroy(gameObject);
        }
    }
}