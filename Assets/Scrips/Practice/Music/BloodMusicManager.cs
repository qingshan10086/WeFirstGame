using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// 血液游戏音乐管理器
/// 负责管理游戏中的所有音乐和音效
/// </summary>
public class BloodMusicManager : MonoBehaviour
{
    // Singleton instance
    public static BloodMusicManager Instance { get; private set; }

    [Header("音频源设置")]
    public AudioSource backgroundMusicSource;  // 背景音乐源
    public AudioSource[] soundEffectSources;   // 音效源数组（对象池）
    public int maxSoundEffectSources = 5;      // 最大音效源数量

    [Header("音频混合器设置")]
    public AudioMixer audioMixer;              // 音频混合器
    public string masterVolumeParameter = "MasterVolume"; // 主音量参数名
    public string musicVolumeParameter = "MusicVolume";   // 音乐音量参数名
    public string sfxVolumeParameter = "SFXVolume";       // 音效音量参数名

    [Header("默认音量设置")]
    [Range(0f, 1f)] public float defaultMasterVolume = 1f;
    [Range(0f, 1f)] public float defaultMusicVolume = 0.7f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    [Header("淡入淡出设置")]
    public float fadeDuration = 1f;  // 默认淡入淡出持续时间
    public Player player;
    // 事件系统
    public static event Action OnBackgroundMusicStarted;
    public static event Action OnBackgroundMusicStopped;
    public static event Action OnBackgroundMusicPaused;
    public static event Action OnVolumeChanged;
    public static event Action<AudioClip> OnSoundEffectPlayed;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
          
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSystem()
    {
        // 初始化音效源对象池
        if (soundEffectSources == null || soundEffectSources.Length == 0)
        {
            soundEffectSources = new AudioSource[maxSoundEffectSources];
            for (int i = 0; i < maxSoundEffectSources; i++)
            {
                GameObject sourceGO = new GameObject("SFXSource_" + i);
                sourceGO.transform.parent = transform;
                soundEffectSources[i] = sourceGO.AddComponent<AudioSource>();
                soundEffectSources[i].spatialBlend = 0f; // 2D音效
                soundEffectSources[i].playOnAwake = false;
            }
        }

        // 设置默认音量
        SetMasterVolume(defaultMasterVolume);
        SetMusicVolume(defaultMusicVolume);
        SetSFXVolume(defaultSFXVolume);

        // 如果背景音乐源存在，初始化它
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.loop = true;
            backgroundMusicSource.volume = defaultMusicVolume;
        }

        Debug.Log("BloodMusicManager initialized successfully");
    }

    // ========== 背景音乐控制功能 ==========

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBackgroundMusic(AudioClip musicClip, bool fadeIn = false)
    {
        Debug.Log($"BloodMusicManager: PlayBackgroundMusic called with clip={musicClip?.name}, fadeIn={fadeIn}");
        
        if (backgroundMusicSource == null)
        {
            Debug.LogError("BloodMusicManager: Background music source is null!");
            return;
        }
        
        if (musicClip == null)
        {
            Debug.LogError("BloodMusicManager: Music clip is null!");
            return;
        }

        if (currentFadeCoroutine != null)
        {
            Debug.Log("BloodMusicManager: Stopping current fade coroutine.");
            StopCoroutine(currentFadeCoroutine);
        }

        ResetBackgroundMusicSpeed(); // 切换音乐时重置播放速度

        if (fadeIn)
        {
            Debug.Log($"BloodMusicManager: Starting fade coroutine for music: {musicClip.name}");
            currentFadeCoroutine = StartCoroutine(FadeBackgroundMusic(musicClip, fadeDuration));
        }
        else
        {
            Debug.Log($"BloodMusicManager: Playing music directly: {musicClip.name}");
            backgroundMusicSource.clip = musicClip;
            backgroundMusicSource.volume = defaultMusicVolume;
            backgroundMusicSource.loop = true;  // 确保循环播放
            backgroundMusicSource.Play();
            Debug.Log($"BloodMusicManager: Music started playing: {musicClip.name}");
            OnBackgroundMusicStarted?.Invoke();
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBackgroundMusic(bool fadeOut = false)
    {
        if (backgroundMusicSource == null || !backgroundMusicSource.isPlaying)
        {
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        if (fadeOut)
        {
            currentFadeCoroutine = StartCoroutine(FadeOutBackgroundMusic(fadeDuration));
        }
        else
        {
            backgroundMusicSource.Stop();
            OnBackgroundMusicStopped?.Invoke();
        }
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
            OnBackgroundMusicPaused?.Invoke();
        }
    }

    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeBackgroundMusic()
    {
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
            OnBackgroundMusicStarted?.Invoke();
        }
    }

    /// <summary>
    /// 切换背景音乐
    /// </summary>
    public void SwitchBackgroundMusic(AudioClip newMusicClip, float fadeDuration = 1f)
    {
        if (newMusicClip == null)
        {
            Debug.LogWarning("BloodMusicManager: New music clip is null");
            return;
        }

        ResetBackgroundMusicSpeed(); // 切换音乐时重置播放速度

        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            currentFadeCoroutine = StartCoroutine(SwitchMusicCoroutine(newMusicClip, fadeDuration));
        }
        else
        {
            PlayBackgroundMusic(newMusicClip, true);
        }
    }

    // ========== 音效控制功能 ==========

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySoundEffect(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("BloodMusicManager: Sound effect clip is null");
            return;
        }

        // 从对象池中获取可用的音效源
        AudioSource source = GetAvailableSoundEffectSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            OnSoundEffectPlayed?.Invoke(clip);
        }
    }

    /// <summary>
    /// 播放音效（通过名称）
    /// </summary>
    public void PlaySoundEffect(string clipName, float volume = 1f, float pitch = 1f)
    {
        if (audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            PlaySoundEffect(clip, volume, pitch);
        }
        else
        {
            Debug.LogWarning("BloodMusicManager: Sound effect clip not found: " + clipName);
        }
    }

    /// <summary>
    /// 获取可用的音效源
    /// </summary>
    private AudioSource GetAvailableSoundEffectSource()
    {
        foreach (AudioSource source in soundEffectSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // 如果没有可用的源，返回第一个正在播放的源（会被覆盖）
        Debug.LogWarning("BloodMusicManager: All sound effect sources are in use, reusing the first one");
        return soundEffectSources[0];
    }

    // ========== 音量控制功能 ==========

    /// <summary>
    /// 设置主音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        SetVolume(masterVolumeParameter, volume);
        OnVolumeChanged?.Invoke();
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        SetVolume(musicVolumeParameter, volume);
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume;
        }
        OnVolumeChanged?.Invoke();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        SetVolume(sfxVolumeParameter, volume);
        foreach (AudioSource source in soundEffectSources)
        {
            source.volume = volume;
        }
        OnVolumeChanged?.Invoke();
    }

    /// <summary>
    /// 设置音量（内部方法，将线性音量转换为dB）
    /// </summary>
    private void SetVolume(string parameterName, float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("BloodMusicManager: Audio mixer is null");
            return;
        }

        // 将0-1的线性音量转换为-80dB到0dB的对数音量
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(parameterName, dB);
    }

    // ========== 淡入淡出效果 ==========

    /// <summary>
    /// 背景音乐淡入
    /// </summary>
    private IEnumerator FadeBackgroundMusic(AudioClip newClip, float duration)
    {
        Debug.Log($"BloodMusicManager: FadeBackgroundMusic coroutine started for {newClip.name}, duration={duration}");
        
        float elapsedTime = 0f;
        float startVolume = backgroundMusicSource.volume;

        // 如果当前正在播放音乐，先淡出
        if (backgroundMusicSource.isPlaying)
        {
            Debug.Log($"BloodMusicManager: Currently playing {backgroundMusicSource.clip?.name}, starting fade out");
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
            
            backgroundMusicSource.Stop();
            Debug.Log("BloodMusicManager: Fade out complete, music stopped");
        }
        else
        {
            Debug.Log("BloodMusicManager: No music currently playing, skipping fade out");
        }

        // 更换音乐并淡入
        Debug.Log($"BloodMusicManager: Setting new music clip: {newClip.name}");
        backgroundMusicSource.clip = newClip;
        backgroundMusicSource.loop = true;  // 确保循环播放
        backgroundMusicSource.volume = 0f;
        backgroundMusicSource.Play();
        Debug.Log("BloodMusicManager: Music clip started playing (volume 0)");
        
        elapsedTime = 0f;

        Debug.Log("BloodMusicManager: Starting fade in");
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            backgroundMusicSource.volume = Mathf.Lerp(0f, defaultMusicVolume, t);
            yield return null;
        }

        backgroundMusicSource.volume = defaultMusicVolume;
        Debug.Log($"BloodMusicManager: Fade complete! Now playing {newClip.name} at volume {defaultMusicVolume}");
        OnBackgroundMusicStarted?.Invoke();
        currentFadeCoroutine = null;
    }

    /// <summary>
    /// 背景音乐淡出
    /// </summary>
    private IEnumerator FadeOutBackgroundMusic(float duration)
    {
        float elapsedTime = 0f;
        float startVolume = backgroundMusicSource.volume;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        backgroundMusicSource.Stop();
        backgroundMusicSource.volume = defaultMusicVolume; // 重置音量
        OnBackgroundMusicStopped?.Invoke();
        currentFadeCoroutine = null;
    }

    /// <summary>
    /// 切换背景音乐协程
    /// </summary>
    private IEnumerator SwitchMusicCoroutine(AudioClip newClip, float duration)
    {
        // 淡出当前音乐
        yield return StartCoroutine(FadeOutBackgroundMusic(duration));
        // 淡入新音乐
        yield return StartCoroutine(FadeBackgroundMusic(newClip, duration));
    }

    // ========== 音频剪辑管理 ==========

    /// <summary>
    /// 注册音频剪辑
    /// </summary>
    public void RegisterAudioClip(string name, AudioClip clip)
    {
        if (string.IsNullOrEmpty(name) || clip == null)
        {
            Debug.LogWarning("BloodMusicManager: Invalid audio clip registration");
            return;
        }

        if (audioClips.ContainsKey(name))
        {
            audioClips[name] = clip;
        }
        else
        {
            audioClips.Add(name, clip);
        }
    }

    /// <summary>
    /// 获取音频剪辑
    /// </summary>
    public AudioClip GetAudioClip(string name)
    {
        if (audioClips.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }
        Debug.LogWarning("BloodMusicManager: Audio clip not found: " + name);
        return null;
    }

    /// <summary>
    /// 预加载音频剪辑
    /// </summary>
    public void PreloadAudioClips(params AudioClip[] clips)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip != null)
            {
                RegisterAudioClip(clip.name, clip);
            }
        }
        Debug.Log("BloodMusicManager: Preloaded " + clips.Length + " audio clips");
    }

    // ========== 调试功能 ==========

    [ContextMenu("Play Test Sound")]
    private void PlayTestSound()
    {
        Debug.Log("BloodMusicManager: Test sound played");
    }

    [ContextMenu("Reset Volumes")]
    public void ResetVolumes()
    {
        SetMasterVolume(defaultMasterVolume);
        SetMusicVolume(defaultMusicVolume);
        SetSFXVolume(defaultSFXVolume);
        Debug.Log("BloodMusicManager: Volumes reset to default values");
    }

    // ========== 播放速度控制 ==========

    /// <summary>
    /// 获取当前背景音乐的播放速度（pitch）
    /// </summary>
    public float GetBackgroundMusicSpeed()
    {
        if (backgroundMusicSource != null)
        {
            return backgroundMusicSource.pitch;
        }
        return 1f; // 默认正常速度
    }

    /// <summary>
    /// 设置背景音乐的播放速度（pitch）
    /// </summary>
    /// <param name="speed">播放速度，默认1.0为正常速度，范围通常为0.5-2.0</param>
    public void SetBackgroundMusicSpeed(float speed)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.pitch = speed;
            Debug.Log("背景音乐播放速度已设置为: " + speed);
        }
    }

    /// <summary>
    /// 重置背景音乐播放速度为正常速度（1.0）
    /// </summary>
    public void ResetBackgroundMusicSpeed()
    {
        SetBackgroundMusicSpeed(1f);
    }

    /// <summary>
    /// 获取当前播放的背景音乐
    /// </summary>
    public AudioClip GetCurrentBackgroundMusic()
    {
        if (backgroundMusicSource != null)
        {
            return backgroundMusicSource.clip;
        }
        return null;
    }

    /// <summary>
    /// 检查背景音乐是否正在播放
    /// </summary>
    public bool IsBackgroundMusicPlaying()
    {
        if (backgroundMusicSource != null)
        {
            return backgroundMusicSource.isPlaying;
        }
        return false;
    }
}