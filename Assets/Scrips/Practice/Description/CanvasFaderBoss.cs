using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFaderBoss : MonoBehaviour
{
    [Header("Canvas设置")]
    private CanvasGroup canvasGroup;
    private Image imageComponent;
    
    [Header("图片切换设置")]
    public bool enableImageSwitch = false;  // 是否启用图片切换
    public Sprite secondImage;              // 淡入后要切换到的第二张图片
    private Sprite originalImage;           // 原始图片
    private bool isUsingSecondImage = false; // 当前是否使用第二张图片
    
    [Header("自动淡入淡出设置")]
    public bool enableAutoFade = true;  // 是否启用自动淡入淡出
    public float fixedCooldown = 5f;    // 固定冷却时间
    public float fadeTime = 1f;         // 淡入淡出时长
    
    private float timer = 0f;
    private bool isFading = false;
    private bool isVisible = false; // 初始化为不可见
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取或添加CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 获取Image组件
        imageComponent = GetComponent<Image>();
        if (imageComponent != null)
        {
            originalImage = imageComponent.sprite;
        }
        
        // 初始化可见性
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
        
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
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeTime);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            yield return null;
        }
        
        // 确保最终透明度为1，并启用交互
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        // 淡入完成后切换图片
        if (enableImageSwitch && imageComponent != null && secondImage != null)
        {
            // 如果当前使用的是原始图片，则切换到第二张图片
            if (!isUsingSecondImage)
            {
                imageComponent.sprite = secondImage;
                isUsingSecondImage = true;
            }
        }
        
        CompleteFade();
    }
    
    // 淡出函数
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeTime);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            yield return null;
        }
        
        // 确保最终透明度为0，并禁用交互
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        CompleteFade();
    }
    
    private void CompleteFade()
    {
        isFading = false;
        isVisible = !isVisible;
        
        // 如果淡出完成（变为不可见），并且启用了图片切换，则重置为原始图片
        if (!isVisible && enableImageSwitch && imageComponent != null)
        {
            imageComponent.sprite = originalImage;
            isUsingSecondImage = false;
        }
        
        // 设置下一次冷却时间为固定值
        timer = fixedCooldown;
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
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
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
    
    // 外部调用的手动切换图片方法
    public void SwitchImageManually()
    {
        if (imageComponent != null && secondImage != null)
        {
            if (isUsingSecondImage)
            {
                imageComponent.sprite = originalImage;
                isUsingSecondImage = false;
            }
            else
            {
                imageComponent.sprite = secondImage;
                isUsingSecondImage = true;
            }
        }
    }
    
    // 外部调用的重置图片为原始图片方法
    public void ResetToOriginalImage()
    {
        if (imageComponent != null)
        {
            imageComponent.sprite = originalImage;
            isUsingSecondImage = false;
        }
    }
    
    // 外部调用的获取当前是否使用第二张图片方法
    public bool IsUsingSecondImage()
    {
        return isUsingSecondImage;
    }
}
