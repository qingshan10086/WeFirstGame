using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本系统调试工具
/// 用于测试和调试文本触发系统的功能
/// </summary>
public class TextDebugger : MonoBehaviour
{
    [Header("调试UI设置")]
    public Text debugText;
    public Button testDisplayTextButton;
    public Button testTriggerZoneButton;
    public Button clearTextQueueButton;
    public InputField testTextInput;
    public Slider displayTimeSlider;
    public Toggle useFadeEffectsToggle;

    [Header("测试文本设置")]
    public string testText = "这是一个调试测试文本！";
    public float testDisplayTime = 3f;

    [Header("调试选项")]
    public bool showDebugInfo = true;
    public bool logToConsole = true;

    private void Start()
    {
        // 设置UI事件
        SetupUIEvents();

        // 检查系统设置
        CheckSystemSetup();

        // 显示初始调试信息
        UpdateDebugInfo("文本调试器已初始化");
    }

    private void SetupUIEvents()
    {
        if (testDisplayTextButton != null)
        {
            testDisplayTextButton.onClick.AddListener(() => TestDisplayText());
        }

        if (testTriggerZoneButton != null)
        {
            testTriggerZoneButton.onClick.AddListener(() => TestTriggerZone());
        }

        if (clearTextQueueButton != null)
        {
            clearTextQueueButton.onClick.AddListener(() => ClearTextQueue());
        }

        if (displayTimeSlider != null)
        {
            displayTimeSlider.minValue = 0.5f;
            displayTimeSlider.maxValue = 10f;
            displayTimeSlider.value = testDisplayTime;
            displayTimeSlider.onValueChanged.AddListener((value) => testDisplayTime = value);
        }

        if (testTextInput != null)
        {
            testTextInput.text = testText;
            testTextInput.onValueChanged.AddListener((value) => testText = value);
        }
    }

    private void CheckSystemSetup()
    {
        string debugInfo = "系统设置检查：";

        // 检查TextDisplayManager
        if (TextDisplayManager.Instance != null)
        {
            debugInfo += "\n✓ TextDisplayManager.Instance 已创建";

            // 检查UI组件
            if (TextDisplayManager.Instance.GetComponentInChildren<Text>() != null)
            {
                debugInfo += "\n✓ Text组件已找到";
            }
            else
            {
                debugInfo += "\n✗ Text组件未找到";
            }

            if (TextDisplayManager.Instance.GetComponentInChildren<Image>() != null)
            {
                debugInfo += "\n✓ Image组件已找到";
            }
            else
            {
                debugInfo += "\n✗ Image组件未找到";
            }
        }
        else
        {
            debugInfo += "\n✗ TextDisplayManager.Instance 未创建";
        }

        UpdateDebugInfo(debugInfo);
    }

    /// <summary>
    /// 测试显示文本
    /// </summary>
    public void TestDisplayText()
    {
        if (TextDisplayManager.Instance == null)
        {
            UpdateDebugInfo("错误：TextDisplayManager.Instance 未找到");
            return;
        }

        UpdateDebugInfo("测试：显示文本");
        TextDisplayManager.Instance.DisplayText(testText, testDisplayTime);
    }

    /// <summary>
    /// 测试创建触发区域
    /// </summary>
    public void TestTriggerZone()
    {
        UpdateDebugInfo("测试：创建触发区域");

        // 创建一个临时的触发区域测试
        GameObject testZone = new GameObject("TestTextTriggerZone");
        TextTriggerZone triggerZone = testZone.AddComponent<TextTriggerZone>();
        triggerZone.displayText = "这是一个测试触发区域！";
        triggerZone.displayTime = 2f;
        triggerZone.triggerWidth = 3f;
        triggerZone.triggerHeight = 3f;

        UpdateDebugInfo($"已创建测试触发区域：{testZone.name}");
        Debug.Log($"创建了测试触发区域：{testZone.name}");
    }

    /// <summary>
    /// 清除文本队列
    /// </summary>
    public void ClearTextQueue()
    {
        if (TextDisplayManager.Instance != null)
        {
            TextDisplayManager.Instance.ClearTextQueue();
            UpdateDebugInfo("已清除文本队列");
        }
        else
        {
            UpdateDebugInfo("错误：无法清除文本队列 - TextDisplayManager未找到");
        }
    }

    /// <summary>
    /// 更新调试信息
    /// </summary>
    private void UpdateDebugInfo(string message)
    {
        if (showDebugInfo)
        {
            if (debugText != null)
            {
                debugText.text += $"\n{System.DateTime.Now:HH:mm:ss} - {message}";

                // 限制调试文本的行数
                string[] lines = debugText.text.Split('\n');
                if (lines.Length > 50)
                {
                    string[] newLines = new string[50];
                    System.Array.Copy(lines, lines.Length - 50, newLines, 0, 50);
                    debugText.text = string.Join("\n", newLines);
                }
            }
        }

        if (logToConsole)
        {
            Debug.Log(message);
        }
    }

    // 监听文本显示事件
    private void OnEnable()
    {
        TextDisplayManager.OnTextStartedDisplaying += HandleTextStartedDisplaying;
        TextDisplayManager.OnTextFinishedDisplaying += HandleTextFinishedDisplaying;
        TextDisplayManager.OnTextDisplayed += HandleTextDisplayed;
    }

    private void OnDisable()
    {
        TextDisplayManager.OnTextStartedDisplaying -= HandleTextStartedDisplaying;
        TextDisplayManager.OnTextFinishedDisplaying -= HandleTextFinishedDisplaying;
        TextDisplayManager.OnTextDisplayed -= HandleTextDisplayed;
    }

    private void HandleTextStartedDisplaying()
    {
        UpdateDebugInfo("文本开始显示");
    }

    private void HandleTextFinishedDisplaying()
    {
        UpdateDebugInfo("文本显示结束");
    }

    private void HandleTextDisplayed(string text)
    {
        UpdateDebugInfo($"文本已显示：{text}");
    }

    // 调试菜单选项
    [ContextMenu("显示系统状态")]
    private void ShowSystemStatus()
    {
        string status = "文本系统状态：";

        if (TextDisplayManager.Instance != null)
        {
            status += $"\nTextDisplayManager: 活跃";
            status += $"\n正在显示文本: {TextDisplayManager.Instance.IsDisplayingText()}";
            status += $"\n队列中文本数: {TextDisplayManager.Instance.GetTextQueueLength()}";
        }
        else
        {
            status += "\nTextDisplayManager: 未活跃";
        }

        UpdateDebugInfo(status);
    }

    [ContextMenu("清除调试日志")]
    private void ClearDebugLog()
    {
        if (debugText != null)
        {
            debugText.text = "调试日志已清除";
        }
    }
}