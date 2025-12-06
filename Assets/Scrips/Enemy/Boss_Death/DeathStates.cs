using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : EnemyState
{
    protected Boss_Death enemy;
    protected Transform player;//申明玩家位置信息
    public GroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
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
        if (enemy.IsPlayerDetected() || Vector2.Distance(player.transform.position, enemy.transform.position) < 2)//如果敌人检测到玩家，或者玩家靠近敌人，进入战斗状态
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
public class IdleState : GroundedState
{
    public IdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName,_enemy)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;//初始化战力时间
    }
    public override void Updata()
    {
        base.Updata();

        if (stateTimer < 0)//战立时间到即进入移动状态
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
}
public class MoveState : GroundedState
{
    public MoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName,_enemy)
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

public class BattleState : GroundedState
{
    private Boss_Death enemy;
    private Transform player;
    private int moveDir;//申明移动方向

    public BattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName,_enemy)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;//获取玩家位置信息
    }
    public override void Updata()
    { 
        base.Updata();
        if (enemy.IsPlayerDetected())//如果检测到玩家
        {
            stateTimer = enemy.battleTime;//重置战斗状态持续时间
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)//如果距离足够，则进入攻击状态
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }
        }
        else
        {
            if (stateTimer < 0)//战斗状态持续时间到即进入站立状态
            {
                stateMachine.ChangeState(enemy.idleState);
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
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
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

public class AttackState : EnemyState
{
    private Boss_Death enemy;
    public AttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
    }
    public override void Updata()
    {
        base.Updata();

        enemy.ZeroVelocity();//攻击时速度为零
        if (triggerCalled)//一完成攻击动画就进入战斗状态
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }

}

public class StunnedState : EnemyState
{
    private Boss_Death enemy;

    public StunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);//重复启动变红协程

        stateTimer = enemy.stunDuration;//初始化被弹反时间

        rb.velocity = new Vector2(-enemy.faceDirection * enemy.stunDirection.x, enemy.stunDirection.y);//被弹反速度
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

public class DeadState : EnemyState
{
    private Boss_Death enemy;
    public DeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Updata()
    {
        base.Updata();

        enemy.ZeroVelocity();//死亡时速度为零
    }
    
}
