using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 空闲状态
public class RedMistIdleState : RedMistBaseState
{
    private float idleTimer;
    private float idleDuration;
    
    public RedMistIdleState(RedMist enemy, RedMistStateMachine stateMachine) : base(enemy, stateMachine)
    {}
    
    public override void Enter()
    {
        idleTimer = 0;
        idleDuration = Random.Range(1f, 3f); // 随机空闲时间
        Debug.Log("进入空闲状态");
    }
    
    public override void Update()
    {
        idleTimer += Time.deltaTime;
        
        // 空闲时间结束后切换到移动状态
        if (idleTimer >= idleDuration)
        {
            stateMachine.ChangeState(RedMistState.Move);
        }
        
        // 如果检测到玩家，切换到攻击状态
        if (IsPlayerInAttackRange())
        {
            stateMachine.ChangeState(RedMistState.Attack);
        }
    }
    
    private bool IsPlayerInAttackRange()
    {
        if (enemy.player == null) return false;
        
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.transform.position);
        return distance <= enemy.playerDetectionRange;
    }
}
