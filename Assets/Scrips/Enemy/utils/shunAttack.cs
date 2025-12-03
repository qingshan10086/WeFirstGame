using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shunAttack : MonoBehaviour
{
    private Animator animator;
    // 1 表示播放攻击1，2 表示播放攻击2
    private int nextAttack = 1;

    // 防止同时启动多个协程
    private bool attackInProgress = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        // 如果收到攻击信号且当前没有在处理攻击，就启动处理协程
        if (animator.GetBool("Attack") && !attackInProgress)
        {
            StartCoroutine(HandleAttack());
        }
    }

    private IEnumerator HandleAttack()
    {
        attackInProgress = true;

        // 顺序触发攻击（设置 AttackID）
        PlayAttack();

        // 等待 Animator / 状态机 将 "Attack" 清除（由 EnemyState.Exit 或动画事件负责）
        yield return new WaitUntil(() => animator == null || !animator.GetBool("Attack"));

        attackInProgress = false;
    }

    void PlayAttack()
    {
        if (animator == null) return;
        if (!animator.GetBool("Attack")) return;

        animator.SetInteger("AttackID", nextAttack);
        // 切换下一次的攻击
        nextAttack = (nextAttack == 1) ? 2 : 1;
    }
}
