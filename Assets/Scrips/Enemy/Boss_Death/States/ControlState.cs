using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ControlState : EnemyState
{
    private Boss_Death enemy;
    private Transform player;
    private Player playerEntity;

    Vector3 v3;
    public ControlState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Boss_Death _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
        playerEntity = PlayerManager.instance?.player;
        player = playerEntity?.transform;
        v3 = new Vector3(0, 5, 0);
    }

    public override void Enter()
    {
        base.Enter();

        // 设置控制持续时间
        stateTimer = enemy.controlDuration;
        enemy.lastControlTime = Time.time;

        // 让 Boss 停止移动并保持当前动作
        enemy.ZeroVelocity();

        if (player != null && enemy.controlPrefab != null)
            GameObject.Instantiate(enemy.controlPrefab, player.position + v3, Quaternion.identity);

        // 启动协程来禁用并在结束后恢复玩家控制
        if (enemy != null)
            enemy.StartCoroutine(DisablePlayerControl());
    }

    private IEnumerator DisablePlayerControl()
    {
        if (playerEntity == null)
            yield break;

        // 只针对 Player 脚本进行禁用/恢复。如果需要禁用其它组件，可在此扩展。
        Player playerScript = playerEntity.GetComponent<Player>();
        if (playerScript != null)
        {
            bool wasEnabled = playerScript.enabled;
            playerScript.enabled = false;

            yield return new WaitForSeconds(enemy.controlDuration);

            playerScript.enabled = wasEnabled;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Updata()
    {
        base.Updata();

        // Boss 在控制期间保持不动
        enemy.ZeroVelocity();

        if (stateTimer < 0f)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
