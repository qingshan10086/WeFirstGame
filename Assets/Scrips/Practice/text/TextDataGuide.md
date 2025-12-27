# TextData 使用指南

## 什么是 TextData？

TextData 是一个 **ScriptableObject** 资源，用于存储和管理文本触发系统中的文本数据。它允许你在不修改代码的情况下，轻松配置文本内容、样式和显示参数。

## 优势

- **数据与逻辑分离**：文本内容与代码分开管理，便于修改和维护
- **复用性**：同一个 TextData 可以被多个触发区域使用
- **可视化编辑**：在 Inspector 中直接编辑文本属性，无需编码
- **版本控制友好**：ScriptableObject 作为资源文件，便于版本管理

## 创建 TextData

### 步骤 1：创建资源

1. 在 Project 窗口中，右键点击任意文件夹
2. 选择 `Create > Game Data > Text Data`
3. 为你的 TextData 命名（例如：`NPCDialogue1`）

### 步骤 2：配置属性

在 Inspector 中配置 TextData 的属性：

#### 基本文本设置
- **displayText**：要显示的文本内容
  - 支持单行文本显示
  - 使用换行符 `\n` 可以在文本中添加换行

#### 显示设置
- **displayTime**：文本显示的持续时间（秒）
  - 范围：0.5f - 10f
  - 设置为 0 或负数将使用系统默认时间

#### 颜色设置
- **textColor**：文本的颜色
- **backgroundColor**：背景颜色（当不使用背景图片时生效）
  - 可通过 Alpha 通道设置透明度



#### 音频设置
- **displaySoundEffect**：文本显示时播放的音效
- **useSoundEffect**：是否播放音效

#### 高级设置
- **textID**：文本的唯一标识符（用于代码识别）
- **textType**：文本类型（普通、对话、指示等）
  - 用于区分不同用途的文本

## 使用 TextData

### 方法 1：通过 TextTriggerZone 使用

1. 在场景中创建一个 TextTriggerZone 对象
2. 勾选 `useTextData` 选项
3. 将创建好的 TextData 资源拖拽到 `textData` 字段
4. 配置触发区域大小、触发条件等参数
5. 运行游戏，当玩家进入触发区域时，将显示 TextData 中的文本

### 方法 2：通过代码使用

在脚本中直接使用 TextData：

```csharp
using UnityEngine;
using Practice.Text;

public class ExampleScript : MonoBehaviour
{
    // 在 Inspector 中拖拽 TextData 资源
    public TextData myTextData;
    
    public void ShowText()
    {
        if (myTextData != null)
        {
            // 显示文本
            TextDisplayManager.Instance.DisplayText(
                myTextData.displayText,
                myTextData.displayTime,
                myTextData.textColor,
                myTextData.backgroundColor
            );
        }
    }
}
```

## 最佳实践

1. **组织资源**：创建专门的文件夹存放 TextData 资源（例如：`Assets/Resources/TextData/`）

2. **命名规范**：使用清晰的命名约定（例如：`NPC_Name_DialogueNumber`）

3. **复用文本**：对于重复使用的文本（如提示信息），创建一个 TextData 并在多个触发区域使用

4. **文本长度控制**：
   - 保持文本简洁，避免过长的文本影响阅读体验
   - 根据文本长度调整 `displayTime`，确保玩家有足够时间阅读

5. **测试显示时间**：根据文本长度调整 `displayTime`，确保玩家有足够时间阅读

## 常见问题

### Q：为什么我的文本不显示？
A：检查以下几点：
- TextData 中的 `displayText` 是否为空
- TextTriggerZone 是否正确配置了 TextData
- TextDisplayManager 是否正确初始化



### Q：如何在文本中添加换行？
A：在 `displayText` 中使用 `\n` 字符，例如：`"第一行\n第二行"`

## 示例配置

### 示例 1：NPC 对话

- **displayText**："欢迎来到我们的村庄！"
- **displayTime**：3.5f
- **textColor**：黄色
- **textType**：Dialogue

### 示例 2：游戏提示

- **displayText**："按空格键跳跃！"
- **displayTime**：2.0f
- **textColor**：白色
- **backgroundColor**：半透明黑色（0, 0, 0, 0.7）
- **textType**：Instruction

---

通过以上步骤，你可以轻松创建和使用 TextData 资源，为你的游戏添加丰富的文本内容！