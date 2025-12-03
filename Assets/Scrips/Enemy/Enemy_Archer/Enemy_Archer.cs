using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Archer : Enemy//敌人中的骷髅兵
{
    #region States  声明各个状态
    public ArcherIdleState idleState {  get; private set; }
    public ArcherMoveState moveState { get; private set; }
    public ArcherBattleState battleState { get; private set; }
    public ArcherAttackState attackState { get; private set; }
    public ArcherStunnedState stunnedState { get; private set; }
    public ArcherDeadState deadState { get; private set; }
    #endregion

    public GameObject arrow;//箭矢预制体

    [Header("Arrow Shoot Info")]
    [SerializeField] private Transform arrowSpawnPoint; // 箭矢生成位置（编辑器赋值）
    [SerializeField] private float arrowSpeed = 8f; // 箭速

    protected override void Awake()
    {
        base.Awake();

        idleState = new ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new ArcherMoveState(this, stateMachine, "Move", this);
        battleState = new ArcherBattleState(this, stateMachine, "Move", this);
        attackState = new ArcherAttackState(this, stateMachine, "Attack", this);
        stunnedState = new ArcherStunnedState(this, stateMachine, "Stunned", this);
        deadState = new ArcherDeadState(this, stateMachine, "Die", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);//初始化状态
    }

    protected override void Update()
    {
        base.Update();

    }

    public override bool CanBeStunned()//被弹反状态优先级较高
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    // 从外部调用：面向玩家生成并发射一支箭
    public void SpawnArrowToPlayer()
    {
        if (arrow == null || arrowSpawnPoint == null) return;

        Transform playerTf = PlayerManager.instance != null ? PlayerManager.instance.player.transform : null;
        Vector2 dir;
        if (playerTf != null)
            dir = (playerTf.position - arrowSpawnPoint.position);
        else
            dir = transform.right * (faceDirection != 0 ? faceDirection : 1);

        SpawnArrowAt(dir);
    }

    // 通用的生成箭的方法
    public void SpawnArrowAt(Vector2 dir)
    {
        if (arrow == null || arrowSpawnPoint == null) return;

        GameObject go = Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
        // 尝试使用 ArrowProjectile 初始化（若箭预制带该脚本）
        var proj = go.GetComponent<ArrowProjectile>();
        if (proj != null)
        {
            proj.Initialize(dir, arrowSpeed, this.stats);
        }
        else
        {
            // 若没有脚本，则尝试直接设置刚体速度
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = dir.normalized * arrowSpeed;
        }

        // 翻转箭的朝向（视觉）
        if (dir.x < 0)
            go.transform.localScale = new Vector3(-Mathf.Abs(go.transform.localScale.x), go.transform.localScale.y, go.transform.localScale.z);
        else
            go.transform.localScale = new Vector3(Mathf.Abs(go.transform.localScale.x), go.transform.localScale.y, go.transform.localScale.z);
    }
}