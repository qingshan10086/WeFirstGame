using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class RedMistAppearState : EnemyState
{
    private RedMist enemy;
    private int selectedAttackIndex;
    private RedMistAnimationTrigger animationTrigger;

    public RedMistAppearState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        rb.gravityScale = 0;
        // 设置出现状态的持续时间
        enemy.ZeroVelocity(); // 停止移动

        // 添加调试日志
        Debug.Log("AppearState Enter - stateTimer set to: " + stateTimer);

        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            Debug.Log("AppearState Enter - 血量为0，切换到死亡状态");
            stateMachine.ChangeState(enemy.deathState);
            return;
        }

        // 获取动画触发器组件
        if (animationTrigger == null)
        {
            animationTrigger = enemy.GetComponentInChildren<RedMistAnimationTrigger>();
            Debug.Log("AppearState Enter - animationTrigger: " + (animationTrigger != null ? "Found" : "Not Found"));
        }

        // 在appear开始时随机选择一种攻击状态
        selectedAttackIndex = Random.Range(0, 4);
        Debug.Log("AppearState Enter - selectedAttackIndex: " + selectedAttackIndex);
        //测试出现1
        //enemy.anim.SetInteger("appearType", 0);
        // 测试出现2
        //enemy.anim.SetInteger("appearType", 1);
        // 测试出现3
        //enemy.anim.SetInteger("appearType", 2);
        // 测试出现4
        //enemy.anim.SetInteger("appearType", 3);
        //根据选择的攻击状态调用对应的出现动画事件
        //这里假设不同的攻击状态对应不同的出现位置
        if (animationTrigger != null)
        {
            if (enemy.attack3Counter > 0)
            {
                enemy.anim.SetInteger("appearType", 2);
                Debug.Log("AppearState Enter - appearType set to 3 (Attack3)");
            }
            else if (enemy.dashAttackCounter > 0)
            {
                enemy.anim.SetInteger("appearType", 0);
                Debug.Log("AppearState Enter - appearType set to 2 (AttackDash)");
            }
            else
            {
                if (selectedAttackIndex == 3)
                {
                    enemy.anim.SetInteger("appearType", 0);
                    Debug.Log("AppearState Enter - appearType set to 0 (AttackDash)");
                }
                else if (selectedAttackIndex == 2)
                {
                    enemy.anim.SetInteger("appearType", 2);
                    Debug.Log("AppearState Enter - appearType set to 2 (Attack3)");
                }
                else if (selectedAttackIndex == 1)
                {
                    enemy.anim.SetInteger("appearType", 1);
                    Debug.Log("AppearState Enter - appearType set to 1 (Attack2)");
                }
                else if (selectedAttackIndex == 0)
                {
                    enemy.anim.SetInteger("appearType", 1);
                    Debug.Log("AppearState Enter - appearType set to 1 (Attack1)");
                }
            }
        }
        else
        {
            Debug.LogWarning("AppearState Enter - animationTrigger is null");
        }
    }

    public override void Updata()
    {
        base.Updata();


        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            Debug.Log("AppearState - 血量为0，切换到死亡状态");
            stateMachine.ChangeState(enemy.deathState);
            return;
        }

        // 如果状态计时器结束，根据之前选择的攻击状态进行切换
        if (triggerCalled)
        {
            Debug.Log("准备转入攻击状态");
            //调试attackdash
            //stateMachine.ChangeState(enemy.attackDashState);
            if(enemy.attack3Counter > 0)
            {
                Debug.Log("AppearState - 切换到attack3State");
                stateMachine.ChangeState(enemy.attack3State);
            }
            else if(enemy.dashAttackCounter > 0)
            {
                Debug.Log("AppearState - 切换到attackDashState");
                stateMachine.ChangeState(enemy.attackDashState);
            }

            else{
                switch (selectedAttackIndex)
                {
                    case 0:
                        Debug.Log("AppearState - 切换到attack1State");
                        stateMachine.ChangeState(enemy.attack1State);
                        break;
                    case 1:
                        Debug.Log("AppearState - 切换到attack2State");
                        stateMachine.ChangeState(enemy.attack2State);
                        break;
                    case 2:
                        Debug.Log("AppearState - 切换到attack3State");
                        stateMachine.ChangeState(enemy.attack3State);
                        enemy.attack3Counter = enemy.maxConsecutiveAttacks;
                        break;
                    case 3:
                        Debug.Log("AppearState - 切换到attackDashState");
                        stateMachine.ChangeState(enemy.attackDashState);
                        enemy.dashAttackCounter = enemy.maxConsecutiveDashAttacks;
                        break;
                    default:
                        Debug.LogError("AppearState - 无效的selectedAttackIndex: " + selectedAttackIndex);
                        stateMachine.ChangeState(enemy.idleState);
                        break;
                }
            }
    }
}

    public override void Exit()
    {
        base.Exit();
        // 出现状态结束时的清理
    }
}