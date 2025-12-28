using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BloodMusicManager使用示例
/// 展示如何使用BloodMusicManager的各种功能
/// </summary>
public class BloodMusicManagerExample : MonoBehaviour
{
    public AudioClip backgroundMusic;      // 背景音乐剪辑
    public AudioClip backgroundMusic2;     // 另一个背景音乐剪辑
    public AudioClip soundEffect1;         // 音效剪辑1
    public AudioClip soundEffect2;         // 音效剪辑2

    private void Start()
    {
        // 确保BloodMusicManager已初始化
        if (BloodMusicManager.Instance != null)
        {
            Debug.Log("BloodMusicManagerExample: BloodMusicManager is ready");
            
            // 预加载音频剪辑
            PreloadAudioClips();
            
            // 注册音量变化事件
            BloodMusicManager.OnVolumeChanged += OnVolumeChanged;
            
            // 播放背景音乐（带淡入效果）
            PlayBackgroundMusicWithFade();
        }
        else
        {
            Debug.LogError("BloodMusicManagerExample: BloodMusicManager instance not found!");
        }
    }

    private void OnDestroy()
    {
        // 取消注册事件
        BloodMusicManager.OnVolumeChanged -= OnVolumeChanged;
    }

    /// <summary>
    /// 预加载音频剪辑
    /// </summary>
    private void PreloadAudioClips()
    {
        if (BloodMusicManager.Instance != null)
        {
            BloodMusicManager.Instance.PreloadAudioClips(
                backgroundMusic,
                backgroundMusic2,
                soundEffect1,
                soundEffect2
            );
        }
    }

    /// <summary>
    /// 带淡入效果的背景音乐播放
    /// </summary>
    public void PlayBackgroundMusicWithFade()
    {
        if (BloodMusicManager.Instance != null && backgroundMusic != null)
        {
            Debug.Log("BloodMusicManagerExample: Playing background music with fade");
            BloodMusicManager.Instance.PlayBackgroundMusic(backgroundMusic, true);
        }
    }

    /// <summary>
    /// 切换背景音乐
    /// </summary>
    public void SwitchBackgroundMusic()
    {
        if (BloodMusicManager.Instance != null && backgroundMusic2 != null)
        {
            Debug.Log("BloodMusicManagerExample: Switching background music");
            BloodMusicManager.Instance.SwitchBackgroundMusic(backgroundMusic2, 2f);
        }
    }

    /// <summary>
    /// 停止背景音乐（带淡出效果）
    /// </summary>
    public void StopBackgroundMusicWithFade()
    {
        if (BloodMusicManager.Instance != null)
        {
            Debug.Log("BloodMusicManagerExample: Stopping background music with fade");
            BloodMusicManager.Instance.StopBackgroundMusic(true);
        }
    }

    /// <summary>
    /// 播放音效1
    /// </summary>
    public void PlaySoundEffect1()
    {
        if (BloodMusicManager.Instance != null && soundEffect1 != null)
        {
            Debug.Log("BloodMusicManagerExample: Playing sound effect 1");
            BloodMusicManager.Instance.PlaySoundEffect(soundEffect1, 1f, 1f);
        }
    }

    /// <summary>
    /// 播放音效2
    /// </summary>
    public void PlaySoundEffect2()
    {
        if (BloodMusicManager.Instance != null && soundEffect2 != null)
        {
            Debug.Log("BloodMusicManagerExample: Playing sound effect 2");
            BloodMusicManager.Instance.PlaySoundEffect(soundEffect2, 0.7f, 1.1f);
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    public void SetVolumeLevels()
    {
        if (BloodMusicManager.Instance != null)
        {
            BloodMusicManager.Instance.SetMasterVolume(0.8f);
            BloodMusicManager.Instance.SetMusicVolume(0.6f);
            BloodMusicManager.Instance.SetSFXVolume(0.9f);
            Debug.Log("BloodMusicManagerExample: Volume levels set");
        }
    }

    /// <summary>
    /// 音量变化事件处理
    /// </summary>
    private void OnVolumeChanged()
    {
        Debug.Log("BloodMusicManagerExample: Volume changed");
    }

    /// <summary>
    /// 在场景视图中显示使用示例的GUI
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 300, 400));
        
        GUILayout.Label("BloodMusicManager Example");
        GUILayout.Space(10);

        if (GUILayout.Button("Play Background Music (Fade)") && backgroundMusic != null)
        {
            PlayBackgroundMusicWithFade();
        }

        if (GUILayout.Button("Switch Background Music") && backgroundMusic2 != null)
        {
            SwitchBackgroundMusic();
        }

        if (GUILayout.Button("Stop Background Music (Fade)") && backgroundMusic != null)
        {
            StopBackgroundMusicWithFade();
        }

        GUILayout.Space(20);
        GUILayout.Label("Sound Effects:");

        if (GUILayout.Button("Play Sound Effect 1") && soundEffect1 != null)
        {
            PlaySoundEffect1();
        }

        if (GUILayout.Button("Play Sound Effect 2") && soundEffect2 != null)
        {
            PlaySoundEffect2();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Set Volume Levels"))
        {
            SetVolumeLevels();
        }

        if (GUILayout.Button("Reset Volumes"))
        {
            if (BloodMusicManager.Instance != null)
            {
                BloodMusicManager.Instance.ResetVolumes();
            }
        }

        GUILayout.EndArea();
    }
}