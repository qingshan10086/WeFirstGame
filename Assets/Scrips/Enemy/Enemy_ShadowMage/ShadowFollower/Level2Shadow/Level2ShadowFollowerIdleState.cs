using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Level2ShadowFollowerIdleState : EnemyState
{

    private Level2_ShadowFollower enemy;
    

    private float distanceFromPlayerToEnemy;//从玩家到敌人的距离

    private float moveSpeed=5f;//鬼影移动速度

    public int faceDirection { get; private set; } = 1;       //面对方向，初始默认向右
    private bool faceRight = true;                          //判断是否面朝右边

    public Level2ShadowFollowerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName,Level2_ShadowFollower enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
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

        MoveToPlayer();

        FlipControl();

        Debug.Log("鬼影离玩家的距离为："+distanceFromPlayerToEnemy);
    }

    private void FlipControl()//翻转管理器，保证鬼影面朝玩家
    {
        if (faceRight)
        {
            if (PlayerManager.instance.player.transform.position.x - enemy.transform.position.x < 0)
            {
                Flip();
            }
        }
        else if (!faceRight)
        {
            if (PlayerManager.instance.player.transform.position.x - enemy.transform.position.x > 0)
            {
                Flip();
            }
        }
    }

    private void Flip()//翻转函数，保证鬼影面朝玩家
    {
        faceDirection = faceDirection * -1;
        faceRight = !faceRight;
        enemy.transform.Rotate(0, 180, 0);
    }

    private void MoveToPlayer()//鬼影向玩家移动的函数
    {
        distanceFromPlayerToEnemy = Vector2.Distance(enemy.transform.position, PlayerManager.instance.player.transform.position);

        Vector3 direction = (PlayerManager.instance.player.transform.position - enemy.transform.position).normalized;
        enemy.transform.position += direction * moveSpeed * Time.deltaTime;

        if (distanceFromPlayerToEnemy > 40f)
        {
            enemy.transform.position += direction * moveSpeed * 5 * Time.deltaTime;
        }
    }

}
