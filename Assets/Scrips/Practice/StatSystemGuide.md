# 角色属性系统使用指南

## 系统概述

Stat文件夹中的脚本实现了一个完整的角色属性系统，用于管理游戏角色的各种属性和战斗计算。该系统由两个核心脚本组成：

1. **Stat.cs** - 基础属性类，用于管理单个属性的值和修饰符
2. **CharacterStats.cs** - 角色属性类，整合多个Stat实例，管理角色的完整属性和战斗功能

## 核心类详解

### 1. Stat 类

**功能**：管理单个属性的值，支持基础值和修饰符（buff/debuff）

**主要属性**：
- `baseValue` - 属性基础值
- `modifiers` - 属性修饰符列表

**主要方法**：
```csharp
// 获取最终属性值（基础值 + 所有修饰符）
int GetValue()

// 设置基础值
void SetDefalutValue(int _value)

// 添加修饰符
void AddModifier(int _modifier)

// 移除修饰符
void RemoveModifier(int _modifier)

// 清除所有修饰符
void ClearModifiers()
```

### 2. CharacterStats 类

**功能**：管理角色的完整属性系统，处理战斗计算

**主要属性分组**：

#### 主要属性
- `strength` - 力量（增加伤害和爆伤）
- `agility` - 敏捷（增加闪避）
- `intelligence` - 智力（魔法伤害相关）
- `vitality` - 生命力（每点+5生命值）
- `magic` - 魔力（魔法值相关）

#### 攻击属性
- `damage` - 基础伤害
- `critChange` - 暴击几率
- `critPower` - 暴击伤害
- `critEquipment` - 装备提供的暴击概率

#### 防御属性
- `maxHealth` - 最大生命值
- `armor` - 护甲（减少伤害）
- `evasion` - 闪避值

#### 其他属性
- `currentHealth` - 当前生命值
- `onHealthChanged` - 生命值变化时的回调委托

**主要方法**：
```csharp
// 对目标造成伤害
void DoDamage(CharacterStats _targetStats)

// 计算并返回最大生命值
int GetMaxHealthValue()

// 回复生命值
void RecoverHP()

// 承受伤害
void TakeDamage(int _damage)
```

### 3. PlayerStats 类

**功能**：玩家专属的属性类，继承自CharacterStats

**主要特点**：
- 与Player组件集成，用于管理玩家角色的属性
- 重写了TakeDamage和Die方法，添加了玩家特有的伤害效果和死亡逻辑

**主要方法**：
```csharp
// 玩家承受伤害
public override void TakeDamage(int _damage)
{
    base.TakeDamage(_damage);
    player.DamageEffect(); // 调用玩家的伤害效果
}

// 玩家死亡
protected override void Die()
{
    base.Die();
    player.Die(); // 调用玩家的死亡方法
}
```

### 4. EnemyStats 类

**功能**：敌人专属的属性类，继承自CharacterStats

**主要特点**：
- 与Enemy组件集成，用于管理敌人角色的属性
- 重写了TakeDamage和Die方法，添加了敌人特有的伤害效果和死亡逻辑

**主要方法**：
```csharp
// 敌人承受伤害
public override void TakeDamage(int _damage)
{
    base.TakeDamage(_damage);
    enemy.DamageEffect(); // 调用敌人的伤害效果
}

// 敌人死亡
protected override void Die()
{
    base.Die();
    enemy.Die(); // 调用敌人的死亡方法
}
```

## 使用方法

### 1. 基础设置

#### 玩家设置

**步骤1**：将PlayerStats组件添加到玩家对象上

**步骤2**：确保玩家对象上有Player组件（用于处理伤害效果和死亡逻辑）

**步骤3**：在Inspector面板中配置初始属性值

**步骤4**：实现生命值显示UI（可选）
```csharp
// 在UI脚本中
private void Start()
{
    PlayerStats playerStats = GetComponent<PlayerStats>();
    playerStats.onHealthChanged += UpdateHealthUI;
}

private void UpdateHealthUI()
{
    // 更新血条显示
    healthSlider.value = (float)playerStats.currentHealth / playerStats.GetMaxHealthValue();
}
```

#### 敌人设置

**步骤1**：将EnemyStats组件添加到敌人对象上

**步骤2**：确保敌人对象上有Enemy组件（用于处理伤害效果和死亡逻辑）

**步骤3**：在Inspector面板中配置初始属性值

**步骤4**：设置敌人AI逻辑，使其能够攻击玩家


### 2. 战斗系统使用

#### 玩家攻击敌人

```csharp
// 在玩家攻击脚本中
public void AttackEnemy(GameObject enemy)
{
    PlayerStats playerStats = GetComponent<PlayerStats>();
    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
    
    if (enemyStats != null)
    {
        playerStats.DoDamage(enemyStats);
    }
}
```

#### 敌人攻击玩家

```csharp
// 在敌人AI脚本中
public void AttackPlayer(GameObject player)
{
    EnemyStats enemyStats = GetComponent<EnemyStats>();
    PlayerStats playerStats = player.GetComponent<PlayerStats>();
    
    if (playerStats != null)
    {
        enemyStats.DoDamage(playerStats);
    }
}
```

#### 直接伤害示例

```csharp
// 在陷阱或技能脚本中
public void ApplyDamage(GameObject target, int damage)
{
    CharacterStats stats = target.GetComponent<CharacterStats>();
    if (stats != null)
    {
        stats.TakeDamage(damage);
    }
}
```

### 3. 属性修改（Buff/Debuff）

**添加增益效果**：
```csharp
// 增加力量5点（持续增益）
CharacterStats stats = GetComponent<CharacterStats>();
stats.strength.AddModifier(5);

// 增加10%暴击率（通过装备）
stats.critEquipment.AddModifier(10);
```

**移除增益效果**：
```csharp
// 移除力量增益
stats.strength.RemoveModifier(5);
```

**临时效果（带持续时间）**：
```csharp
IEnumerator ApplyTemporaryBuff(CharacterStats stats, float duration)
{
    // 添加增益
    stats.damage.AddModifier(10);
    
    // 等待持续时间
    yield return new WaitForSeconds(duration);
    
    // 移除增益
    stats.damage.RemoveModifier(10);
}

// 使用方法
StartCoroutine(ApplyTemporaryBuff(GetComponent<CharacterStats>(), 5f));
```

### 4. 实现玩家和敌人的核心组件

#### 玩家组件示例（Player.cs）

```csharp
using UnityEngine;

public class Player : MonoBehaviour
{
    public void DamageEffect()
    {
        // 实现玩家受伤效果
        Debug.Log("玩家受伤！");
        // 播放受伤动画
        // 播放受伤音效
        // 屏幕震动等效果
    }
    
    public void Die()
    {
        // 实现玩家死亡逻辑
        Debug.Log("玩家死亡！");
        // 播放死亡动画
        // 显示游戏结束界面
        // 停止游戏逻辑
    }
}
```

#### 敌人组件示例（Enemy.cs）

```csharp
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void DamageEffect()
    {
        // 实现敌人受伤效果
        Debug.Log("敌人受伤！");
        // 播放受伤动画
        // 播放受伤音效
        // 敌人颜色变化等效果
    }
    
    public void Die()
    {
        // 实现敌人死亡逻辑
        Debug.Log("敌人死亡！");
        // 播放死亡动画
        // 生成死亡效果
        // 给予玩家奖励
        // 销毁敌人对象
        Destroy(gameObject, 1f); // 1秒后销毁敌人
    }
}
```

## 战斗计算流程

### 1. 攻击计算流程

```
攻击者.DoDamage(目标) → 检查目标是否闪避 → 计算基础伤害 → 检查是否暴击 → 计算暴击伤害 → 检查目标护甲 → 目标承受伤害
```

### 2. 关键计算公式

#### 闪避计算
```
总闪避值 = 目标.evasion + 目标.agility
如果随机数(0-100) < 总闪避值 → 攻击被闪避
```

#### 伤害计算
```
基础伤害 = 攻击者.damage + 攻击者.strength
如果暴击 → 最终伤害 = 基础伤害 × (攻击者.critPower + 攻击者.strength) × 0.01
否则 → 最终伤害 = 基础伤害
```

#### 护甲减免
```
减免后伤害 = 最终伤害 - 目标.armor
如果减免后伤害 ≤ 0 → 伤害 = 1
```

#### 暴击概率
```
总暴击概率 = 攻击者.critChange + 攻击者.critEquipment
如果随机数(0-100) < 总暴击概率 → 触发暴击
```

## 最佳实践

1. **合理设置初始属性**：根据角色定位（坦克、输出、法师）设置合适的初始属性值

2. **使用修饰符而非直接修改基础值**：对于临时效果（buff/debuff），使用AddModifier/RemoveModifier方法

3. **实现死亡逻辑**：为不同类型的角色（玩家、敌人）实现各自的死亡逻辑

4. **添加视觉反馈**：在生命值变化、暴击、闪避时添加视觉和音效反馈

5. **性能优化**：避免在Update中频繁调用GetValue()方法，适当缓存计算结果

## 扩展建议

1. **添加魔法系统**：扩展CharacterStats类，添加魔法伤害、魔法防御等属性

2. **添加等级系统**：实现基于等级的属性成长

3. **添加装备系统**：创建装备类，与CharacterStats集成

4. **添加技能系统**：实现基于属性的技能伤害计算

5. **添加属性上限**：为某些属性设置最大值限制

## 常见问题

### Q: 为什么伤害计算结果与预期不符？
A: 检查以下几点：
- 确保所有相关属性都正确配置
- 检查是否有未移除的修饰符影响结果
- 查看控制台输出，是否有闪避或暴击发生

### Q: 如何实现治疗功能？
A: 可以扩展CharacterStats类，添加治疗方法：
```csharp
public void Heal(int amount)
{
    currentHealth += amount;
    if (currentHealth > GetMaxHealthValue())
    {
        currentHealth = GetMaxHealthValue();
    }
    
    if (onHealthChanged != null)
    {
        onHealthChanged();
    }
}
```

### Q: 如何实现多段伤害？
A: 可以多次调用DoDamage方法，或修改DoDamage方法支持多段伤害参数

## 示例代码

### 完整的玩家和敌人交互示例

#### 玩家控制器示例

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStats stats;
    private GameObject currentTarget;
    
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        
        // 配置初始属性
        stats.strength.SetDefalutValue(10);
        stats.agility.SetDefalutValue(8);
        stats.vitality.SetDefalutValue(12);
        stats.damage.SetDefalutValue(5);
        stats.critChange.SetDefalutValue(10);
        
        // 绑定生命值变化事件
        stats.onHealthChanged += UpdateHealthUI;
    }
    
    void Update()
    {
        // 寻找最近的敌人
        FindNearestEnemy();
        
        // 攻击目标
        if (Input.GetKeyDown(KeyCode.Space) && currentTarget != null)
        {
            AttackEnemy(currentTarget);
        }
        
        // 使用治疗药水
        if (Input.GetKeyDown(KeyCode.H))
        {
            HealPlayer(20);
        }
        
        // 施加力量增益
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(ApplyStrengthBuff(3f));
        }
    }
    
    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = Mathf.Infinity;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                currentTarget = enemy;
            }
        }
    }
    
    void AttackEnemy(GameObject enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            stats.DoDamage(enemyStats);
            Debug.Log($"玩家攻击了敌人，造成伤害！");
        }
    }
    
    void UpdateHealthUI()
    {
        // 更新血条UI
        Debug.Log($"当前生命值: {stats.currentHealth}/{stats.GetMaxHealthValue()}");
    }
    
    void HealPlayer(int amount)
    {
        stats.currentHealth += amount;
        if (stats.currentHealth > stats.GetMaxHealthValue())
        {
            stats.currentHealth = stats.GetMaxHealthValue();
        }
        stats.onHealthChanged?.Invoke();
        Debug.Log($"玩家回复了 {amount} 点生命值！");
    }
    
    IEnumerator ApplyStrengthBuff(float duration)
    {
        stats.strength.AddModifier(5);
        Debug.Log("力量增益已施加！");
        yield return new WaitForSeconds(duration);
        stats.strength.RemoveModifier(5);
        Debug.Log("力量增益已结束！");
    }
}
```

#### 敌人AI示例

```csharp
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private EnemyStats stats;
    private GameObject player;
    public float attackRange = 2f;
    public float moveSpeed = 1f;
    
    void Start()
    {
        stats = GetComponent<EnemyStats>();
        player = GameObject.FindWithTag("Player");
        
        // 配置初始属性
        stats.strength.SetDefalutValue(5);
        stats.vitality.SetDefalutValue(8);
        stats.damage.SetDefalutValue(3);
    }
    
    void Update()
    {
        if (player == null) return;
        
        // 计算与玩家的距离
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        // 如果在攻击范围内，攻击玩家
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        // 否则，向玩家移动
        else
        {
            MoveTowardsPlayer();
        }
    }
    
    void MoveTowardsPlayer()
    {
        // 向玩家方向移动
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
    
    void AttackPlayer()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            stats.DoDamage(playerStats);
            Debug.Log($"敌人攻击了玩家，造成伤害！");
        }
    }
}
```

希望这个指南能帮助你理解和使用角色属性系统！如果有任何问题或需要进一步的帮助，请随时咨询。