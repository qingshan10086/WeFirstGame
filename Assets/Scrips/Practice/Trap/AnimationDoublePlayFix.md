# 动画执行两遍问题排查与解决方案

## 问题分析
通过对代码的分析，发现可能导致动画执行两遍的几个主要原因：

### 1. 动画状态参数设置不一致
- 在 `BreakableBrickAnimationEvent.cs` 中，动画事件处理方法直接使用了硬编码的参数名：
  ```csharp
  public void OnBreakAnimationComplete()
  {
      Debug.Log("破碎动画播放完成");
      animator.SetBool("Break", false);
  }
  ```
- 但在 `BreakableBrick.cs` 中，这些参数名是可配置的：
  ```csharp
  [SerializeField] private string breakBoolName = "Break";
  ```
- 这种不一致可能导致动画状态机在参数重置时出现问题

### 2. 脚本组件可能重复挂载
- 如果 `BreakableBrick` 或 `BreakableBrickAnimationEvent` 脚本被重复挂载到同一个游戏对象或其子对象上
- 可能会导致动画控制逻辑被多次执行

### 3. 动画控制器配置问题
- 动画控制器中的过渡条件设置不当，可能导致动画在完成后又立即重新触发
- 例如，如果动画的退出条件设置不正确，或者动画状态的循环模式被意外启用

### 4. 动画事件配置问题
- 如果在动画编辑器中错误地多次添加了相同的动画事件
- 可能会导致事件处理方法被多次调用，从而触发多次动画

## 解决方案

### 修复1：统一动画参数名
将 `BreakableBrickAnimationEvent.cs` 中的硬编码参数名改为使用 `BreakableBrick` 脚本中的配置：

```csharp
// 修改前
animator.SetBool("Break", false);
animator.SetBool("Respawn", false);

// 修改后
if (breakableBrick != null)
{
    animator.SetBool(breakableBrick.breakBoolName, false);
    animator.SetBool(breakableBrick.respawnBoolName, false);
}
```

### 修复2：检查脚本组件是否重复挂载
1. 选中砖块游戏对象
2. 在Inspector面板中检查是否重复挂载了相同的脚本
3. 如果发现重复，移除多余的脚本组件

### 修复3：检查动画控制器配置
1. 打开动画控制器（Animator Controller）
2. 检查以下几点：
   - 确保动画状态的「Loop Time」未被勾选
   - 检查动画过渡条件是否正确设置
   - 确保动画完成后能正确回到初始状态

### 修复4：检查动画事件配置
1. 打开动画文件（.anim）
2. 在动画时间轴中检查是否多次添加了相同的事件
3. 如果发现重复，移除多余的事件

## 调试步骤

1. **添加调试脚本**：
   - 将 `BreakableBrickDebug.cs` 脚本挂载到砖块游戏对象上
   - 运行游戏，观察Console中的调试信息

2. **监控动画状态变化**：
   - 观察 `breakTriggerCount` 和 `respawnTriggerCount` 的值
   - 检查是否有重复触发的情况

3. **检查参数设置**：
   - 确保 `BreakableBrick` 脚本中的动画参数名与动画控制器中的参数名一致
   - 确保 `BreakableBrickAnimationEvent` 脚本正确引用了主脚本

## 代码修复建议

### 修改 BreakableBrickAnimationEvent.cs
```csharp
/// <summary>
/// 破碎动画播放完成事件
/// 由动画事件在破碎动画最后一帧调用
/// </summary>
public void OnBreakAnimationComplete()
{
    Debug.Log("破碎动画播放完成");
    
    // 使用主脚本中的配置参数名，而不是硬编码
    if (breakableBrick != null && animator != null)
    {
        animator.SetBool(breakableBrick.breakBoolName, false);
    }
}

/// <summary>
/// 复原动画播放完成事件
/// 由动画事件在复原动画最后一帧调用
/// </summary>
public void OnRespawnAnimationComplete()
{
    Debug.Log("复原动画播放完成");
    
    // 使用主脚本中的配置参数名，而不是硬编码
    if (animator != null)
    {
        animator.SetBool(breakableBrick.respawnBoolName, false);
    }
    
    // 通知主脚本复原动画完成
    if (breakableBrick != null)
    {
        breakableBrick.OnRespawnAnimationCompleted();
    }
}
```

### 修复 BreakableBrick.cs 中的参数访问
确保动画参数名可以被动画事件脚本访问：

```csharp
// 在 BreakableBrick.cs 中添加公共属性访问
public string breakBoolName { get { return breakBoolName; } }
public string respawnBoolName { get { return respawnBoolName; } }
```

## 总结
通过以上排查和修复步骤，应该能够解决动画执行两遍的问题。主要关键点是确保：
1. 动画参数名在所有脚本中保持一致
2. 脚本组件没有被重复挂载
3. 动画控制器和动画事件配置正确

如果问题仍然存在，可以使用提供的调试脚本进一步分析动画状态的变化过程。