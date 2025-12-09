using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 攻击状态
public class RedMistAttackState : RedMistBaseState
{
    private float attackTimer;
    private float attackCooldown = 2f; // 攻击冷却时间
    
    public RedMistAttackState(RedMist enemy, RedMistStateMachine stateMachine) : base(enemy, stateMachine)
    {}
    
    public override void Enter()
    {
        attackTimer = 0;
        Debug.Log("进入攻击状态");
        // 这里可以添加攻击动画和攻击逻辑
    }
    
    public override void Update()
    {
        attackTimer += Time.deltaTime;
        
        // 攻击冷却结束后切换回空闲状态
        if (attackTimer >= attackCooldown)
        {
            stateMachine.ChangeState(RedMistState.Idle);
        }
    }
}
