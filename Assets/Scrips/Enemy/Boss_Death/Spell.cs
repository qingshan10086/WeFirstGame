using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Enemy
{
    public float lifeTime = 1.5f;
    public SpellIdleState idleState { get; private set; }

    protected override void Awake()
    { 
        base.Awake();
        idleState = new SpellIdleState(this, stateMachine, "Idle");
        lifeTime = 1.5f;
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        base.Update();
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    
}

public class SpellIdleState : EnemyState
{ 
    public SpellIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
    {
    }
}