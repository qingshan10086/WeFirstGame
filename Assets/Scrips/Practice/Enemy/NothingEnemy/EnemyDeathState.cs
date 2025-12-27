using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private Enemy_nothing enemy;

    public EnemyDeathState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (Enemy_nothing)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.ZeroVelocity();
        enemy.rb.bodyType = RigidbodyType2D.Static; // 死亡后禁用物理
        enemy.cd.enabled = false; // 死亡后禁用碰撞器
        
        // 禁用所有子物体的碰撞器（如果有）
        Collider2D[] childColliders = enemy.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in childColliders)
        {
            collider.enabled = false;
        }
        
        enemy.anim.SetBool("death", true);
        
        // 延迟销毁物体，确保死亡动画播放完成
        enemy.StartCoroutine(DestroyAfterDelay(2f));
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Object.Destroy(enemy.gameObject);
    }

    public override void Updata()
    {
        base.Updata();
        // 死亡状态不需要更新逻辑，保持直到销毁
    }

    public override void Exit()
    {
        base.Exit();
        // 死亡状态不应该退出，所以可以留空
    }
}
