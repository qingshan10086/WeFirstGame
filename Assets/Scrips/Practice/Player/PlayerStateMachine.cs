using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine    //玩家状态机
{
    public PlayerState currentState {  get; private set; }

    public void Initialize(PlayerState _startState)    //初始化状态
    {
         currentState = _startState;
         currentState.Enter();
    }


    public void ChangeState(PlayerState _newState)   //改变状态函数
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }



}
