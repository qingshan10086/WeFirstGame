using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritFaderBoss : MonoBehaviour
{
    // 冷却模式枚举
    public enum CooldownMode
    {
        Fixed,
        Random
    }
    
    private SpriteRenderer spriteRenderer;
    
    [Header("自动淡入淡出设置")]
    public bool enableAutoFade = true;  // 是否启用自动淡入淡出
    
    // 固定冷却设置
    public float fixedCooldown = 5f;
    
    private float timer = 0f;
    public float fadeTime = 1f;
    private bool isFading = false;
    private bool isVisible = false; // 初始化为不可见
    private Color originalColor;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        Color newcolor=spriteRenderer.color;
        newcolor.a=0f;
        spriteRenderer.color=newcolor;
        // 设置初始计时器为固定冷却时间
        timer = fixedCooldown;
    }
    
    // Update is called once per frame
    void Update()
    {
        // 只有在启用自动淡入淡出且不在淡入淡出过程中时才执行
        if (enableAutoFade && !isFading)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartFade();
            }
        }
    }
    
    private void StartFade()
    {
        isFading = true;
        
        // 根据当前可见状态选择淡入或淡出
        if (isVisible)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            StartCoroutine(FadeIn());
        }
    }
    
    // 淡入函数
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color newColor = originalColor;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeTime);
            newColor.a = Mathf.Lerp(0f, 1f, progress);
            spriteRenderer.color = newColor;
            yield return null;
        }
        
        // 确保最终透明度为1
        newColor.a = 1f;
        spriteRenderer.color = newColor;
        CompleteFade();
    }
    
    // 淡出函数
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color newColor = originalColor;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeTime);
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            spriteRenderer.color = newColor;
            yield return null;
        }
        
        // 确保最终透明度为0
        newColor.a = 0f;
        spriteRenderer.color = newColor;
        CompleteFade();
    }
    
    private void CompleteFade()
    {
        isFading = false;
        isVisible = !isVisible;
        
        // 设置下一次冷却时间为固定值
        timer = fixedCooldown;
        
        // 确保最终透明度正确
        Color finalColor = originalColor;
        finalColor.a = isVisible ? 1f : 0f;
        spriteRenderer.color = finalColor;
    }
    
    // 外部调用的淡入方法（返回IEnumerator以便协程等待）
    public IEnumerator FadeInExternal()
    {
        if (!isFading)
        {
            isFading = true;
            isVisible = false; // 确保从不可见状态开始淡入
            yield return StartCoroutine(FadeIn());
        }
        else
        {
            // 如果已经在淡入中，等待直到淡入完成
            while (isFading)
            {
                yield return null;
            }
        }
    }
    
    // 外部调用的淡出方法
    public void FadeOutExternal()
    {
        if (!isFading)
        {
            isFading = true;
            isVisible = true; // 确保从可见状态开始淡出
            StartCoroutine(FadeOut());
        }
    }
    
    // 外部调用的切换淡入淡出方法
    public void ToggleFadeExternal()
    {
        if (!isFading)
        {
            StartFade();
        }
    }
    
    // 外部调用的立即设置可见性方法
    public void SetVisibleImmediately(bool visible)
    {
        if (!isFading)
        {
            isVisible = visible;
            Color newColor = originalColor;
            newColor.a = visible ? 1f : 0f;
            spriteRenderer.color = newColor;
        }
    }
    
    // 外部调用的获取当前可见性方法
    public bool IsVisible()
    {
        return isVisible;
    }
    
    // 外部调用的获取是否正在淡入淡出方法
    public bool IsFading()
    {
        return isFading;
    }
}