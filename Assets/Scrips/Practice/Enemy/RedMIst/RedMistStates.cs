using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 状态枚举
public enum RedMistState
{
    Idle,
    Move,
    Attack,
    Death
}

// 状态基类
public abstract class RedMistBaseState
{
    protected RedMist enemy;
    protected RedMistStateMachine stateMachine;
    
    public RedMistBaseState(RedMist enemy, RedMistStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }
    
    // 进入状态时调用
    public virtual void Enter() {}
    
    // 更新状态时调用
    public virtual void Update() {}
    
    // 退出状态时调用
    public virtual void Exit() {}
}
