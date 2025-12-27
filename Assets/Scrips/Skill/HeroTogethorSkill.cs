using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTogethorSkill : MonoBehaviour
{
    [Header("配置")]
    [SerializeField] private GameObject heroPrefab;         // 指向 HeroKnight 的预制体
    [SerializeField] private float transformDuration = 30f; // 变身持续时间（秒）
    [SerializeField] private float cooldown = 60f;          // 技能冷却（可选）
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f); // HeroKnight 相对于 Player 的生成偏移

    private float cooldownTimer = 0f;
    private bool isActive = false;
    private GameObject spawnedHero;
    private Player player;

    private List<MonoBehaviour> disabledPlayerComponents = new List<MonoBehaviour>();
    private Animator playerAnimator;

    private void Awake()
    {
        // 优先通过 PlayerManager 获取 player，否则查找场景中的 Player
        player = (PlayerManager.instance != null && PlayerManager.instance.player != null)
            ? PlayerManager.instance.player
            : FindObjectOfType<Player>();

        playerAnimator = player != null ? player.GetComponent<Animator>() : null;
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (isActive) return;

        if (Input.GetKeyDown(KeyCode.P) && cooldownTimer <= 0f)
        {
            StartCoroutine(TransformRoutine());
            cooldownTimer = cooldown;
        }
    }

    private IEnumerator TransformRoutine()
    {
        if (player == null || heroPrefab == null)
        {
            Debug.LogWarning("HeroKnightTransformSkill: player 或 heroPrefab 未设置");
            yield break;
        }

        isActive = true;

        

        // 实例化 HeroKnight，并把位置、朝向与 Player 对齐
        spawnedHero = Instantiate(heroPrefab, player.transform.position + spawnOffset, player.transform.rotation);
        var playerSr = player.GetComponent<SpriteRenderer>();
        var heroSr = spawnedHero.GetComponent<SpriteRenderer>();
        if (playerSr != null && heroSr != null)
        {
            heroSr.flipX = playerSr.flipX;
        }

        // 等待持续时间
        yield return new WaitForSeconds(transformDuration);

        // 销毁 HeroKnight 实例，恢复 Player 控制
        if (spawnedHero != null) Destroy(spawnedHero);

        

        isActive = false;
    }
}