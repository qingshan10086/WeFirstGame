using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Boss战音乐管理器
/// 负责在Boss战开始/结束时切换音乐
/// 基于BloodMusicManager实现
/// </summary>
public class BossBattleMusicManager : MonoBehaviour
{
    [Header("Boss战音乐")]
    public AudioClip normalMusic;            // 普通音乐剪辑
    public AudioClip bossMusicPhase1;        // Boss战第一阶段音乐剪辑
    public AudioClip bossMusicPhase2;        // Boss战第二阶段音乐剪辑
    public AudioClip victorySound;          // 胜利音效剪辑
    
    [Header("过渡设置")]
    public float fadeDuration = 1f;        // 淡入淡出持续时间
    
    [Header("订阅设置")]
    public bool subscribeOnStart = true;   // 是否在Start时订阅
    public bool unsubscribeOnDestroy = true; // 是否在销毁时取消订阅

    private void Start()
    {
        // 确保BloodMusicManager已初始化
        if (BloodMusicManager.Instance == null)
        {
            Debug.LogError("BossBattleMusicManager: BloodMusicManager instance not found!");
            return;
        }
        
        // 预加载Boss战音乐
        PreloadBossMusicClips();
        
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

    // 预加载Boss战音乐剪辑
    private void PreloadBossMusicClips()
    {
        if (BloodMusicManager.Instance != null)
        {
            List<AudioClip> clipsToPreload = new List<AudioClip>();
            
            if (normalMusic != null) clipsToPreload.Add(normalMusic);
            if (bossMusicPhase1 != null) clipsToPreload.Add(bossMusicPhase1);
            if (bossMusicPhase2 != null) clipsToPreload.Add(bossMusicPhase2);
            
            if (clipsToPreload.Count > 0)
            {
                BloodMusicManager.Instance.PreloadAudioClips(clipsToPreload.ToArray());
                Debug.Log("BossBattleMusicManager: Preloaded " + clipsToPreload.Count + " boss battle music clips");
            }
        }
    }

    // 订阅Boss战事件
    public void SubscribeToBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.AddListener(OnBossBattleStarted);
            EventManager.Instance.OnBossBattleSecondPhase.AddListener(OnBossBattleSecondPhaseStarted);
            EventManager.Instance.OnBossBattleEnd.AddListener(OnBossBattleEnded);
            EventManager.Instance.OnBossDefeated.AddListener(OnBossDefeated);
            Debug.Log("BossBattleMusicManager subscribed to events");
        }
        else
        {
            Debug.LogError("BossBattleMusicManager: EventManager instance not found!");
        }
    }

    // 取消订阅Boss战事件
    public void UnsubscribeFromBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.RemoveListener(OnBossBattleStarted);
            EventManager.Instance.OnBossBattleSecondPhase.RemoveListener(OnBossBattleSecondPhaseStarted);
            EventManager.Instance.OnBossBattleEnd.RemoveListener(OnBossBattleEnded);
            EventManager.Instance.OnBossDefeated.RemoveListener(OnBossDefeated);
            Debug.Log("BossBattleMusicManager unsubscribed from events");
        }
    }

    // Boss战开始时切换到Boss战第一阶段音乐
    private void OnBossBattleStarted()
    {
        Debug.Log("Boss battle music started");
        BloodMusicManager.Instance.SetBackgroundMusicSpeed(1f);
        // if (BloodMusicManager.Instance != null && bossMusicPhase1 != null)
        // {
        //     BloodMusicManager.Instance.SetMusicVolume(0.5f);
        //     BloodMusicManager.Instance.SwitchBackgroundMusic(bossMusicPhase1, fadeDuration);
        // }
    }

    // Boss战第二阶段开始时切换到第二阶段音乐
    private void OnBossBattleSecondPhaseStarted()
    {
        Debug.Log("Boss battle music phase 2 started");
        
        if (BloodMusicManager.Instance != null && bossMusicPhase2 != null)
        {
            BloodMusicManager.Instance.SwitchBackgroundMusic(bossMusicPhase2, fadeDuration);
        }
    }

    // Boss战结束时切换回普通音乐
    private void OnBossBattleEnded()
    {
        Debug.Log("Boss battle music ended, switching back to normal music");
        
        if (BloodMusicManager.Instance != null && normalMusic != null)
        {
            BloodMusicManager.Instance.SwitchBackgroundMusic(normalMusic, fadeDuration);
        }
    }

    // Boss被击败时播放胜利音乐
    private void OnBossDefeated()
    {
        Debug.Log("Boss defeated, playing victory music");
        BloodMusicManager.Instance.SwitchBackgroundMusic(victorySound, fadeDuration);
        // 可以在这里添加胜利音乐逻辑
        // 例如：BloodMusicManager.Instance.PlaySoundEffect(victorySound);
    }
}
