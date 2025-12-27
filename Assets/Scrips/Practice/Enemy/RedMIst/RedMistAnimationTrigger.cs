using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMistAnimationTrigger : MonoBehaviour
{
    private RedMist enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<RedMist>();
    }

    // 动画事件函数 - 动画完成时调用
    public void AnimationFinishTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    // 兼容旧的动画触发方法
    public void animTriggerEvent()
    {
        enemy.animTriggerEvent();
    }

    // 动画事件函数 - 攻击开始时调用
    public void AttackStartTrigger()
    {
        // 可以在这里添加攻击开始时的效果，比如粒子效果、音效等
    }

    // 动画事件函数 - 攻击命中时调用（圆形检测）
    public void AttackHitTrigger()
    {
        // 检测攻击范围内的玩家
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.checkAttack.position, enemy.checkAttackRange, enemy.whatIsPlayer);
        foreach (Collider2D collider in hitColliders)
        {
            // 对玩家造成伤害
            collider.GetComponent<PlayerStats>().TakeDamage(enemy.stats.damage.GetValue());
        }
    }
    
    // 动画事件函数 - 矩形攻击命中时调用
    public void RectangleAttackHitTrigger()
    {
        // 直接使用checkAttackBox的位置
        Vector2 attackPosition = enemy.checkAttackBox.position;
        
        // 使用checkAttackBox的旋转角度，将弧度转换为角度
        float rotation = enemy.checkAttackBox.rotation.z * Mathf.Rad2Deg;
        
        // 使用矩形检测攻击范围内的玩家，应用旋转角度
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackPosition, enemy.checkAttackBoxSize, rotation, enemy.whatIsPlayer);
        foreach (Collider2D collider in hitColliders)
        {
            // 对玩家造成伤害
            if (enemy.dashAttackDamageTimer <= 0)
            {
                collider.GetComponent<PlayerStats>().TakeDamage(enemy.dashDamage);
                enemy.dashAttackDamageTimer = enemy.dashAttackCooldown;
            }
        }
    }
    
    // 动画事件函数 - 圆形攻击命中时调用
    public void CircleAttackHitTrigger()
    {
        // 直接使用checkCircleAttack的位置
        Vector2 attackPosition = enemy.checkCircleAttack.position;
        
        // 使用圆形检测攻击范围内的玩家
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPosition, enemy.checkCircleAttackRadius, enemy.whatIsPlayer);
        foreach (Collider2D collider in hitColliders)
        {
            // 对玩家造成伤害
            collider.GetComponent<PlayerStats>().TakeDamage(enemy.fallDamage);
        }
    }

    // 动画事件函数 - 冲刺攻击开始时调用
    public void DashAttackStartTrigger()
    {
        // 可以在这里添加冲刺攻击开始时的效果
    }

    // 动画事件函数 - 特殊攻击开始时调用
    public void SpecialAttackStartTrigger()
    {
        // 可以在这里添加特殊攻击开始时的效果
    }
    
    // 动画事件函数 - 发射单个剑气（攻击2）
    public void ReleaseSingleSwordSlash()
    {
        if (enemy == null || enemy.swordSlashPrefab == null)
        {
            Debug.LogWarning("RedMistAnimationTrigger: 无法发射剑气，缺少必要引用");
            return;
        }
        
        // 计算剑气生成位置：RedMist位置正前方
        Vector3 spawnPosition = enemy.transform.position + new Vector3(enemy.faceDirection * 0.5f, enemy.slashDistance, 0);
        
        // 实例化剑气
        GameObject slash = Object.Instantiate(enemy.swordSlashPrefab, spawnPosition, Quaternion.identity);
        SwordSlash swordSlash = slash.GetComponent<SwordSlash>();
        if (swordSlash != null)
        {
            // 向当前朝向发射剑气
            swordSlash.SetDirection(-enemy.faceDirection);
        }
    }
    
    // 动画事件函数 - 向左右两端发射剑气（攻击3）
    public void ReleaseDualSwordSlash()
    {
        if (enemy == null || enemy.swordSlashPrefab == null)
        {
            Debug.LogWarning("RedMistAnimationTrigger: 无法发射剑气，缺少必要引用");
            return;
        }
        
        // 计算剑气生成位置：RedMist位置下方1.74f
        Vector3 spawnPosition = enemy.transform.position + new Vector3(0, -3.2f, 0);
        
        // 向左释放剑气
        GameObject leftSlash = Object.Instantiate(enemy.swordSlashPrefab, spawnPosition, Quaternion.identity);
        SwordSlash swordSlashLeft = leftSlash.GetComponent<SwordSlash>();
        if (swordSlashLeft != null)
        {
            swordSlashLeft.SetDirection(-1); // 向左移动
        }
        
        // 向右释放剑气
        GameObject rightSlash = Object.Instantiate(enemy.swordSlashPrefab, spawnPosition, Quaternion.identity);
        SwordSlash swordSlashRight = rightSlash.GetComponent<SwordSlash>();
        if (swordSlashRight != null)
        {
            swordSlashRight.SetDirection(1); // 向右移动
        }
    }

    // 动画事件函数 - 在玩家身旁出现（左右随机）
    public void AppearAtPlayerSide()
    {
        if (enemy.player != null)
        {
            // 随机选择左右方向
            int direction = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            
            // 计算出现位置：玩家位置加上左右方向乘以出现距离
            Vector3 appearPosition = enemy.player.transform.position + new Vector3(direction * enemy.sideAppearDistance, 0, 0);
            
            // 设置RedMist的位置
            enemy.transform.position = appearPosition;
            
            // 翻转RedMist朝向玩家
            if (enemy.faceDirection != direction)
            {
                enemy.Flip();
            }
        }
    }

    // 动画事件函数 - 在玩家斜上方出现（角度随机）
    public void AppearAtPlayerDiagonal()
    {
        if (enemy.player != null)
        {
            // 随机左右方向，-1表示左，1表示右
            int direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            
            // 随机角度，在30度到60度之间
            float angle = UnityEngine.Random.Range(45f, 60f);
            float radians = angle * Mathf.Deg2Rad;
            
            // 计算出现位置：玩家位置加上斜上方的偏移量，考虑左右方向
            Vector3 appearPosition = enemy.player.transform.position + new Vector3(
                direction * Mathf.Cos(radians) * enemy.diagonalAppearDistance,
                Mathf.Sin(radians) * enemy.diagonalAppearDistance,
                0
            );
            
            // 设置RedMist的位置
            enemy.transform.position = appearPosition;
            
            // 确定朝向：如果在玩家左侧，方向为-1，否则为1
            int faceDirection = appearPosition.x < enemy.player.transform.position.x ? -1 : 1;
            
            // 翻转RedMist朝向玩家
            if (enemy.faceDirection != direction)
            {
                enemy.Flip();
            }
        }
    }

    // 动画事件函数 - 在玩家正上方出现
    public void AppearAtPlayerTop()
    {
        if (enemy.player != null)
        {
            // 计算出现位置：玩家位置正上方指定距离
            Vector3 appearPosition = enemy.player.transform.position + new Vector3(0, enemy.topAppearDistance, 0);
            
            // 设置RedMist的位置
            enemy.transform.position = appearPosition;
        }
    }
    
    // 动画事件函数 - 在玩家上方生成特殊剑气
    public void GenerateSpecialSwordSlashAbovePlayer()
    {
        if (enemy == null || enemy.player == null || enemy.specialSwordSlashPrefab == null)
        {
            Debug.LogWarning("RedMistAnimationTrigger: 无法生成特殊剑气，缺少必要引用");
            return;
        }
        
        // 在玩家上方生成特殊剑气
        Vector3 spawnPosition = enemy.player.transform.position + new Vector3(0, enemy.upOffset, 0);
        
        // 实例化特殊剑气
        GameObject specialSlash = Object.Instantiate(enemy.specialSwordSlashPrefab, spawnPosition, Quaternion.identity);
        SwordSlash swordSlash = specialSlash.GetComponent<SwordSlash>();

    }
}
    