using UnityEngine;
using UnityEngine.UI;
public class fullScreen : MonoBehaviour
{
    private UnityEngine.UI.Button button;
    private Text buttonText;

    void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        buttonText = GetComponentInChildren<Text>();

        // 初始化按钮文本
        UpdateButtonText();

        // 绑定点击事件
        button.onClick.AddListener(ToggleFullscreen);
    }

    void ToggleFullscreen()
    {
        // 切换全屏状态
        Screen.fullScreen = !Screen.fullScreen;

        // 更新按钮文本
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        buttonText.text = Screen.fullScreen ? "窗口模式" : "全屏模式";
    }
}