using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dust : Enemy
{
    #region States  声明各个状态
    public DustIdleState idleState { get; private set; }
    public DustWalkState groudedState { get; private set; }
    public DustBattleState battleState { get; private set; }
    public DustAttackState attackState { get; private set; }
    public DustDeadState deadState { get; private set; }
    public DustHitState hitState { get; private set; }

    #endregion

    public GameObject healthBar; //血条对象

    protected override void Awake()
    {
        base.Awake();
        idleState = new DustIdleState(this, stateMachine, "Idle", this);
        groudedState = new DustWalkState(this, stateMachine, "Move", this);
        battleState = new DustBattleState(this, stateMachine, "Move", this);
        attackState = new DustAttackState(this, stateMachine, "Attack", this);
        deadState = new DustDeadState(this, stateMachine, "Die", this);
        hitState = new DustHitState(this, stateMachine, "Hit", this);
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
            stateMachine.ChangeState(hitState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    public void ShowHealthBar() //显示血条
    {
        if (healthBar != null)
        {
            var cg = healthBar.GetComponent<CanvasGroup>();

            cg.alpha = 1f;

        }
    }

    public void HideHealthBar()
    {
        if (healthBar != null)
        {
            var cg = healthBar.GetComponent<CanvasGroup>();

            cg.alpha = 0f;

        }
    }
}