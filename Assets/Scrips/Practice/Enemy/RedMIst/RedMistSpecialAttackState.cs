
using System.Collections;
using UnityEngine;

public class RedMistSpecialAttackState : EnemyState
{
    private RedMist enemy;
    private float generationDistance; // 生成剑气的间隔距离
    private Vector3 leftlastGenerationPosition; // 左侧上次生成剑气的位置
    private Vector3 rightlastGenerationPosition; // 右侧上次生成剑气的位置
    private int maxSlashCount = 12; // 最大生成剑气数量
    private int currentSlashCount = 0;
    private float upOffset;
    public float releaseCooldown = 2f;
    public RedMistSpecialAttackState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemy, _stateMachine, _animBoolName)
    {
        enemy = (RedMist)_enemy;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 8f; // 特殊攻击持续时间
        enemy.ZeroVelocity(); // 停止移动
        generationDistance = enemy.generationDistance; // 从RedMist脚本获取生成距离
        upOffset = enemy.upOffset; // 从RedMist脚本获取偏移距离
        
        // 重置计数和位置
        currentSlashCount = 0;
        leftlastGenerationPosition = enemy.transform.position;
        rightlastGenerationPosition = enemy.transform.position;
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
    }

    public override void Updata()
    {
        base.Updata();
        
        // 检查血量，如果血量为0，切换到死亡状态
        if (enemy.stats != null && enemy.stats.currentHealth <= 0)
        {
            stateMachine.ChangeState(enemy.deathState);
            return;
        }
        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.disappearState);
            return;
        }
        if(releaseCooldown<=0)
        {
            ReleaseSpecialSwordSlash();
            releaseCooldown=2f;
        }
        releaseCooldown-=Time.deltaTime;

    }
    
    // 在RedMist附近随机位置生成剑气
    private void GenerateSwordSlash(Vector3 moveoffset)
    {
        if (enemy.swordSlashPrefab == null)
        {
            Debug.LogError("SwordSlash prefab is not assigned!");
            return;
        }
        
        Vector3 spawnPosition1 = leftlastGenerationPosition + moveoffset;
        Vector3 spawnPosition2 = rightlastGenerationPosition - moveoffset;
        
        // 优先使用特殊攻击剑气预制体
        GameObject slashPrefab = enemy.specialSwordSlashPrefab;
        
        // 如果特殊攻击剑气预制体未设置，则使用普通剑气预制体
        if (slashPrefab == null)
        {
            slashPrefab = enemy.swordSlashPrefab;
        }
        
        // 生成剑气预制体，不需要设置方向
        Object.Instantiate(slashPrefab, spawnPosition1+new Vector3(0,upOffset,0), Quaternion.identity);
        Object.Instantiate(slashPrefab, spawnPosition2+new Vector3(0,upOffset,0), Quaternion.identity);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
        
        // 重置特殊攻击冷却计时器
        enemy.specialAttackTimer = enemy.specialAttackInterval;
    }

    void ReleaseSpecialSwordSlash()
    {
        
        // 检查剑气预制体是否存在
        if (enemy.swordSlashPrefab == null)
        {
            Debug.LogWarning("SwordSlashPrefab is not assigned in RedMist");
            return;
        }
        //偏移距离
        Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0, 0);
        Vector3 moveoffset=new Vector3(generationDistance, 0, 0);
        //初始化初始位置为当前位置
        leftlastGenerationPosition = enemy.transform.position+offset;
        rightlastGenerationPosition = enemy.transform.position+offset;
        
        //优先使用特殊攻击剑气预制体
        GameObject slashPrefab = enemy.specialSwordSlashPrefab;
        if (slashPrefab == null)
        {
            slashPrefab = enemy.swordSlashPrefab;
        }
        
        //在初始位置基础上生成剑气
        Object.Instantiate(slashPrefab, leftlastGenerationPosition+new Vector3(0,upOffset,0), Quaternion.identity);
        // 每隔指定距离生成剑气，直到达到最大数量
         while(currentSlashCount <= maxSlashCount)
        {
            GenerateSwordSlash(moveoffset);
            leftlastGenerationPosition +=moveoffset;
            rightlastGenerationPosition -=moveoffset;
            currentSlashCount+=2;
        }
        currentSlashCount=0;
        
       
    }
}