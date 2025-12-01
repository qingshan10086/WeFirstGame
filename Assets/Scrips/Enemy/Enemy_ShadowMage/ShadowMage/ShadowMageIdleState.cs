using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShadowMageIdleState : EnemyState
{
    protected Enemy_ShadowMage enemy;//申明影法师
    protected UnityEngine.Transform player;//申明玩家位置信息


    public int faceDir = 1;//初始面朝方向为右
    private bool faceRight = true;//判断是否面朝右边
    private float faceToPlayerCoolDown = 1.5f;//影法师转向的频率
    private float faceToPlayerTimer = 0f;


 


    public ShadowMageIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_ShadowMage _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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

        FaceToPlayer();

        if (enemy.IsPlayerDetected())//如果检测到玩家
        {
            stateTimer = enemy.battleTime;//重置战斗状态持续时间

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)//如果距离足够，则进入攻击状态
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attack2State);
                }
            }

        }



    }

    private void FaceToPlayer()//使影法师始终面对玩家
    {
        if (faceToPlayerTimer > 0)
        {
            faceToPlayerTimer-= Time.deltaTime;
            return;
        }


        if (player.position.x - enemy.transform.position.x < 0 && faceRight)
        {
            faceDir = faceDir * -1;
            faceRight = !faceRight;
            enemy.transform.Rotate(0, 180, 0);
            faceToPlayerTimer = faceToPlayerCoolDown;
        }
        if (player.position.x - enemy.transform.position.x > 0 && !faceRight)
        {
            faceDir = faceDir * -1;
            faceRight = !faceRight;
            enemy.transform.Rotate(0, 180, 0);
            faceToPlayerTimer = faceToPlayerCoolDown;
        }
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
