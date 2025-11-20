using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ShadowMageIdleState : EnemyState
{
    protected Enemy_ShadowMage enemy;//申明影法师
    protected UnityEngine.Transform player;//申明玩家位置信息


    private int faceDir = 1;//初始面朝方向为右
    private bool faceRight = true;//判断是否面朝右边

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

    }

    private void FaceToPlayer()//
    {
        if (player.position.x - enemy.transform.position.x < 0 && faceRight)
        {
            faceDir = faceDir * -1;
            faceRight = !faceRight;
            enemy.transform.Rotate(0, 180, 0);
        }
        if (player.position.x - enemy.transform.position.x > 0 && !faceRight)
        {
            faceDir = faceDir * -1;
            faceRight = !faceRight;
            enemy.transform.Rotate(0, 180, 0);
        }
    }
}
