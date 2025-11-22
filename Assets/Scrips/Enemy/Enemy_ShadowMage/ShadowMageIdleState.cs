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
    private float faceToPlayerCoolDown = 2.5f;//影法师转向的频率
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
}
