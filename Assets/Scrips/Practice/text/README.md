# 文本触发系统使用说明

## 系统概述

文本触发系统是一个用于Unity 2D平台游戏的文本显示和触发系统，允许在玩家进入特定区域时显示自定义文本内容。该系统采用单例模式设计，支持文本淡入淡出动画、显示时间控制、文本队列管理以及通过ScriptableObject进行文本数据管理。

## 核心组件

### 1. TextDisplayManager (文本显示管理器)

**功能**：单例管理器，负责处理所有文本的显示、动画和队列管理。

**使用方法**：
1. 在场景中创建一个空对象，命名为"TextDisplayManager"
2. 添加`TextDisplayManager`组件
3. 配置UI组件：
   - `textComponent`：用于显示文本的Text组件
   - `textPanel`：包含背景的面板GameObject
4. 设置动画参数：
   - `fadeInDuration`：文本淡入时间
   - `fadeOutDuration`：文本淡出时间
   - `defaultDisplayTime`：默认显示时间

**背景图片支持**：
- 文本面板(panel)可以同时支持背景颜色和背景图片
- 背景图片支持位置偏移和大小缩放调整
- 背景图片和颜色可以同时使用，通过透明度叠加效果
- 系统会自动创建BackgroundSprite子对象用于显示背景图片

**UI设置注意事项**：
- 确保Text组件位于面板的顶层，以便正常显示文本
- 背景图片会自动创建并放置在面板的最底层
- 背景配置由TextDisplayManager统一管理
- 运行时会自动应用背景位置和大小的配置

**代码访问**：
```csharp
// 显示文本
TextDisplayManager.Instance.DisplayText("要显示的文本", 3f, Color.white, new Color(0, 0, 0, 0.5f));

// 立即显示文本（清除队列）
TextDisplayManager.Instance.DisplayTextImmediately("紧急文本", 2f);
```

### 2. TextTriggerZone (文本触发区域)

**功能**：2D触发区域，当玩家进入该区域时自动显示自定义文本内容。

**使用方法**：
1. 在场景中创建一个空对象，命名为"TextTriggerZone"
2. 添加`TextTriggerZone`组件
3. 配置触发区域大小：
   - `triggerWidth`：触发区域宽度
   - `triggerHeight`：触发区域高度
4. 设置文本内容：
   - 方法1：直接输入文本
     - 设置`displayText`、`displayTime`、`textColor`、`backgroundColor`等参数
   - 方法2：使用TextData
     - 勾选`useTextData`并分配TextData对象
5. 配置触发条件：
   - `triggerOnce`：是否只触发一次
   - `resetOnExit`：玩家离开后是否重置触发状态
   - `displayImmediately`：是否立即显示（清除队列）
6. 设置玩家识别：
   - `playerTag`：玩家对象的标签
   - 或使用`specificPlayer`指定特定玩家对象

**可视化**：
- 在编辑器中会显示黄色的线框表示触发区域
- 线框上方会显示文本预览

### 3. TextData (文本数据)

**功能**：ScriptableObject用于存储文本数据，支持复用和统一管理。

**创建方法**：
1. 在Project窗口中右键点击
2. 选择`Create > Game Data > Text Data`
3. 命名并配置文本数据：
   - `displayText`：要显示的文本内容（仅支持单行）
   - `displayTime`：显示时间
   - `textColor`：文本颜色
   - `backgroundColor`：背景颜色
   - `displaySoundEffect`：显示时播放的音效
   - `textID`：用于代码识别的ID
   - `textType`：文本类型（普通、对话、指示等）

**使用场景**：
- 存储游戏中的对话文本
- 管理教程提示文本
- 保存角色的台词或旁白

### 4. TextDebugger (文本调试器)

**功能**：用于测试和调试文本触发系统的工具。

**使用方法**：
1. 在场景中创建一个空对象，命名为"TextDebugger"
2. 添加`TextDebugger`组件
3. 配置调试UI（可选）
4. 在运行时使用按钮测试功能：
   - `TestDisplayTextButton`：测试显示文本
   - `TestTriggerZoneButton`：测试创建触发区域
   - `ClearTextQueueButton`：清除文本队列

**调试菜单**：
- 在Inspector中右键点击组件，选择"显示系统状态"查看当前系统状态
- 选择"清除调试日志"清空调试信息

## 设置流程

### 1. 基础设置

1. **创建TextDisplayManager**：
   - 在场景中创建空对象并添加TextDisplayManager组件
   - 配置UI组件和动画参数

2. **创建UI界面**：
   - 创建Canvas
   - 在Canvas下创建Panel作为文本面板
   - 在Panel下添加Text组件用于显示文本
   - 将Panel和Text组件分配给TextDisplayManager

### 2. 创建文本触发区域

1. **添加TextTriggerZone**：
   - 在场景中需要显示文本的位置创建空对象
   - 添加TextTriggerZone组件
   - 设置触发区域大小和位置

2. **配置文本内容**：
   - 方法1：直接在组件中输入文本和显示参数
   - 方法2：创建TextData对象并分配给组件

3. **设置触发条件**：
   - 选择是否只触发一次
   - 设置玩家离开后是否重置
   - 配置是否立即显示文本

### 3. 测试和调试

1. **运行测试**：
   - 进入Play模式
   - 让玩家角色进入触发区域
   - 检查文本是否正确显示

2. **使用调试器**：
   - 添加TextDebugger组件
   - 使用调试UI测试文本显示
   - 查看系统状态和日志

## 示例场景

### 示例1：基础文本触发

**目标**：在玩家进入特定区域时显示简单提示文本。

**设置**：
1. 创建TextTriggerZone对象
2. 设置触发区域大小为(5, 5)
3. 设置`displayText`为"这里是安全区域！"
4. 设置`displayTime`为3秒
5. 确保`playerTag`设置为"Player"

### 示例2：使用TextData的对话系统

**目标**：在玩家接近NPC时显示对话文本。

**设置**：
1. 创建TextData对象，设置：
   - `displayText`为"欢迎来到我们的村庄！"
   - `textColor`为黄色
   - `displayTime`为4秒
   - `textType`为Dialogue

2. 创建TextTriggerZone对象
3. 勾选`useTextData`并分配创建的TextData
4. 设置触发区域大小为(3, 3)
5. 将触发区域放置在NPC周围

### 示例3：文本队列管理

**目标**：按顺序显示多个文本内容。

**设置**：
1. 创建多个TextData对象，每个包含不同的文本内容
2. 创建多个TextTriggerZone对象，按顺序放置在场景中
3. 为每个触发区域分配对应的TextData
4. 确保所有触发区域的`displayImmediately`设置为false，使文本按队列顺序显示
5. 设置合理的`displayTime`，确保每个文本有足够的显示时间

**应用场景**：
- 教程关卡中的分步指引
- 剧情推进中的连续对话
- 探索游戏世界时的线索提示



## 代码示例

### 使用代码显示文本

```csharp
// 在脚本中显示文本
public void ShowHint()
{
    // 显示简单文本
    TextDisplayManager.Instance.DisplayText("收集所有金币以解锁下一关！", 3f);
    
    // 显示带颜色的文本
    TextDisplayManager.Instance.DisplayText("警告：前方有危险！", 2f, Color.red, new Color(0, 0, 0, 0.8f));
    
    // 立即显示重要文本
    TextDisplayManager.Instance.DisplayTextImmediately("游戏结束", 5f);
}
```

### 监听文本显示事件

```csharp
private void OnEnable()
{
    // 监听文本显示事件
    TextDisplayManager.OnTextStartedDisplaying += HandleTextStarted;
    TextDisplayManager.OnTextFinishedDisplaying += HandleTextFinished;
    TextDisplayManager.OnTextDisplayed += HandleTextDisplayed;
}

private void OnDisable()
{
    // 取消监听
    TextDisplayManager.OnTextStartedDisplaying -= HandleTextStarted;
    TextDisplayManager.OnTextFinishedDisplaying -= HandleTextFinished;
    TextDisplayManager.OnTextDisplayed -= HandleTextDisplayed;
}

private void HandleTextStarted()
{
    Debug.Log("文本开始显示");
}

private void HandleTextFinished()
{
    Debug.Log("文本显示结束");
}

private void HandleTextDisplayed(string text)
{
    Debug.Log($"显示的文本：{text}");
}
```

## 常见问题与解决方案

### 问题1：文本不显示

**可能原因**：
- TextDisplayManager未正确配置UI组件
- TextTriggerZone的触发区域太小
- 玩家对象的标签与设置的`playerTag`不匹配
- TextData中的文本内容为空

**解决方案**：
- 检查TextDisplayManager的`textComponent`和`textPanel`是否正确分配
- 调整TextTriggerZone的`triggerWidth`和`triggerHeight`
- 确保玩家对象的标签与`playerTag`一致
- 检查TextData的`displayText`是否有内容

### 问题2：文本显示后立即消失

**可能原因**：
- 显示时间设置过短
- 淡入淡出时间超过显示时间

**解决方案**：
- 增加`displayTime`的值
- 确保`fadeInDuration + fadeOutDuration`小于`displayTime`

### 问题3：多个文本同时显示

**可能原因**：
- `displayImmediately`设置为true
- 触发条件未正确设置

**解决方案**：
- 将`displayImmediately`设置为false以使用文本队列
- 检查`triggerOnce`和`resetOnExit`的设置

### 问题4：触发区域不显示

**可能原因**：
- 对象未选中
- 触发区域大小设置为0

**解决方案**：
- 在编辑器中选中TextTriggerZone对象
- 确保`triggerWidth`和`triggerHeight`大于0

## 性能优化

1. **避免频繁创建文本**：使用TextData复用文本内容
2. **控制文本显示频率**：合理设置`triggerOnce`和`resetOnExit`
3. **优化动画**：避免使用过长的淡入淡出时间
4. **限制文本长度**：过长的文本会影响渲染性能

## 扩展功能

### 自定义动画效果

可以通过修改`TextDisplayManager`中的`FadeText`协程来添加自定义的文本动画效果，如缩放、旋转或颜色变化。

### 音频集成

在`TextData`中可以配置音效，在`TextTriggerZone`的`TriggerTextDisplay`方法中添加音频播放代码：

```csharp
if (textData.displaySoundEffect != null)
{
    AudioSource.PlayClipAtPoint(textData.displaySoundEffect, Camera.main.transform.position, textData.soundEffectVolume);
}
```

### 文本类型扩展

可以在`TextData`的`TextType`枚举中添加更多文本类型，如：

```csharp
public enum TextType
{
    Normal,         // 普通文本
    Dialogue,       // 对话文本
    Instruction,    // 指示文本
    Warning,        // 警告文本
    Success,        // 成功文本
    Error,          // 错误文本
    Tutorial,       // 教程文本
    Lore,           // 背景故事文本
    Quest,          // 任务文本
    Achievement     // 成就文本
}
```

## 总结

文本触发系统提供了一个灵活、易用的方式来在Unity 2D游戏中显示触发式文本内容。通过合理使用TextDisplayManager、TextTriggerZone和TextData组件，可以创建丰富的游戏叙事、教程提示和玩家反馈。系统支持多种配置选项和扩展方式，可以根据游戏需求进行定制。