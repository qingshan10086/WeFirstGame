using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Sculpture : Enemy
{
    Player player;
    public SculptureState sculptureState { get;private set; }
    public SculptureDeathState sculptureDeathState { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        sculptureState = new SculptureState(this, stateMachine, "Sculpture");
        sculptureDeathState = new SculptureDeathState(this, stateMachine, "Die");

        player = FindObjectOfType<Player>();
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(sculptureState);
    }
    public override void Die()
    {
        base.Die();
        player.stats.currentHealth = player.stats.GetMaxHealthValue();
        player.stats.onHealthChanged?.Invoke();
        stateMachine.ChangeState(sculptureDeathState);
    }
}

public class SculptureState : EnemyState
{
    private Sculpture sculpture;
    public SculptureState(Sculpture sculpture, EnemyStateMachine stateMachine, string animBoolName) 
        : base(sculpture, stateMachine, animBoolName)
    {
        this.sculpture = sculpture;
    }
}

public class SculptureDeathState : EnemyState
{
    private Sculpture sculpture;
    public SculptureDeathState(Sculpture sculpture, EnemyStateMachine stateMachine, string animBoolName) 
        : base(sculpture, stateMachine, animBoolName)
    {
        this.sculpture = sculpture;
    }
    //¿ªÆô´Ý»ÙÐ­³Ì
    public override void Enter()
    {
        base.Enter();
        sculpture.StartCoroutine(DestroyAfterAnimation());
    }
    private IEnumerator DestroyAfterAnimation()
    {
        //Á½Ãëºó´Ý»ÙµñÏñ
        yield return new WaitForSeconds(2f);
        GameObject.Destroy(sculpture.gameObject);
    }
}