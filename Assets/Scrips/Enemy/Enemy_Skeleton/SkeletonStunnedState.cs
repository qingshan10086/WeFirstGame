using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : EnemyState//骷髅兵被弹反状态
{
    private Enemy_Skeleton enemy;

    public SkeletonStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);//重复启动变红协程

        stateTimer = enemy.stunDuration;//初始化被弹反时间

        rb.velocity=new Vector2(-enemy.faceDirection*enemy.stunDirection.x,enemy.stunDirection.y);//被弹反速度
    }

    public override void Exit()
    {
        base.Exit();

        enemy.fx.Invoke("CancelRedBlink", 0);//取消变红协程
    }

    public override void Updata()
    {
        base.Updata();

        if (stateTimer < 0)//被弹反时间到即进入站立状态
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
