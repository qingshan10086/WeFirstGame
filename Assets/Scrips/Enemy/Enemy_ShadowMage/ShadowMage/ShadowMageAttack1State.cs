using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShadowMageAttack1State : EnemyState
{
    private Enemy_ShadowMage enemy;//获取影法师
    public GameObject shadowFollower;//获取影子随从
    private float StateTimer;//该状态持续时间

    private float attackCooldown = 2f;//随从攻击间隔
    private float attackCooldownTimer = 0f;
    

    private  UnityEngine.Transform player;//申明玩家位置信息
    private float followerWays;//判断是那种攻击方式
    public bool follower1=false;//随从攻击方式1
    public bool follower2=false;//随从攻击方式2




    public ShadowMageAttack1State(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_ShadowMage enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter(); 
        StateTimer = 21f;//该状态持续时间
        shadowFollower = GameObject.Find("ShadowFollower");//获取随从信息


        enemy.stat.evasion.AddModifier(101);//添加超过100的闪避值，使怪物在此状态不能被攻击
    }

    public override void Exit()
    {
        base.Exit();

        followerWays = -1;//关闭随从攻击
        follower1 = false;
        follower2 = false;

        shadowFollower.transform.position = new Vector2(100,100);//使随从离开屏幕

        enemy.stat.evasion.RemoveModifier(101);//去除该闪避值
    }

    public override void Updata()
    {
        base.Updata();


        player = PlayerManager.instance.player.transform;//每帧获取，不知道要不要考虑性能



        attackCooldownTimer -= Time.deltaTime;
        if (attackCooldownTimer < 0)
        {
            followerWays = Random.Range(1, 100);//以该值来判断随从的攻击方式        
            attackCooldownTimer = attackCooldown;
        }
        else
        { 
            followerWays = 101;
        }


 
        if (followerWays <= 50&&followerWays>=0)
        {
               follower1 = true;
               follower2 = false;
               shadowFollower.transform.position = new Vector2(player.position.x - 1, player.position.y +15f);//控制随从出现位置
        }

        if (followerWays>50 && followerWays <=100)
        {
               follower1 = false;
               follower2 = true;
               shadowFollower.transform.position = new Vector2(player.position.x - 30, player.position.y);//控制随从出现位置
        }
       

        StateTimer -= Time.deltaTime;
        if (StateTimer<0)//状态持续一定时间后回到站立状态
        {
            stateMachine.ChangeState(enemy.idleState);
        }

    }
}
