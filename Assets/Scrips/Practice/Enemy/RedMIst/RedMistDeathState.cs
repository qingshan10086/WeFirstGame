using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 死亡状态
public class RedMistDeathState : RedMistBaseState
{
    public RedMistDeathState(RedMist enemy, RedMistStateMachine stateMachine) : base(enemy, stateMachine)
    {}
    
    public override void Enter()
    {
        Debug.Log("进入死亡状态");
        // 这里可以添加死亡动画和死亡逻辑
        enemy.rb.velocity = Vector2.zero;
    }
    
    public override void Update()
    {
        // 死亡状态通常不需要更新
    }
}
