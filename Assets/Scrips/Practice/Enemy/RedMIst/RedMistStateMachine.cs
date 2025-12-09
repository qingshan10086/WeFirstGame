using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 状态机类
public class RedMistStateMachine
{
    private RedMist enemy;
    private Dictionary<RedMistState, RedMistBaseState> states;
    private RedMistBaseState currentState;
    
    public RedMistStateMachine(RedMist enemy)
    {
        this.enemy = enemy;
        states = new Dictionary<RedMistState, RedMistBaseState>();
        
        // 初始化所有状态
        states.Add(RedMistState.Idle, new RedMistIdleState(enemy, this));
        states.Add(RedMistState.Move, new RedMistMoveState(enemy, this));
        states.Add(RedMistState.Attack, new RedMistAttackState(enemy, this));
        states.Add(RedMistState.Death, new RedMistDeathState(enemy, this));
    }
    
    // 开始状态机
    public void Start()
    {
        // 默认从空闲状态开始
        currentState = states[RedMistState.Idle];
        currentState.Enter();
    }
    
    // 更新当前状态
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
    
    // 切换状态
    public void ChangeState(RedMistState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        
        currentState = states[newState];
        currentState.Enter();
    }
    
    // 获取当前状态
    public RedMistState GetCurrentState()
    {
        var stateEntry = states.FirstOrDefault(x => x.Value == currentState);
        return stateEntry.Key;
    }
}
