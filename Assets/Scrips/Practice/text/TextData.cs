using UnityEngine;

/// <summary>
/// 文本数据ScriptableObject
/// 用于存储文本信息，方便在编辑器中管理和复用
/// </summary>
[CreateAssetMenu(fileName = "NewTextData", menuName = "Game Data/Text Data", order = 1)]
public class TextData : ScriptableObject
{
    [Header("基本文本设置")]
    [Tooltip("要显示的文本")]
    [TextArea(3, 10)]
    public string displayText = "这是一个文本数据对象！";
    
    [Tooltip("是否使用连续对话")]
    public bool useContinuousDialogue = false;
    
    [Tooltip("连续对话文本数组")]
    [TextArea(2, 5)]
    public string[] dialogueTextArray;
    
    [Header("显示设置")]
    [Tooltip("文本显示时间")]
    [Range(0.5f, 10f)]
    public float displayTime = 3f;
    
    [Header("颜色设置")]
    [Tooltip("文本颜色")]
    public Color textColor = Color.white;
    
    [Tooltip("背景颜色")]
    public Color backgroundColor = new Color(0, 0, 0, 0.5f);
    

    
    [Header("音频设置")]
    [Tooltip("显示文本时播放的音效")]
    public AudioClip displaySoundEffect;
    
    [Tooltip("音效音量")]
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;
    
    [Header("高级设置")]
    [Tooltip("文本ID，用于代码中识别")]
    public string textID = "default_text";
    
    [Tooltip("是否是重要文本（可以影响游戏逻辑）")]
    public bool isImportantText = false;
    
    [Tooltip("文本类型")]
    public TextType textType = TextType.Normal;
    
    /// <summary>
    /// 文本类型枚举
    /// </summary>
    public enum TextType
    {
        Normal,         // 普通文本
        Dialogue,       // 对话文本
        Instruction,    // 指示文本
        Warning,        // 警告文本
        Success,        // 成功文本
        Error,          // 错误文本
        Tutorial,       // 教程文本
        Lore            // 背景故事文本
    }
    
    /// <summary>
    /// 获取格式化的文本信息
    /// </summary>
    public string GetFormattedInfo()
    {
        string info = $"TextID: {textID}, Type: {textType}, DisplayTime: {displayTime}s";
        return info;
    }
    
    /// <summary>
    /// 验证文本数据是否有效
    /// </summary>
    public bool IsValid()
    {
        if (useContinuousDialogue)
        {
            // 连续对话模式下，检查dialogueTextArray是否有内容
            return !string.IsNullOrEmpty(textID) && 
                   dialogueTextArray != null && 
                   dialogueTextArray.Length > 0 && 
                   !string.IsNullOrEmpty(dialogueTextArray[0]);
        }
        else
        {
            // 普通模式下，检查displayText是否有内容
            return !string.IsNullOrEmpty(displayText) && !string.IsNullOrEmpty(textID);
        }
    }
}
