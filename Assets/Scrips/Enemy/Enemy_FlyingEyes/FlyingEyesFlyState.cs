using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyesFlyState : EnemyState
{
    private Transform player;//玩家位置
    private Enemy_FlyingEyes enemy;
    private int moveDir;//移动方向
   

    public FlyingEyesFlyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Enemy_FlyingEyes _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player=PlayerManager.instance.player.transform;//获取玩家位置
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        enemy.SetVelocity(enemy.moveSpeed * enemy.faceDirection, enemy.rb.velocity.y);//移动速度设置


        if (!enemy.IsGroundDetected() || enemy.IsWallDetected())//如果检测到前方为地面或者墙壁，翻转
        {
            enemy.Flip();
        }



        if (enemy.IsPlayerDetected() || Vector2.Distance(player.transform.position, enemy.transform.position) < 2)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
