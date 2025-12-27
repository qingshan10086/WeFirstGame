# BossDefeatedTextDisplay 使用说明

## 📋 功能概述

BossDefeatedTextDisplay是一个专门用于在Boss被击败后显示胜利文本的组件。该组件与TextDisplayManager完全集成，支持单条文本、多条连续对话、TextData ScriptableObject配置等多种显示模式。

## 🚀 核心特性

- ✅ **完全集成TextDisplayManager**：使用统一的文本显示系统
- ✅ **TextData ScriptableObject支持**：支持通过ScriptableObject管理胜利文本
- ✅ **连续对话功能**：可显示多条连续的胜利文本
- ✅ **事件驱动架构**：自动响应Boss战结束事件
- ✅ **灵活样式配置**：自定义文本颜色、背景颜色等
- ✅ **音效支持**：胜利音效和音量控制
- ✅ **手动测试功能**：支持在编辑器中手动触发测试

## 📁 文件位置

```
Assets/Scrips/Practice/RedMistBoss/BossDefeatedTextDisplay.cs
```

## 🛠️ 基础设置

### 1. 组件挂载

1. 在Unity编辑器中，选择需要显示胜利文本的场景对象（通常是Canvas或UI Manager）
2. 添加 `BossDefeatedTextDisplay` 组件
3. 组件会自动创建并初始化

### 2. 基本配置

在Inspector面板中配置以下基础参数：

```csharp
[Header("基础设置")]
public bool enableVictoryText = true;           // 启用胜利文本显示
public string victoryText = "Boss被击败！";     // 胜利文本内容
public float victoryTextDuration = 3f;          // 文本显示持续时间
public float victoryTextDelay = 1f;             // 延迟显示时间
```

## 🎨 高级配置

### 样式设置

```csharp
[Header("样式设置")]
public bool useCustomStyle = true;              // 使用自定义样式
public Color victoryTextColor = Color.yellow;   // 胜利文本颜色
public bool useVictoryBackground = true;        // 使用胜利背景
public Color victoryBackgroundColor = Color.red;// 胜利背景颜色
```

### 音效设置

```csharp
[Header("音效设置")]
public AudioClip victorySound;                  // 胜利音效
[Range(0f, 1f)]
public float soundVolume = 0.8f;                // 音效音量
```

## 📝 TextData ScriptableObject 配置

### 创建TextData资产

1. 在Assets中右键 → Create → TextData
2. 设置TextData的参数：
   - `displayTime`: 文本显示时间
   - `textColor`: 文本颜色
   - `backgroundColor`: 背景颜色

### 连续对话配置

1. 在TextData中启用 `useContinuousDialogue`
2. 在 `dialogueTextArray` 中添加多条胜利文本：
   ```csharp
   // 例如：
   dialogueTextArray = new string[]
   {
       "Boss已经被击败！",
       "胜利属于勇敢的冒险者！",
       "感谢你的勇气和坚持！"
   };
   ```

### 在组件中引用

将创建好的TextData拖拽到BossDefeatedTextDisplay的 `victoryTextData` 字段中。

## 🔧 使用方式

### 1. 自动触发（推荐）

组件会自动订阅 `EventManager.OnBossBattleEnd` 事件，Boss被击败时会自动显示胜利文本：

```csharp
// 无需额外代码，事件会自动触发
private void SubscribeToEvents()
{
    if (EventManager.Instance != null)
    {
        EventManager.Instance.OnBossBattleEnd.AddListener(OnBossDefeated);
    }
}
```

### 2. 手动触发

在编辑器中，可以右键点击组件选择 "Test Victory Text Display" 来手动测试：

```csharp
[ContextMenu("Test Victory Text Display")]
private void TestVictoryTextDisplay()
{
    Debug.Log("手动测试Boss被击败文本显示");
    StartCoroutine(DisplayVictoryTextSequence());
}
```

### 3. 脚本调用

也可以通过代码手动触发：

```csharp
// 获取组件引用
BossDefeatedTextDisplay textDisplay = FindObjectOfType<BossDefeatedTextDisplay>();

// 手动触发胜利文本显示
textDisplay.ForceDisplayVictoryText("自定义胜利文本", 5f);
```

## 📊 配置优先级

系统按照以下优先级处理文本显示：

1. **TextData配置模式**（最高优先级）
   - 如果 `victoryTextData` 已配置且 `useTextData` 为true
   - 支持连续对话和自定义样式

2. **本地连续对话模式**
   - 如果 `useContinuousDialogue` 为true且 `victoryDialogueArray` 有内容
   - 按顺序显示数组中的文本

3. **单个胜利文本模式**（默认）
   - 使用 `victoryText` 字段的单条文本

## 🎯 显示模式详解

### 单条胜利文本模式

```csharp
// 最简单的配置
public string victoryText = "Boss被击败！";
public float victoryTextDuration = 3f;
```

### 连续对话模式

```csharp
// 多条胜利文本，按顺序显示
public string[] victoryDialogueArray = new string[]
{
    "第一句话：Boss被击败！",
    "第二句话：胜利属于你！",
    "第三句话：继续前进吧！"
};
```

### TextData模式

```csharp
// 使用ScriptableObject管理复杂的胜利文本配置
public TextData victoryTextData;  // 拖拽TextData资产到此字段
public bool useTextData = true;   // 启用TextData模式
```

## 🔊 音效配置

### 胜利音效设置

1. 准备胜利音效文件（.wav、.mp3等格式）
2. 拖拽到 `victorySound` 字段
3. 调整 `soundVolume`（0.0-1.0）控制音量

### 音效播放时机

音效会在以下时机播放：
- 在显示胜利文本之前播放
- 只在成功显示文本时播放音效

## 🧪 测试和调试

### 1. 编辑器测试

- 右键点击组件 → "Test Victory Text Display"
- 观察控制台输出和文本显示效果

### 2. 控制台调试

组件提供详细的调试信息：

```csharp
Debug.Log("开始显示Boss被击败胜利文本");
Debug.Log("Boss被击败胜利文本显示完成");
Debug.LogWarning("检测到对话框大小突变: 原始={originalPanelSize}, 当前={currentPanelSize}");
```

### 3. 状态监控

- `isDisplaying`: 当前是否正在显示文本
- `textQueue`: 文本队列状态
- 组件会自动处理重复触发

## ⚙️ 高级功能

### 自定义事件处理

如果需要自定义事件处理，可以重写以下方法：

```csharp
protected override void OnBossDefeated()
{
    base.OnBossDefeated();
    
    // 添加自定义逻辑
    Debug.Log("执行自定义Boss被击败处理");
    
    // 调用父类方法显示文本
    StartCoroutine(DisplayVictoryTextSequence());
}
```

### 扩展显示内容

可以扩展 `DisplayVictoryTextSequence` 方法来添加更多内容：

```csharp
private IEnumerator DisplayVictoryTextSequence()
{
    // 显示胜利文本
    yield return base.DisplayVictoryTextSequence();
    
    // 添加其他效果（如粒子特效、震动等）
    if (victoryEffect != null)
    {
        victoryEffect.Play();
    }
    
    // 延迟一段时间
    yield return new WaitForSeconds(2f);
    
    // 触发下一个事件
    EventManager.Instance.TriggerNextLevel();
}
```

## 📝 注意事项

1. **依赖关系**：
   - 需要 `EventManager` 实例来处理Boss战事件
   - 需要 `TextDisplayManager` 实例来显示文本

2. **性能考虑**：
   - 组件使用协程处理动画，避免阻塞主线程
   - 自动处理重复触发，避免文本队列溢出

3. **兼容性**：
   - 与现有的文本显示系统完全兼容
   - 支持所有TextData的功能特性

4. **调试建议**：
   - 在控制台查看详细的调试信息
   - 使用编辑器测试功能验证配置
   - 检查TextData资产的有效性

## 🔧 故障排除

### 常见问题

**Q: 文本不显示？**
A: 检查TextDisplayManager是否存在且正确初始化

**Q: 音效不播放？**
A: 确认AudioSource组件存在且victorySound已配置

**Q: 事件不触发？**
A: 检查EventManager实例是否存在且正确初始化

**Q: TextData不生效？**
A: 确认useTextData为true且TextData资产有效

### 调试步骤

1. 检查控制台是否有错误信息
2. 确认所有依赖组件都已正确初始化
3. 使用编辑器测试功能验证基本配置
4. 检查TextData资产的有效性

## 📚 相关文件

- `TextDisplayManager.cs` - 核心文本显示管理器
- `TextData.cs` - TextData ScriptableObject定义
- `EventManager.cs` - 事件管理系统
- `BossBattleUI.cs` - Boss战UI管理器

---

通过以上配置，BossDefeatedTextDisplay可以为您提供专业、灵活、易用的Boss击败文本显示功能。如有其他问题，请参考相关文件或查看控制台调试信息。