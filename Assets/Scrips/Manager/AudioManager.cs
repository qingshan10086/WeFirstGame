using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;



    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;


    public bool playBgm;
    private int bgmIndex=3;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else {instance = this; }
    }


    private void Update()
    {
        if (!playBgm)
        {
            StopAllBGM();
        }
        else
        {
            if (!bgm[bgmIndex].isPlaying) { PlayBGM(bgmIndex); }
        }
    }


    public void PlaySFX(int _sfxIndex)//播放音效音频的函数
    {
        float pitch = sfx[_sfxIndex].pitch;//保证pitch不变
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(pitch * 0.85f, pitch * 1.1f);            
            sfx[_sfxIndex].Play();
            sfx[_sfxIndex].pitch=pitch;  
        }
    }
    public void StopSFX(int _index)=>sfx[_index].Stop();//停止音效音频播放的函数
   
    public void PlayBGM(int _bgmIndex)//播放背景音频
    {
        bgmIndex = _bgmIndex;
        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()//停止所有背景音频
    {
        for (int i = 0;i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
