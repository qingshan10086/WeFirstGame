using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherMoveState :ArcherGroundedState//骷髅兵移动状态
{
    public ArcherMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        enemy.SetVelocity(enemy.moveSpeed * enemy.faceDirection, enemy.rb.velocity.y);//移动速度设置

        if (!enemy.IsGroundDetected() || enemy.IsWallDetected())//如果检测到前方为地面或者墙壁，翻转，并进入站立状态
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}