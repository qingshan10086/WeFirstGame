using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击状态机类，用于管理敌人的三种攻击状态
/// </summary>
public class AttackStateMachine
{
    // 当前状态
    public EnemyState2 currentState;
    // 所属的敌人
    public Enemy_nothing enemy;
    // 三种攻击状态引用
    public AttackOneState attackOneState;
    public AttackTwoState attackTwoState;
    public AttackThreeState attackThreeState;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_enemy">所属的敌人</param>
    public AttackStateMachine(Enemy_nothing _enemy)
    {
        enemy = _enemy;
        // 初始化三种攻击状态
        attackOneState = new AttackOneState(null, enemy, "attack");
        attackTwoState = new AttackTwoState(null, enemy, "attack");
        attackThreeState = new AttackThreeState(null, enemy, "attack");
    }
    
    /// <summary>
    /// 初始化攻击状态机
    /// </summary>
    /// <param name="_startingState">初始攻击状态</param>
    public void Initialize(EnemyState2 _startingState)
    {
        currentState = _startingState;
        currentState.Enter();
    }
    
    /// <summary>
    /// 改变攻击状态
    /// </summary>
    /// <param name="_newState">新的攻击状态</param>
    public void ChangeState(EnemyState2 _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
    
    /// <summary>
    /// 更新攻击状态机
    /// </summary>
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
    
    /// <summary>
    /// 随机选择一种攻击状态
    /// </summary>
    public EnemyState2 GetRandomAttackState()
    {
        // 根据敌人的攻击概率设置来选择攻击状态
        float random = Random.value;
        
        // 使用敌人的attackRate来分配概率
        // attackRate是技能1的概率，剩下的概率平均分配给技能2和技能3
        if (random < enemy.attackRate)
        {
            return attackOneState;
        }
        else if (random < enemy.attackRate + (1 - enemy.attackRate) / 2)
        {
            return attackTwoState;
        }
        else
        {
            return attackThreeState;
        }
    }
}