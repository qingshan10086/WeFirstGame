using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyesBattleState : EnemyState
{
    private Transform player;
    private Enemy_FlyingEyes enemy;
    private int moveDir;
    private float attack1Distance=1.2f;//攻击1的距离
    //private float attack2Distance;//攻击2的距离

    public FlyingEyesBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_FlyingEyes _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;//获取玩家位置信息
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

       




        if (enemy.IsPlayerDetected())//如果检测到玩家
        {
            stateTimer = enemy.battleTime;//重置战斗状态持续时间

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance&& enemy.IsPlayerDetected().distance > attack1Distance)//如果距离足够，则进入攻击状态
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attack2State);
                }
            }

            if (enemy.IsPlayerDetected().distance < attack1Distance)//如果距离足够，则进入攻击状态
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attack1State);
                }
            }

        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 20)//如果距离过远，进入站立状态
            {
                stateMachine.ChangeState(enemy.flyState);
            }
        }




        if (player.position.x > enemy.transform.position.x)//战斗状态始终面向玩家
        {
            moveDir = 1;
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }

        enemy.SetVelocity(enemy.moveSpeed * moveDir*1.5f, rb.velocity.y);//战斗速度 
    }




    private bool CanAttack()//攻击冷却设置
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }


}

