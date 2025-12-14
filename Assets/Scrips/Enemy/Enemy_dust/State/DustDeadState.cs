using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustDeadState : EnemyState
{
    private Enemy_Dust enemy;
    public DustDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Dust _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();

        enemy.ZeroVelocity();//死亡时速度为零


        var hb = enemy.GetComponentInChildren<LittleMosterHealthBar_UI>();
        if (hb != null)
        {
            hb.gameObject.SetActive(false);
        }
    }
    public override void Updata()
    {
        base.Updata();
        
    }
}
