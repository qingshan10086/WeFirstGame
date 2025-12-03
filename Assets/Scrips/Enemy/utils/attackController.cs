using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackController : MonoBehaviour
{
    private Animator animator;
    // 可配置的攻击动画数量，这里是3个（attackA, B, C）
    [SerializeField] private int numberOfAttacks = 3;

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

        // 触发一次随机攻击（设置 AttackID)
        TriggerRandomAttack();

        // 等待 Animator / 状态机 将 "Attack" 清除（由 EnemyState.Exit 或动画事件负责）
        // 这样不会提前修改 Animator 的参数，避免中断动画事件或状态转换。
        yield return new WaitUntil(() => animator == null || !animator.GetBool("Attack"));

        attackInProgress = false;
    }

    // 这个方法可以在任何需要的时候调用
    public void TriggerRandomAttack()
    {
        // 生成一个1到3之间的随机整数（包括1和3）
        int randomAttackID = Random.Range(1, numberOfAttacks + 1);

        // 将随机生成的ID设置给Animator中的参数
        animator.SetInteger("AttackID", randomAttackID);
    }
}
