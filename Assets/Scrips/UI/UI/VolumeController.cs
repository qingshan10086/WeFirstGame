using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VolumeController : MonoBehaviour
{
    [Header("组件引用")]
    public VideoPlayer videoPlayer;      // 视频播放器
    public Slider volumeSlider;          // 音量滑块

    private AudioSource videoAudioSource; // 视频的音频源

    void Start()
    {
        // 获取视频的AudioSource组件
        if (videoPlayer != null)
        {
            videoAudioSource = videoPlayer.GetComponent<AudioSource>();

            if (videoAudioSource == null)
            {
                Debug.LogError("未找到Video Player关联的AudioSource!");
                return;
            }
        }

        // 初始化Slider值
        if (volumeSlider != null)
        {
            volumeSlider.value = videoAudioSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVideoVolume);
        }
    }

    // 设置视频音量
    public void SetVideoVolume(float volume)
    {
        if (videoAudioSource != null)
        {
            videoAudioSource.volume = volume;
        }
    }

    // 静音/取消静音功能
    public void ToggleMute()
    {
        if (videoAudioSource != null)
        {
            videoAudioSource.mute = !videoAudioSource.mute;

            // 更新Slider显示（静音时设置为0，但保留实际值）
            if (volumeSlider != null && videoAudioSource.mute)
            {
                volumeSlider.value = 0;
            }
        }
    }
}