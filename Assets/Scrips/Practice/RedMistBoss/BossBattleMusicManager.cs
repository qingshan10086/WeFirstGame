using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Boss战音乐管理器
/// 负责在Boss战开始/结束时切换音乐
/// </summary>
public class BossBattleMusicManager : MonoBehaviour
{
    [Header("音频设置")]
    public AudioSource normalMusicSource;  // 普通音乐源
    public AudioSource bossMusicSource;    // Boss战音乐源
    public AudioMixerGroup musicMixerGroup; // 可选：音频混合器组
    
    [Header("过渡设置")]
    public float fadeDuration = 2f;        // 淡入淡出持续时间
    public float normalMusicVolume = 0.7f; // 普通音乐音量
    public float bossMusicVolume = 1.0f;   // Boss战音乐音量
    
    [Header("订阅设置")]
    public bool subscribeOnStart = true;   // 是否在Start时订阅
    public bool unsubscribeOnDestroy = true; // 是否在销毁时取消订阅

    private Coroutine currentFadeCoroutine; // 当前淡入淡出协程

    private void Start()
    {
        // 初始化音乐源状态
        InitializeMusicSources();
        
        if (subscribeOnStart)
        {
            SubscribeToBossBattleEvents();
        }
    }

    private void OnDestroy()
    {
        if (unsubscribeOnDestroy)
        {
            UnsubscribeFromBossBattleEvents();
        }
    }

    // 初始化音乐源
    private void InitializeMusicSources()
    {
        if (normalMusicSource != null)
        {
            normalMusicSource.volume = normalMusicVolume;
            normalMusicSource.loop = true;
            normalMusicSource.mute = false;
        }
        
        if (bossMusicSource != null)
        {
            bossMusicSource.volume = 0f;
            bossMusicSource.loop = true;
            bossMusicSource.mute = false;
        }
        
        Debug.Log("BossBattleMusicManager initialized");
    }

    // 订阅Boss战事件
    public void SubscribeToBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.AddListener(OnBossBattleStarted);
            EventManager.Instance.OnBossBattleEnd.AddListener(OnBossBattleEnded);
            Debug.Log("BossBattleMusicManager subscribed to events");
        }
        else
        {
            Debug.LogError("EventManager instance not found!");
        }
    }

    // 取消订阅Boss战事件
    public void UnsubscribeFromBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.RemoveListener(OnBossBattleStarted);
            EventManager.Instance.OnBossBattleEnd.RemoveListener(OnBossBattleEnded);
            Debug.Log("BossBattleMusicManager unsubscribed from events");
        }
    }

    // Boss战开始时切换到Boss战音乐
    private void OnBossBattleStarted()
    {
        Debug.Log("Boss battle music started");
        
        // 停止当前的淡入淡出协程
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        
        // 开始淡入Boss战音乐，淡出普通音乐
        currentFadeCoroutine = StartCoroutine(FadeToBossMusic());
    }

    // Boss战结束时切换回普通音乐
    private void OnBossBattleEnded()
    {
        Debug.Log("Boss battle music ended, switching back to normal music");
        
        // 停止当前的淡入淡出协程
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        
        // 开始淡入普通音乐，淡出Boss战音乐
        currentFadeCoroutine = StartCoroutine(FadeToNormalMusic());
    }

    // 淡入Boss战音乐，淡出普通音乐
    private IEnumerator FadeToBossMusic()
    {
        float elapsedTime = 0f;
        
        // 记录初始音量
        float startNormalVolume = normalMusicSource != null ? normalMusicSource.volume : 0f;
        float startBossVolume = bossMusicSource != null ? bossMusicSource.volume : 0f;
        
        // 如果Boss战音乐还没播放，开始播放
        if (bossMusicSource != null && !bossMusicSource.isPlaying)
        {
            bossMusicSource.Play();
        }
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            
            // 平滑过渡音量
            if (normalMusicSource != null)
            {
                normalMusicSource.volume = Mathf.Lerp(startNormalVolume, 0f, t);
            }
            
            if (bossMusicSource != null)
            {
                bossMusicSource.volume = Mathf.Lerp(startBossVolume, bossMusicVolume, t);
            }
            
            yield return null;
        }
        
        // 确保音量设置正确
        if (normalMusicSource != null)
        {
            normalMusicSource.volume = 0f;
            normalMusicSource.Pause();
        }
        
        if (bossMusicSource != null)
        {
            bossMusicSource.volume = bossMusicVolume;
        }
        
        currentFadeCoroutine = null;
    }

    // 淡入普通音乐，淡出Boss战音乐
    private IEnumerator FadeToNormalMusic()
    {
        float elapsedTime = 0f;
        
        // 记录初始音量
        float startNormalVolume = normalMusicSource != null ? normalMusicSource.volume : 0f;
        float startBossVolume = bossMusicSource != null ? bossMusicSource.volume : 0f;
        
        // 如果普通音乐还没播放，开始播放
        if (normalMusicSource != null && !normalMusicSource.isPlaying)
        {
            normalMusicSource.Play();
        }
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            
            // 平滑过渡音量
            if (normalMusicSource != null)
            {
                normalMusicSource.volume = Mathf.Lerp(startNormalVolume, normalMusicVolume, t);
            }
            
            if (bossMusicSource != null)
            {
                bossMusicSource.volume = Mathf.Lerp(startBossVolume, 0f, t);
            }
            
            yield return null;
        }
        
        // 确保音量设置正确
        if (normalMusicSource != null)
        {
            normalMusicSource.volume = normalMusicVolume;
        }
        
        if (bossMusicSource != null)
        {
            bossMusicSource.volume = 0f;
            bossMusicSource.Pause();
        }
        
        currentFadeCoroutine = null;
    }
}
