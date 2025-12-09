using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 移动状态
public class RedMistMoveState : RedMistBaseState
{
    private float moveTimer;
    private float moveDuration;
    
    public RedMistMoveState(RedMist enemy, RedMistStateMachine stateMachine) : base(enemy, stateMachine)
    {}
    
    public override void Enter()
    {
        moveTimer = 0;
        moveDuration = Random.Range(2f, 4f); // 随机移动时间
        Debug.Log("进入移动状态");
    }
    
    public override void Update()
    {
        moveTimer += Time.deltaTime;
        
        // 保持水平移动
        enemy.MaintainHorizontalVelocity();
        
        // 移动时间结束后切换到空闲状态
        if (moveTimer >= moveDuration)
        {
            stateMachine.ChangeState(RedMistState.Idle);
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
