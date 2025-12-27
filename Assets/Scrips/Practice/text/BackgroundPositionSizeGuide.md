# 背景图片位置和大小调整功能指南

## 功能概述

文本触发系统现在支持对背景图片进行**位置偏移**和**大小缩放**调整，让你可以更灵活地定制文本显示效果。

## 可用参数

### 1. 位置偏移 (backgroundPositionOffset)
- **类型**：Vector2
- **单位**：像素
- **默认值**：(0, 0) - 居中显示
- **作用**：调整背景图片相对于文本面板的位置
- **使用方法**：
  - X值：正值向右偏移，负值向左偏移
  - Y值：正值向上偏移，负值向下偏移

### 2. 大小缩放 (backgroundSizeScale)
- **类型**：Vector2
- **单位**：缩放比例（倍数）
- **默认值**：(1, 1) - 原始大小
- **作用**：调整背景图片的尺寸
- **使用方法**：
  - 大于1.0：放大图片
  - 小于1.0：缩小图片
  - 保持原始比例：确保X和Y值相同

## 配置方法

### 方法1：通过 TextData 配置

1. **创建 TextData**：右键点击 Project 窗口 → `Create > Game Data > Text Data`

2. **配置背景参数**：
   - 勾选 `useBackgroundImage`
   - 选择 `backgroundImage` 资源
   - 设置 `backgroundPositionOffset`：
     - 例如：(0, 15f) → 向上偏移15像素
   - 设置 `backgroundSizeScale`：
     - 例如：(1.2f, 1.2f) → 放大20%

### 方法2：通过代码配置

```csharp
// 加载背景图片
Sprite talkBubble = Resources.Load<Sprite>("TextBackgrounds/TalkBubble");

// 设置位置和大小参数
Vector2 positionOffset = new Vector2(50f, -20f); // 向右下方偏移
Vector2 sizeScale = new Vector2(0.9f, 1.1f); // 宽度缩小10%，高度放大10%

// 显示带自定义背景的文本
TextDisplayManager.Instance.DisplayText(
    "带自定义位置和大小的背景", 
    3f, 
    Color.black, 
    new Color(0, 0, 0, 0.3f), 
    talkBubble, 
    true, 
    positionOffset, 
    sizeScale
);
```

## 应用场景

### 场景1：对话气泡定位

当使用对话气泡作为背景时，你可能希望气泡指向说话角色的方向：

```csharp
// 气泡指向右侧角色
Vector2 rightBubbleOffset = new Vector2(30f, 0f);
TextDisplayManager.Instance.DisplayText(
    "你好，我在右边！", 2f, Color.black, Color.clear, 
    bubbleSprite, true, rightBubbleOffset, Vector2.one
);

// 气泡指向左侧角色
Vector2 leftBubbleOffset = new Vector2(-30f, 0f);
TextDisplayManager.Instance.DisplayText(
    "你好，我在左边！", 2f, Color.black, Color.clear, 
    bubbleSprite, true, leftBubbleOffset, Vector2.one
);
```

### 场景2：不同屏幕尺寸适配

针对不同的游戏屏幕尺寸，可以调整背景大小：

```csharp
// 在大屏幕上放大背景
float screenScale = Screen.width / 1920f; // 基于1920宽度的缩放比例
Vector2 scale = new Vector2(screenScale, screenScale);

TextDisplayManager.Instance.DisplayText(
    "自适应屏幕尺寸的文本", 3f, Color.black, Color.clear, 
    backgroundSprite, true, Vector2.zero, scale
);
```

### 场景3：强调重要信息

使用更大的背景和偏移位置来突出重要信息：

```csharp
// 放大并居中显示重要提示
Vector2 emphasisSize = new Vector2(1.5f, 1.5f);

TextDisplayManager.Instance.DisplayText(
    "⚠️ 前方有危险！", 3f, Color.red, new Color(0, 0, 0, 0.8f), 
    warningSprite, true, Vector2.zero, emphasisSize
);
```

## 测试方法

### 测试脚本

在 `TextBackgroundTest.cs` 脚本中添加以下测试方法：

```csharp
[ContextMenu("Test Different Positions")]
public void TestDifferentPositions()
{
    // 测试不同位置的背景
    Sprite bubble = Resources.Load<Sprite>("TextBackgrounds/TalkBubble");
    
    // 右侧
    TextDisplayManager.Instance.DisplayText(
        "右侧气泡", 2f, Color.black, Color.clear, 
        bubble, true, new Vector2(40f, 0f), Vector2.one
    );
    
    // 左侧
    TextDisplayManager.Instance.DisplayText(
        "左侧气泡", 2f, Color.black, Color.clear, 
        bubble, true, new Vector2(-40f, 0f), Vector2.one
    );
    
    // 上方
    TextDisplayManager.Instance.DisplayText(
        "上方气泡", 2f, Color.black, Color.clear, 
        bubble, true, new Vector2(0f, 30f), Vector2.one
    );
    
    // 下方
    TextDisplayManager.Instance.DisplayText(
        "下方气泡", 2f, Color.black, Color.clear, 
        bubble, true, new Vector2(0f, -30f), Vector2.one
    );
}

[ContextMenu("Test Different Sizes")]
public void TestDifferentSizes()
{
    // 测试不同大小的背景
    Sprite bubble = Resources.Load<Sprite>("TextBackgrounds/TalkBubble");
    
    // 小尺寸
    TextDisplayManager.Instance.DisplayText(
        "小气泡", 2f, Color.black, Color.clear, 
        bubble, true, Vector2.zero, new Vector2(0.8f, 0.8f)
    );
    
    // 中等尺寸
    TextDisplayManager.Instance.DisplayText(
        "中气泡", 2f, Color.black, Color.clear, 
        bubble, true, Vector2.zero, new Vector2(1.0f, 1.0f)
    );
    
    // 大尺寸
    TextDisplayManager.Instance.DisplayText(
        "大气泡", 2f, Color.black, Color.clear, 
        bubble, true, Vector2.zero, new Vector2(1.5f, 1.5f)
    );
}
```

### 测试步骤

1. 将测试脚本添加到场景中的任意对象
2. 在 Inspector 中右键点击脚本组件
3. 选择测试方法：
   - `Test Background Position and Size`：测试位置和大小组合
   - `Test Different Positions`：测试不同位置
   - `Test Different Sizes`：测试不同大小

## 最佳实践

1. **保持一致性**：为同一类型的文本（如对话、提示、警告）使用统一的背景样式

2. **避免过度偏移**：
   - 偏移值建议在 -50 到 50 像素之间
   - 过大的偏移可能导致背景图片超出屏幕

3. **保持比例**：
   - 缩放时建议保持 X 和 Y 值相同，避免图片变形
   - 如需要不同比例，确保变形效果符合设计需求

4. **测试不同屏幕**：
   - 在不同分辨率下测试背景显示效果
   - 确保在小屏幕设备上背景图片不会被截断

5. **结合淡入淡出**：
   - 位置和大小调整会与淡入淡出动画协同工作
   - 保持动画流畅，避免突兀的位置变化

## 常见问题

### Q: 背景图片位置为什么没有变化？
A: 检查以下几点：
- 是否设置了正确的 `backgroundPositionOffset` 参数
- 是否在调用 `DisplayText` 时传递了位置参数
- TextData 中的 `useBackgroundImage` 是否勾选

### Q: 背景图片为什么变形了？
A: 可能原因：
- `backgroundSizeScale` 的 X 和 Y 值不同
- 原始图片本身是变形的
- 尝试使用相同的 X 和 Y 缩放值来保持比例

### Q: 如何重置背景图片的位置和大小？
A: 将参数设置为默认值：
- 位置：`Vector2.zero`
- 大小：`Vector2.one`

## 总结

背景图片的位置和大小调整功能为文本触发系统提供了更高的灵活性，让你可以创建更具个性化和视觉吸引力的文本显示效果。通过合理配置这些参数，可以提升游戏的叙事效果和玩家体验。