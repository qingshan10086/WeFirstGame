using UnityEngine;
using System.Collections;



/// <summary>
/// Boss被击败文本显示器
/// 与TextDisplayManager集成，使用统一的文本显示系统
/// </summary>
public class BossDefeatedTextDisplay : MonoBehaviour
{

    
    [Header("文本设置")]
    [Tooltip("使用TextData ScriptableObject")]
    public bool useTextData = false;
    
    [Tooltip("文本数据对象")]
    public TextData textData;
    
    [Tooltip("要显示的胜利文本")]
    public string victoryText = "Boss被击败！";
    
    [Tooltip("连续对话文本数组")]
    public string[] dialogueTextArray;
    
    [Tooltip("文本显示时间")]
    public float displayTime = 3f;
    
    [Tooltip("文本颜色")]
    public Color textColor = Color.yellow;
    
    [Tooltip("背景颜色")]
    public Color backgroundColor = new Color(0.5f, 0f, 0f, 0.8f);
    
    [Tooltip("是否使用连续对话")]
    public bool useContinuousDialogue = false;
    
    [Header("触发设置")]
    [Tooltip("是否立即显示文本（清除队列）")]
    public bool displayImmediately = false;
    
    [Tooltip("延迟显示时间")]
    public float displayDelay = 1f;
    
    [Header("声音效果")]
    [Tooltip("胜利音效")]
    public AudioClip victorySound;
    
    [Tooltip("音效音量")]
    [Range(0f, 1f)]
    public float soundVolume = 0.8f;
    
    private AudioSource audioSource;
    private bool isDisplaying = false;
    
    private void Awake()
    {
        // 初始化音频源
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;
        
        // 订阅Boss战结束事件
        SubscribeToEvents();
        
        Debug.Log("BossDefeatedTextDisplay: 已初始化并订阅Boss战结束事件");
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        UnsubscribeFromEvents();
    }
    
    /// <summary>
    /// 订阅Boss被击败事件
    /// </summary>
    private void SubscribeToEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossDefeated.AddListener(OnBossDefeated);
            Debug.Log("BossDefeatedTextDisplay: 订阅Boss被击败事件");
        }
        else
        {
            Debug.LogError("BossDefeatedTextDisplay: EventManager.Instance not found!");
        }
    }
    
    /// <summary>
    /// 取消订阅Boss被击败事件
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossDefeated.RemoveListener(OnBossDefeated);
            Debug.Log("BossDefeatedTextDisplay: 取消订阅Boss被击败事件");
        }
    }
    
    /// <summary>
    /// Boss被击败时的处理
    /// </summary>
    private void OnBossDefeated()
    {
        if (!isDisplaying)
        {
Debug.Log("Boss被击败文本开始播放"); StartCoroutine(DisplayVictoryTextSequence());
        }
    }
    
    /// <summary>
    /// 显示胜利文本序列
    /// </summary>
    private IEnumerator DisplayVictoryTextSequence()
    {
        isDisplaying = true;
        
        Debug.Log("BossDefeatedTextDisplay: 开始显示胜利文本");
        
        // 等待延迟时间
        yield return new WaitForSeconds(displayDelay);
        
        // 播放胜利音效
        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound, soundVolume);
        }
        
        // 检查TextDisplayManager是否存在
        if (TextDisplayManager.Instance == null)
        {
            Debug.LogError("BossDefeatedTextDisplay: TextDisplayManager.Instance not found!");
            isDisplaying = false;
            yield break;
        }
        
        // 根据配置选择显示方式
        if (useTextData && textData != null)
        {
            // 使用TextData模式
            if (textData.useContinuousDialogue)
            {
                // TextData连续对话模式
                if (textData.dialogueTextArray == null || textData.dialogueTextArray.Length == 0)
                {
                    Debug.LogWarning("BossDefeatedTextDisplay: TextData dialogueTextArray is empty");
                    isDisplaying = false;
                    yield break;
                }
                
                if (displayImmediately)
                {
                    // 立即显示模式：显示第一条，其余排队
                    TextDisplayManager.Instance.DisplayTextImmediately(
                        textData.dialogueTextArray[0],
                        textData.displayTime > 0 ? textData.displayTime : displayTime,
                        textData.textColor,
                        textData.backgroundColor
                    );
                    
                    // 添加剩余文本到队列
                    for (int i = 1; i < textData.dialogueTextArray.Length; i++)
                    {
                        TextDisplayManager.Instance.DisplayText(
                            textData.dialogueTextArray[i],
                            textData.displayTime > 0 ? textData.displayTime : displayTime,
                            textData.textColor,
                            textData.backgroundColor
                        );
                    }
                }
                else
                {
                    // 普通排队模式：所有文本按顺序排队
                    foreach (string text in textData.dialogueTextArray)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            TextDisplayManager.Instance.DisplayText(
                                text,
                                textData.displayTime > 0 ? textData.displayTime : displayTime,
                                textData.textColor,
                                textData.backgroundColor
                            );
                        }
                    }
                }
                
                Debug.Log($"BossDefeatedTextDisplay: 显示TextData连续对话，共{textData.dialogueTextArray.Length}条消息");
            }
            else
            {
                // 使用TextData的单行文本显示
                if (string.IsNullOrEmpty(textData.displayText))
                {
                    Debug.LogWarning("BossDefeatedTextDisplay: TextData displayText is empty");
                    isDisplaying = false;
                    yield break;
                }
                
                // 显示文本
                if (displayImmediately)
                {
                    TextDisplayManager.Instance.DisplayTextImmediately(
                        textData.displayText,
                        textData.displayTime > 0 ? textData.displayTime : displayTime,
                        textData.textColor,
                        textData.backgroundColor
                    );
                }
                else
                {
                    TextDisplayManager.Instance.DisplayText(
                        textData.displayText,
                        textData.displayTime > 0 ? textData.displayTime : displayTime,
                        textData.textColor,
                        textData.backgroundColor
                    );
                }
                
                Debug.Log($"BossDefeatedTextDisplay: 显示TextData文本: {textData.displayText}");
            }
        }
        else if (useContinuousDialogue)
        {
            // 本地连续对话模式
            if (dialogueTextArray == null || dialogueTextArray.Length == 0)
            {
                Debug.LogWarning("BossDefeatedTextDisplay: dialogueTextArray is empty");
                isDisplaying = false;
                yield break;
            }
            
            if (displayImmediately)
            {
                // 立即显示模式：显示第一条，其余排队
                TextDisplayManager.Instance.DisplayTextImmediately(
                    dialogueTextArray[0],
                    displayTime,
                    textColor,
                    backgroundColor
                );
                
                // 添加剩余文本到队列
                for (int i = 1; i < dialogueTextArray.Length; i++)
                {
                    TextDisplayManager.Instance.DisplayText(
                        dialogueTextArray[i],
                        displayTime,
                        textColor,
                        backgroundColor
                    );
                }
            }
            else
            {
                // 普通排队模式：所有文本按顺序排队
                foreach (string text in dialogueTextArray)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        TextDisplayManager.Instance.DisplayText(
                            text,
                            displayTime,
                            textColor,
                            backgroundColor
                        );
                    }
                }
            }
            
            Debug.Log($"BossDefeatedTextDisplay: 显示本地连续对话，共{dialogueTextArray.Length}条消息");
        }
        else
        {
            // 直接文本显示
            if (string.IsNullOrEmpty(victoryText))
            {
                Debug.LogWarning("BossDefeatedTextDisplay: victoryText is empty");
                isDisplaying = false;
                yield break;
            }
            
            // 显示文本
            if (displayImmediately)
            {
                TextDisplayManager.Instance.DisplayTextImmediately(
                    victoryText, 
                    displayTime, 
                    textColor, 
                    backgroundColor
                );
            }
            else
            {
                TextDisplayManager.Instance.DisplayText(
                    victoryText, 
                    displayTime, 
                    textColor, 
                    backgroundColor
                );
            }
            
            Debug.Log($"BossDefeatedTextDisplay: 显示胜利文本: {victoryText}");
        }
        
        isDisplaying = false;
        Debug.Log("BossDefeatedTextDisplay: 胜利文本显示完成");
    }
    

    

    
    /// <summary>
    /// 强制显示胜利文本（用于调试）
    /// </summary>
    /// <param name="customText">自定义文本</param>
    public void ForceDisplayVictoryText(string customText)
    {
        if (!isDisplaying)
        {
            string originalText = victoryText;
            victoryText = customText;
            StartCoroutine(DisplayVictoryTextSequence());
            victoryText = originalText; // 恢复原始值
        }
    }
    

    
    /// <summary>
    /// 手动触发胜利文本显示（用于测试）
    /// </summary>
    [ContextMenu("测试显示胜利文本")]
    public void TestDisplayVictoryText()
    {
        if (!isDisplaying)
        {
            StartCoroutine(DisplayVictoryTextSequence());
        }
    }
    
    /// <summary>
    /// 设置胜利文本内容
    /// </summary>
    /// <param name="text">新的胜利文本</param>
    public void SetVictoryText(string text)
    {
        victoryText = text;
        Debug.Log($"胜利文本已更新为: {victoryText}");
    }
    
    /// <summary>
    /// 设置胜利文本颜色
    /// </summary>
    /// <param name="color">新的文本颜色</param>
    public void SetVictoryTextColor(Color color)
    {
        textColor = color;
        Debug.Log($"胜利文本颜色已更新为: {textColor}");
    }
    
    /// <summary>
    /// 设置胜利文本背景颜色
    /// </summary>
    /// <param name="color">新的背景颜色</param>
    public void SetVictoryBackgroundColor(Color color)
    {
        backgroundColor = color;
        Debug.Log($"胜利文本背景颜色已更新为: {backgroundColor}");
    }
    
    /// <summary>
    /// 设置是否使用连续对话
    /// </summary>
    /// <param name="useContinuous">是否使用连续对话</param>
    public void SetUseContinuousDialogue(bool useContinuous)
    {
        useContinuousDialogue = useContinuous;
        Debug.Log($"是否使用连续对话: {useContinuousDialogue}");
    }
    
    /// <summary>
    /// 设置连续对话文本数组
    /// </summary>
    /// <param name="dialogueArray">对话文本数组</param>
    public void SetDialogueTextArray(string[] dialogueArray)
    {
        dialogueTextArray = dialogueArray;
        useContinuousDialogue = dialogueTextArray != null && dialogueTextArray.Length > 0;
        Debug.Log($"连续对话文本数组已设置，共{dialogueTextArray?.Length ?? 0}条");
    }
}