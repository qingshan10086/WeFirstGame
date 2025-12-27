using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本显示管理器
/// 负责管理游戏中的所有文本显示和动画效果
/// </summary>
public class TextDisplayManager : MonoBehaviour
{
    // Singleton instance
    public static TextDisplayManager Instance { get; private set; }

    [Header("UI设置")]
    public Text textComponent;
    public GameObject textPanel;

    [Header("动画设置")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public float defaultDisplayTime = 999999f; // 设置非常长的默认显示时间，确保文本不会自动消失

    [Header("颜色设置")]
    public Color defaultTextColor = Color.white;
    public Color defaultBackgroundColor = new Color(0, 0, 0, 0.5f);

    [Header("背景设置")]
    public Image customBackgroundImage; // 用户手工配置的背景图片
    public Sprite backgroundSprite; // 背景图片
    public Vector2 backgroundPositionOffset; // 背景图片位置偏移
    public Vector2 backgroundSizeScale = Vector2.one; // 背景图片大小缩放
    public bool useBackgroundSprite = true; // 是否使用背景图片

    [Header("输入控制")]
    public bool disableInputDuringText = true;
    public KeyCode closeTextKey = KeyCode.Space;
    public KeyCode continueTextKey = KeyCode.E;

    // 内部变量
    private Queue<TextDisplayData> textQueue = new Queue<TextDisplayData>();
    private bool isDisplayingText = false;
    private Coroutine currentTextCoroutine;
    private Image backgroundImage;
    private Image backgroundSpriteImage;

    // 事件系统
    public static event System.Action OnTextStartedDisplaying;
    public static event System.Action OnTextFinishedDisplaying;
    public static event System.Action<string> OnTextDisplayed;

    private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTextSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTextSystem()
    {
        // 设置默认UI状态
        if (textComponent != null)
        {
            textComponent.color = new Color(defaultTextColor.r, defaultTextColor.g, defaultTextColor.b, 0f);
            textComponent.text = string.Empty;
            Debug.Log("TextDisplayManager: textComponent initialized");
        }

        if (textPanel != null)
        {
            Debug.Log("TextDisplayManager: textPanel found, initializing backgrounds");
            
            // 确保对话框面板的RectTransform是稳定的
            RectTransform panelRectTransform = textPanel.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                
                // 保存原始设置，完全保留用户在UI中的所有设置
                
                // **完全保持原始尺寸，不进行任何缩放**
                // 为了避免对话框在游戏中变得过大，我们不进行任何缩放计算
                // 保持用户在UI编辑器中设置的原始尺寸不变
                
                // **完全不修改任何用户设置 - 锚点、位置、重心、尺寸都保持原样**
                // 保持 anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta 完全不变
                
                // 不做任何修改，让对话框保持UI编辑器中的原始设置
  
            }
            
            // 背景颜色图片
            backgroundImage = textPanel.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(defaultBackgroundColor.r, defaultBackgroundColor.g, defaultBackgroundColor.b, 0f);
                Debug.Log("TextDisplayManager: backgroundImage initialized");
            }
            else
            {
                Debug.LogWarning("TextDisplayManager: backgroundImage not found on textPanel");
            }
            
            // 背景精灵图片 - 只使用用户配置的customBackgroundImage
            if (customBackgroundImage != null)
            {
                backgroundSpriteImage = customBackgroundImage;
                backgroundSpriteImage.color = new Color(1f, 1f, 1f, 0f); // 初始设置为透明
                
                // 配置背景图片完全跟随对话框面板大小
                RectTransform bgRectTransform = customBackgroundImage.GetComponent<RectTransform>();
                if (bgRectTransform != null)
                {
                    // 设置背景图片为对话框的子对象，跟随对话框缩放
                    if (customBackgroundImage.transform.parent != textPanel.transform)
                    {
                        customBackgroundImage.transform.SetParent(textPanel.transform, false);
                    }
                    
                    // 设置背景图片为stretch模式，完全跟随对话框大小
                    bgRectTransform.anchorMin = Vector2.zero;
                    bgRectTransform.anchorMax = Vector2.one;
                    bgRectTransform.offsetMin = Vector2.zero;
                    bgRectTransform.offsetMax = Vector2.zero;
                    bgRectTransform.anchoredPosition = Vector2.zero;
                    
                    Debug.Log("TextDisplayManager: customBackgroundImage configured to stretch with dialog panel");
                }
                
                Debug.Log("TextDisplayManager: Using user-configured customBackgroundImage");
            }
            else
            {
                Debug.Log("TextDisplayManager: No customBackgroundImage configured");
            }
            
            textPanel.SetActive(false);
            Debug.Log("TextDisplayManager: textPanel set to inactive");
        }
        else
        {
            Debug.LogError("TextDisplayManager: textPanel is null");
        }

        Debug.Log("TextDisplayManager initialized successfully");
        
        // 启动协程来持续监控面板和背景图片缩放
        StartCoroutine(MonitorPanelAndBackgroundScaling());
    }

    /// <summary>
    /// 显示文本
    /// </summary>
    public void DisplayText(string text, float displayTime = -1f, Color? textColor = null, Color? bgColor = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("TextDisplayManager: Attempting to display empty text");
            return;
        }

        float actualDisplayTime = displayTime > 0 ? displayTime : defaultDisplayTime;
        Color actualTextColor = textColor ?? defaultTextColor;
        Color actualBgColor = bgColor ?? defaultBackgroundColor;

        TextDisplayData data = new TextDisplayData(text, actualDisplayTime, actualTextColor, actualBgColor);
        textQueue.Enqueue(data);

        OnTextDisplayed?.Invoke(text);

        // 如果当前没有显示文本，立即开始显示
        if (!isDisplayingText)
        {
            StartDisplayingText();
        }
    }

    /// <summary>
    /// 立即显示文本（清除队列）
    /// </summary>
    public void DisplayTextImmediately(string text, float displayTime = -1f, Color? textColor = null, Color? bgColor = null)
    {
        // 清除当前队列
        textQueue.Clear();

        // 停止当前显示的文本
        if (currentTextCoroutine != null)
        {
            StopCoroutine(currentTextCoroutine);
            isDisplayingText = false;
        }

        // 立即显示新文本
        DisplayText(text, displayTime, textColor, bgColor);
    }

    /// <summary>
    /// 开始显示队列中的文本
    /// </summary>
    private void StartDisplayingText()
    {
        if (textQueue.Count == 0)
        {
            return;
        }

        isDisplayingText = true;
        TextDisplayData data = textQueue.Dequeue();
        currentTextCoroutine = StartCoroutine(DisplayTextCoroutine(data));
    }

    /// <summary>
    /// 文本显示协程
    /// </summary>
    private IEnumerator DisplayTextCoroutine(TextDisplayData data)
    {
        Debug.Log($"TextDisplayManager: Displaying text: {data.text}");
        
        // 确保UI组件存在
        if (textComponent == null || textPanel == null)
        {
            Debug.LogError("TextDisplayManager: UI components not assigned!");
            isDisplayingText = false;
            StartDisplayingText(); // 尝试显示下一个文本
            yield break;
        }

        // 启用文本面板
        textPanel.SetActive(true);

        // 设置文本内容
        textComponent.text = data.text;

        // 检查并设置背景图片
        if (backgroundSpriteImage != null)
        {
            if (useBackgroundSprite && backgroundSprite != null)
            {
                backgroundSpriteImage.sprite = backgroundSprite;
                backgroundSpriteImage.type = Image.Type.Sliced;
                backgroundSpriteImage.preserveAspect = false;
                backgroundSpriteImage.fillMethod = Image.FillMethod.Horizontal;
                backgroundSpriteImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                backgroundSpriteImage.fillClockwise = true;
                backgroundSpriteImage.fillAmount = 1f;
                backgroundSpriteImage.enabled = true;
                
                // 确保背景图片填充整个父容器
                RectTransform bgRectTransform = backgroundSpriteImage.GetComponent<RectTransform>();
                if (bgRectTransform != null)
                {
                    // 确保背景图片完全跟随对话框面板大小
                     if (backgroundSpriteImage.transform.parent != textPanel.transform)
                     {
                         backgroundSpriteImage.transform.SetParent(textPanel.transform, false);
                     }
                     
                     // 完全重置RectTransform并设置为严格跟随对话框大小
                     bgRectTransform.SetParent(textPanel.transform, true); // 保持世界位置
                     bgRectTransform.SetAsFirstSibling(); // 确保在最底层
                     
                     // 重置所有transform属性
                     bgRectTransform.anchorMin = Vector2.zero;
                     bgRectTransform.anchorMax = Vector2.one;
                     bgRectTransform.offsetMin = Vector2.zero;
                     bgRectTransform.offsetMax = Vector2.zero;
                     bgRectTransform.anchoredPosition = Vector2.zero;
                     bgRectTransform.pivot = new Vector2(0.5f, 0.5f);
                     bgRectTransform.localScale = Vector3.one;
                     bgRectTransform.localRotation = Quaternion.identity;
                     
                     // 调试信息：显示对话框和背景图片的大小
                     RectTransform panelRect = textPanel.GetComponent<RectTransform>();
                     if (panelRect != null)
                     {
                         Vector2 panelSize = panelRect.rect.size;
                         Vector2 bgSize = bgRectTransform.rect.size;
                         Debug.Log($"TextDisplayManager: Panel size: {panelSize}, Background size: {bgSize}, Ratio: {bgSize.x/panelSize.x:F2}");
                     }
                     
                     Debug.Log("TextDisplayManager: Background image configured to stretch with dialog panel");
                }
                
                // 如果使用背景图片，将背景颜色透明度设置为0
                if (backgroundImage != null)
                {
                    backgroundImage.color = new Color(data.backgroundColor.r, data.backgroundColor.g, data.backgroundColor.b, 0f);
                }
            }
            else
            {
                backgroundSpriteImage.sprite = null;
                backgroundSpriteImage.enabled = false;
            }
        }

        // 淡入效果
        yield return StartCoroutine(FadeText(true, data.textColor, data.backgroundColor));
        OnTextStartedDisplaying?.Invoke();
        
        // 确保文本层级在背景之上，防止被挡住
        if (textComponent != null)
        {
            textComponent.transform.SetAsLastSibling();
            Debug.Log("TextDisplayManager: Text set to topmost layer to avoid being blocked");
        }
        
        // 调试日志：显示当前文本和显示时间
        Debug.Log($"TextDisplayManager: Now displaying text: '{data.text}' for {data.displayTime} seconds");
        
        // 等待显示时间或用户输入
        float elapsedTime = 0f;
        bool userInteracted = false;
        
        while (elapsedTime < data.displayTime)
        {
            // 检查关闭文本键
            if (Input.GetKeyDown(closeTextKey))
            {
                userInteracted = true;
                Debug.Log("TextDisplayManager: Close key pressed");
                break;
            }
            
            // 检查继续文本键
            if (Input.GetKeyDown(continueTextKey))
            {
                userInteracted = true;
                Debug.Log("TextDisplayManager: Continue key pressed");
                break;
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 调试日志：退出等待循环的原因
        Debug.Log($"TextDisplayManager: Exiting wait loop. Time elapsed: {elapsedTime}, Display time: {data.displayTime}, User interacted: {userInteracted}");

        // 淡出效果
        yield return StartCoroutine(FadeText(false, data.textColor, data.backgroundColor));
        OnTextFinishedDisplaying?.Invoke();

        // 重置状态
        textComponent.text = string.Empty;
        textPanel.SetActive(false);
        isDisplayingText = false;

        // 显示队列中的下一个文本
        StartDisplayingText();
    }

    /// <summary>
    /// 文本淡入淡出效果
    /// </summary>
    private IEnumerator FadeText(bool fadeIn, Color textColor, Color backgroundColor)
    {
        float duration = fadeIn ? fadeInDuration : fadeOutDuration;
        float elapsedTime = 0f;

        Color startTextColor = fadeIn ? new Color(textColor.r, textColor.g, textColor.b, 0f) : textComponent.color;
        Color endTextColor = fadeIn ? textColor : new Color(textColor.r, textColor.g, textColor.b, 0f);

        Color startBgColor = fadeIn ? new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0f) : backgroundImage.color;
        Color endBgColor = fadeIn ? backgroundColor : new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0f);

        Color startSpriteColor = Color.clear;
        Color endSpriteColor = Color.clear;
        bool hasBackgroundSprite = backgroundSpriteImage != null && backgroundSpriteImage.sprite != null;

        if (hasBackgroundSprite)
        {
            // 使用白色作为背景图片的颜色基础，只调整透明度，这样可以正确显示图片原始颜色
            startSpriteColor = fadeIn ? new Color(1f, 1f, 1f, 0f) : backgroundSpriteImage.color;
            endSpriteColor = fadeIn ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 使用平滑的缓动函数
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 更新文本颜色
            textComponent.color = Color.Lerp(startTextColor, endTextColor, smoothT);

            // 更新背景颜色
            if (backgroundImage != null)
            {
                backgroundImage.color = Color.Lerp(startBgColor, endBgColor, smoothT);
            }

            // 更新背景图片透明度
            if (hasBackgroundSprite)
            {
                backgroundSpriteImage.color = Color.Lerp(startSpriteColor, endSpriteColor, smoothT);
            }

            yield return null;
        }

        // 确保最终颜色正确
        textComponent.color = endTextColor;
        
        if (backgroundImage != null)
        {
            backgroundImage.color = endBgColor;
        }
        
        if (hasBackgroundSprite)
        {
            backgroundSpriteImage.color = endSpriteColor;
        }
    }

    /// <summary>
    /// 清除当前文本队列
    /// </summary>
    public void ClearTextQueue()
    {
        textQueue.Clear();
        Debug.Log("TextDisplayManager: Text queue cleared");
    }

    /// <summary>
    /// 检查是否正在显示文本
    /// </summary>
    public bool IsDisplayingText()
    {
        return isDisplayingText;
    }

    /// <summary>
    /// 获取当前文本队列长度
    /// </summary>
    public int GetTextQueueLength()
    {
        return textQueue.Count;
    }

    // 内部数据结构
    private class TextDisplayData
    {
        public string text;
        public float displayTime;
        public Color textColor;
        public Color backgroundColor;

        public TextDisplayData(string text, float displayTime, Color textColor, Color backgroundColor)
        {
            this.text = text;
            this.displayTime = displayTime;
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
        }
    }

    // 持续监控对话框面板和背景图片缩放的协程
    private System.Collections.IEnumerator MonitorPanelAndBackgroundScaling()
    {
        // 等待初始化完成
        yield return new WaitForSeconds(0.1f);
        
        // 保存原始尺寸和屏幕尺寸用于比较
        Vector2 originalPanelSize = Vector2.zero;
        Vector2 originalBackgroundSize = Vector2.zero;
        Vector2 lastScreenSize = new Vector2(Screen.width, Screen.height);
        
        while (true)
        {
            try
            {
                if (textPanel != null)
                {
                    RectTransform panelRect = textPanel.GetComponent<RectTransform>();
                    if (panelRect != null)
                    {
                        // 首次获取原始尺寸
                        if (originalPanelSize == Vector2.zero)
                        {
                            originalPanelSize = panelRect.rect.size;
                            Debug.Log($"TextDisplayManager: 记录原始对话框尺寸: {originalPanelSize}");
                        }
                        
                        Vector2 currentPanelSize = panelRect.rect.size;
                        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
                        
                        // 检测屏幕尺寸变化（只监控，不修改）
                        if (Mathf.Abs(currentScreenSize.x - lastScreenSize.x) > 10f || 
                            Mathf.Abs(currentScreenSize.y - lastScreenSize.y) > 10f)
                        {
                            Debug.Log($"检测到屏幕尺寸变化: {lastScreenSize} -> {currentScreenSize}");
                            lastScreenSize = currentScreenSize;
                            
                            Debug.Log($"对话框保持原尺寸: {currentPanelSize}, 位置保持不变={panelRect.anchoredPosition}");
                        }
                        
                        // 检查对话框大小是否异常（只检查，不修改）
                        if (currentScreenSize.x > 0 && currentScreenSize.y > 0)
                        {
                            // 只记录当前状态，不做任何修改
                            Debug.Log($"对话框当前状态: 尺寸={currentPanelSize}, 屏幕={currentScreenSize}, 位置={panelRect.anchoredPosition}");
                        }
                        
                        // 监控背景图片
                        if (backgroundSpriteImage != null && backgroundSpriteImage.isActiveAndEnabled)
                        {
                            RectTransform bgRect = backgroundSpriteImage.GetComponent<RectTransform>();
                            if (bgRect != null)
                            {
                                // 首次获取原始尺寸
                                if (originalBackgroundSize == Vector2.zero)
                                {
                                    originalBackgroundSize = bgRect.rect.size;
                                }
                                
                                Vector2 bgSize = bgRect.rect.size;
                                Vector2 panelSize = currentPanelSize;
                                
                                // 如果背景图片不匹配对话框大小，重新设置
                                if (Mathf.Abs(bgSize.x - panelSize.x) > 5f || Mathf.Abs(bgSize.y - panelSize.y) > 5f)
                                {
                                    Debug.Log($"修正背景图片大小: Panel={panelSize}, Background={bgSize}");
                                    
                                    // 背景图片使用stretch模式跟随对话框
                                    bgRect.anchorMin = Vector2.zero;
                                    bgRect.anchorMax = Vector2.one;
                                    bgRect.offsetMin = Vector2.zero;
                                    bgRect.offsetMax = Vector2.zero;
                                    bgRect.anchoredPosition = Vector2.zero;
                                    bgRect.pivot = new Vector2(0.5f, 0.5f);
                                    bgRect.localScale = Vector3.one;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"MonitorPanelAndBackgroundScaling error: {e.Message}");
            }
            
            yield return new WaitForSeconds(0.3f); // 每0.3秒检查一次，降低频率
        }
    }

    // 调试功能
    [ContextMenu("Test Display Text")]
    private void TestDisplayText()
    {
        DisplayText("这是一个测试文本！", 2f);
    }
}
