# 城镇敌人伤害系统使用指南

## 概述

本指南将介绍如何为城镇敌人(Enemy_town和Enemy_town2)实现完整的伤害系统，包括敌人受到伤害、攻击玩家、以及死亡逻辑。

## 系统组件

### 核心脚本
1. **Enemy_town.cs** - 第一种城镇敌人的行为脚本
2. **Enemy_town2.cs** - 第二种城镇敌人的行为脚本
3. **EnemyStats_town.cs** - 城镇敌人的属性和伤害处理脚本
4. **CharacterStats.cs** - 角色属性的基础类(位于Scrips/Stat目录)

## 组件设置步骤

### 1. 为敌人添加基础组件

1. 在Unity编辑器中选择敌人游戏对象
2. 添加以下基础组件：
   - Rigidbody2D(设置为Dynamic)
   - Collider2D(Box或Capsule)
   - Animator(可选，用于动画)

### 2. 添加伤害系统组件

1. 添加EnemyStats_town组件
2. 在EnemyStats_town组件中设置敌人的属性值：
   - 生命值(maxHealth)
   - 攻击力(damage)
   - 防御力(armor)
   - 闪避率(evasion)
   - 暴击率(critChange)
   - 暴击伤害(critPower)

### 3. 配置碰撞器和触发器

1. 确保敌人有一个碰撞器(Collider2D)用于物理碰撞
2. 添加一个Trigger Collider2D用于检测玩家(可选，取决于敌人的攻击方式)
3. 设置正确的Layer(通常为"Enemy")

### 4. 设置攻击参数

1. 在Enemy_town或Enemy_town2组件中设置攻击参数：
   - attackDamage: 敌人对玩家的基础伤害值
   - checkAttack: 攻击检测点的Transform
   - checkAttackRange: 攻击检测范围

### 5. 设置伤害效果参数

1. knockbackDuration: 受到伤害后的击退持续时间
2. knockbackDirection: 受到伤害后的击退方向向量
3. 添加EntityFX子对象(用于受击光效)

## 伤害系统工作流程

### 1. 敌人攻击玩家

当敌人碰撞到玩家或检测到玩家进入攻击范围时，会调用AttackPlayer()方法：

```csharp
// Enemy_town.cs/Enemy_town2.cs
private void AttackPlayer()
{
    if (playerStats != null)
    {
        // 计算伤害
        int totalDamage = Mathf.RoundToInt(attackDamage);
        
        // 对玩家造成伤害
        playerStats.TakeDamage(totalDamage);
    }
}
```

### 2. 敌人受到伤害

当其他角色(如玩家)对敌人造成伤害时，会调用EnemyStats_town.TakeDamage()方法：

```csharp
// EnemyStats_town.cs
public override void TakeDamage(int _damage)
{
    base.TakeDamage(_damage);
    
    // 调用对应的伤害效果
    if (enemy_town != null)
    {
        enemy_town.DamageEffect();
    }
    else if (enemy_town2 != null)
    {
        enemy_town2.DamageEffect();
    }
}
```

### 3. 伤害效果

敌人受到伤害后会播放光效和击退效果：

```csharp
// Enemy_town.cs/Enemy_town2.cs
public virtual void DamageEffect()
{
    // 播放受击光效
    if (fx != null)
    {
        fx.StartCoroutine("FlashFX");
    }
    
    // 开始击退
    StartCoroutine(HitKnockback());
}
```

### 4. 敌人死亡

当敌人的生命值降为0时，会调用EnemyStats_town.Die()方法：

```csharp
// EnemyStats_town.cs
protected override void Die()
{
    base.Die();
    
    // 调用对应的死亡逻辑
    if (enemy_town != null)
    {
        enemy_town.Die();
    }
    else if (enemy_town2 != null)
    {
        enemy_town2.Die();
    }
}
```

## 调试与优化

### 调试模式

在Enemy_town和Enemy_town2组件中，可以启用showDebugLogs选项来显示调试日志，帮助追踪伤害系统的工作流程。

### 常见问题

1. **敌人无法攻击玩家**
   - 检查玩家的Tag是否为"Player"
   - 检查碰撞器是否正确设置
   - 确保playerStats已正确获取

2. **敌人不受伤害**
   - 检查EnemyStats_town组件是否已添加
   - 检查CharacterStats中的属性设置
   - 确保TakeDamage方法被正确调用

3. **伤害效果不显示**
   - 检查EntityFX组件是否已正确添加
   - 确保FlashFX协程存在于EntityFX脚本中

## 扩展建议

1. **添加血条UI**
   - 可以参考HealthBar目录下的LittleMosterHealthBar_UI.cs实现敌人血条

2. **添加动画系统**
   - 为敌人的攻击、受击和死亡状态添加动画
   - 在DamageEffect和Die方法中触发相应的动画

3. **添加音效**
   - 在攻击、受击和死亡时播放相应的音效
   - 可以使用AudioManager来管理音效

4. **添加掉落系统**
   - 在Die方法中添加物品掉落逻辑

## 示例代码

### 玩家攻击敌人示例

```csharp
// 在玩家攻击脚本中
private void AttackEnemy(Enemy_town enemy)
{
    if (enemy != null && enemy.stats != null)
    {
        // 计算玩家的伤害
        int playerDamage = playerStats.damage.GetValue() + playerStats.strength.GetValue();
        
        // 对敌人造成伤害
        enemy.stats.TakeDamage(playerDamage);
    }
}
```

## 总结

通过以上步骤，您可以为城镇敌人实现完整的伤害系统。该系统支持敌人攻击玩家、受到玩家伤害、以及死亡逻辑，并且可以通过调整参数来平衡游戏难度。
