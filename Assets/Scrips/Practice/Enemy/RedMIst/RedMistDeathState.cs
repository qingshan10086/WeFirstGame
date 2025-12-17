using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMistDeathState : EnemyState
{
    private RedMist enemy;
    private Collider2D[] colliders;

    public RedMistDeathState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        
        // 停止所有移动
        enemy.ZeroVelocity();
        
        // 禁用所有碰撞器，防止与玩家或环境交互
        colliders = enemy.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        
        // 禁用刚体物理
        enemy.rb.bodyType = RigidbodyType2D.Static;
        
        // 禁用状态机更新，防止进入其他状态
        enemy.enabled = false;
        
        // 播放死亡音效（如果有）
        // if (enemy.deathSound != null)
        // {
        //     AudioSource.PlayClipAtPoint(enemy.deathSound, enemy.transform.position);
        // }
        
        // 5秒后销毁游戏对象
        Object.Destroy(enemy.gameObject, 5f);
    }

    public override void Updata()
    {
        base.Updata();
        // 死亡状态下不需要更新
    }

    public override void Exit()
    {
        base.Exit();
    }
}
