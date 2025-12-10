using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastState : EnemyState
{
    private Boss_Death enemy;
    private Transform player;

    public CastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance?.player?.transform;

        // 立即生成预制体在 player 之上
        if (enemy.castPrefab != null && player != null)
        {
            Vector3 spawnPos = player.position + Vector3.up * enemy.castSpawnHeight;
            GameObject.Instantiate(enemy.castPrefab, spawnPos, Quaternion.identity);
        }

        // 记录施法时间（冷却基于这个）
        enemy.lastCastTime = Time.time;

        stateTimer = enemy.castDuration;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        enemy.ZeroVelocity();

        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
