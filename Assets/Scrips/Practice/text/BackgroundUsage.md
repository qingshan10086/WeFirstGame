# 文本触发系统背景处理说明

## 背景组件结构

在文本触发系统中，背景显示采用了**分层结构**，包含两个独立的Image组件：

### 1. 背景颜色 (backgroundImage)
- **位置**：直接挂载在 `textPanel` 对象上
- **用途**：用于显示纯色背景或半透明遮罩
- **获取方式**：`backgroundImage = textPanel.GetComponent<Image>()`
- **控制**：通过 `backgroundColor` 参数设置颜色和透明度

### 2. 背景图片 (backgroundSpriteImage)
- **位置**：挂载在 `textPanel` 的子对象 `BackgroundSprite` 上
- **用途**：用于显示自定义Sprite背景图片（如对话气泡）
- **获取方式**：动态创建或查找 `BackgroundSprite` 对象
- **控制**：
  - 通过 `backgroundSprite` 和 `useBackgroundSprite` 参数设置图片
  - 通过 `backgroundPositionOffset` 参数调整位置（像素单位）
  - 通过 `backgroundSizeScale` 参数调整大小（缩放比例）
- **默认状态**：
  - 位置：居中对齐（Offset: (0, 0)）
  - 大小：原始图片大小（Scale: (1, 1)）

## 背景优先级规则

系统采用以下规则处理背景显示：

1. **仅背景颜色**：当不使用背景图片时，显示 `backgroundImage` 的颜色
2. **仅背景图片**：当使用背景图片时，
   - 显示 `backgroundSpriteImage` 的图片
   - 自动将 `backgroundImage` 的透明度设置为 0（隐藏背景颜色）
   - 背景图片会保持原始颜色和透明度（通过白色基础色确保正确显示）

## 背景图片加载流程

1. **初始化**：在 `TextDisplayManager.Awake()` 中创建或获取 `BackgroundSprite` 对象
2. **配置**：在 `DisplayTextCoroutine` 中设置背景图片属性
3. **显示**：在 `FadeText` 协程中控制背景图片的透明度动画
4. **隐藏**：文本显示结束时重置背景图片状态

## 使用建议

- **纯色背景**：直接设置 `backgroundColor` 即可
- **图片背景**：准备好Sprite资源，通过代码直接设置或由TextDisplayManager统一管理
- **性能优化**：避免同时使用复杂背景图片和半透明背景颜色

## 常见问题排查

1. **背景图片不显示**：
   - 检查 `useBackgroundImage` 是否设置为 true
   - 确认 `backgroundImage` 资源是否有效
   - 检查 `BackgroundSprite` 对象是否存在

2. **背景颜色和图片冲突**：
   - 系统会自动处理冲突，使用图片时会隐藏颜色
   - 如需同时显示，可调整背景图片的透明度

3. **背景图片颜色异常**：
   - 系统使用白色作为图片的基础色（color = new Color(1,1,1,alpha)）
   - 确保你的Sprite资源本身颜色正确，没有被其他因素影响