


using UnityEngine;

/// <summary>
/// 第三种攻击状态
/// </summary>
public class AttackThreeState : EnemyState
{
    private Enemy_nothing enemy;

    public AttackThreeState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (Enemy_nothing)_enemy;
    }

    public override void Enter()
    {
        enemy.anim.SetInteger("combo", 2);
        base.Enter();
        Debug.Log("进入第三种攻击状态");
        enemy.ZeroVelocity(); // 停止移动
        // 设置攻击动画参数
        stateTimer = enemy.attackCooldown * 1.5f; // 设置攻击冷却时间（最长）
        
        // 在玩家脚下召唤spike预制体
        if (enemy.player != null && enemy.spikePrefab != null)
        {
            // 计算spike生成位置：玩家位置的正下方，添加一点偏移以确保在地面
            Vector3 spawnPosition = enemy.player.transform.position;
            spawnPosition.y -= enemy.spikeSpawnOffset;
            
            // 实例化spike预制体
            Object.Instantiate(enemy.spikePrefab, spawnPosition, Quaternion.identity);
            Debug.Log("在玩家脚下生成了spike");
        }
    }

    public override void Updata()
    {
        base.Updata();
        // 攻击动画播放完毕后回到空闲状态
        if (enemy.anim.GetBool("attack")==false)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("退出第三种攻击状态");
    }
}
