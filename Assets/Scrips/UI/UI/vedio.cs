using UnityEngine;
using UnityEngine.UI;

public class vedio : MonoBehaviour
{
    private UnityEngine.UI.Button button;
    private Text buttonText;
    private bool bool1=false;

    void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        buttonText = GetComponentInChildren<Text>();

        // 初始化按钮文本
        UpdateButtonText();

        // 绑定点击事件
        button.onClick.AddListener(changeBool);
    }

    void changeBool()
    {
        bool1 = !bool1;
        // 更新按钮文本
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        buttonText.text = bool1 ? "图像品质:流畅" : "图像品质:极佳";
    }
}
