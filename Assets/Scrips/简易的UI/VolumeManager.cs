using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;

    [Header("Volume Control")]
    public Slider ZongvolumeSlider; // Slider 用于调节总音量
    private void Awake()
    {
        // 确保 VolumeManager 是单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化音量（你可以设置一个默认值，或从保存中加载）
        if (ZongvolumeSlider != null)
        {
            ZongvolumeSlider.value = AudioListener.volume;  // 初始时将 Slider 值与当前音量匹配
            ZongvolumeSlider.onValueChanged.AddListener(OnVolumeChanged); // 添加监听事件
        }

    }

    // 当音量发生变化时调用
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;  // 更新全局音量
    }

   
}

