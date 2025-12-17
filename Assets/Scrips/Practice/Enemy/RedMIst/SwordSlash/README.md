# SwordSlash 剑气预制体使用说明

## 功能概述
- 剑气预制体类似于Arrow，但具有以下特性：
  - 碰到玩家不会消失，会对玩家造成伤害后继续飞行
  - 碰到带有"Wall"标签的物体才会消失
  - 具有最大生命周期，超时后自动消失

## 设置步骤

### 1. 创建预制体
1. 在Unity编辑器中，右键点击Project窗口，选择 `Create > Prefab`
2. 命名为 `SwordSlash`

### 2. 添加组件
将以下组件添加到预制体上：
- `Rigidbody2D`
  - 设置 `Body Type` 为 `Dynamic`
  - 设置 `Gravity Scale` 为 `0`（确保剑气不受重力影响）
- `BoxCollider2D` 或 `CircleCollider2D`
  - 设置 `Is Trigger` 为 `false`（确保是实体碰撞体）
- `SpriteRenderer`
  - 选择合适的剑气图片
  - 设置 `Sorting Layer` 为 `Trap`
- `SwordSlash` 脚本

### 3. 配置SwordSlash脚本参数
- `damageAmount`: 设置剑气造成的伤害值
- `lifetime`: 设置剑气的最大生命周期（秒）
- `wallTag`: 设置墙的标签（默认: "Wall"）

### 4. 层级和标签设置
- 将预制体的 `Layer` 设置为 `trap`
- 确保场景中的墙物体有 `Wall` 标签

### 5. 使用方法

#### 在代码中创建剑气：
```csharp
// 实例化剑气预制体
GameObject slashObj = Instantiate(swordSlashPrefab, spawnPosition, Quaternion.identity);

// 获取SwordSlash组件
SwordSlash swordSlash = slashObj.GetComponent<SwordSlash>();

// 设置伤害值（可选）
swordSlash.SetDamage(20f);

// 设置速度向量
Vector2 velocity = new Vector2(directionX, directionY) * speed;
swordSlash.SetVelocity(velocity);
```

#### 示例：从RedMist敌人发射剑气
```csharp
// 在RedMist的攻击方法中
public void LaunchSwordSlash(Vector2 direction)
{
    // 实例化剑气
    GameObject slash = Instantiate(swordSlashPrefab, transform.position, Quaternion.identity);
    
    // 设置速度
    float slashSpeed = 10f;
    slash.GetComponent<SwordSlash>().SetVelocity(direction.normalized * slashSpeed);
    
    // 设置伤害（使用敌人的伤害值）
    slash.GetComponent<SwordSlash>().SetDamage(stats.damage.GetValue());
}
```

## 注意事项
- 确保在Unity编辑器中已经创建了 `trap` 层级和 `Wall` 标签
- 剑气的图片资源需要单独提供
- 如果需要调整剑气的旋转方向，可以修改 `UpdateSlashRotation` 方法
- 碰到玩家后，剑气会继续飞行直到碰到墙或超过生命周期