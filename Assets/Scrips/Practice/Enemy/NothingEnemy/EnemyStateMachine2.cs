// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemyStateMachine2 
// {
//     public EnemyState2 currentState { get; private set; }

//     public void Initialize(EnemyState2 _state)
//     {
//         currentState = _state;
//         currentState.Enter();
//     }

//     public void ChangeState(EnemyState2 _state)
//     {
//         Debug.Log($"Changing state from {currentState.GetType().Name} to {_state.GetType().Name}");
//         currentState.Exit();
//         currentState = _state;
//         currentState.Enter();
//     }
// }
