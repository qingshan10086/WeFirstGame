// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Diagnostics;
// using UnityEngine;

// public class EnemyState2 
// {
//     protected EnemyStateMachine2 stateMachine;
//     protected Enemy_nothing enemy;
//     private string animBoolName;//动画变化标志

//     protected float stateTimer;//计时器
//     public bool triggerCalled;//触发标志


//     public EnemyState2(EnemyStateMachine2 _stateMachine, Enemy_nothing _enemy, string _animBoolName)
//     {
//         stateMachine = _stateMachine;
//         enemy = _enemy;
//         animBoolName = _animBoolName;
//     }

//     public virtual void Enter()
//     {
//         triggerCalled = false;
//         enemy.anim.SetBool(animBoolName, true);
//     }

// public virtual void Update()
//     {
//         stateTimer -= Time.deltaTime;
        
//     }
//     public virtual void Exit()
//     {
//         enemy.anim.SetBool(animBoolName, false);
//     }
   
//    public virtual void triggerEvent()
//     {
//         triggerCalled = true;
//     }
// }


