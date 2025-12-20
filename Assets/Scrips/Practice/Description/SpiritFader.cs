using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritFader : MonoBehaviour
{
    // 冷却模式枚举
    public enum CooldownMode
    {
        Fixed,      // 固定冷却时间
        Random      // 随机冷却时间
    }
    
    private SpriteRenderer spriteRenderer;
    
    [Header("冷却设置")]
    public CooldownMode cooldownMode = CooldownMode.Fixed;
    
    // 固定冷却设置
    public float fixedCooldown = 5f;
    
    // 随机冷却设置
    public float minRandomCooldown = 2f;
    public float maxRandomCooldown = 8f;
    
    private float timer = 0f;
    public float fadeTime = 1f;
    private bool isFading = false;
    private bool isVisible = true;
    private float currentFadeTime = 0f;
    private Color originalColor;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // 根据选择的冷却模式设置初始计时器
        if (cooldownMode == CooldownMode.Fixed)
        {
            timer = fixedCooldown;
        }
        else if (cooldownMode == CooldownMode.Random)
        {
            timer = Random.Range(minRandomCooldown, maxRandomCooldown);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFading)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartFade();
            }
        }
        else
        {
            UpdateFade();
        }
    }

    private void StartFade()
    {
        isFading = true;
        currentFadeTime = fadeTime;
    }

    private void UpdateFade()
    {
        currentFadeTime -= Time.deltaTime;
        float progress = 1f - (currentFadeTime / fadeTime);
        
        Color newColor = originalColor;
        
        if (isVisible)
        {
            // 淡出效果
            newColor.a = Mathf.Lerp(1f, 0f, progress);
        }
        else
        {
            // 淡入效果
            newColor.a = Mathf.Lerp(0f, 1f, progress);
        }
        
        spriteRenderer.color = newColor;
        
        if (currentFadeTime <= 0)
        {
            CompleteFade();
        }
    }

    private void CompleteFade()
    {
        isFading = false;
        isVisible = !isVisible;
        
        // 根据选择的冷却模式设置下一次冷却时间
        if (cooldownMode == CooldownMode.Fixed)
        {
            timer = fixedCooldown;
        }
        else if (cooldownMode == CooldownMode.Random)
        {
            // 在最小和最大随机冷却时间之间生成一个随机值
            timer = Random.Range(minRandomCooldown, maxRandomCooldown);
        }
        
        // 确保最终透明度正确
        Color finalColor = originalColor;
        finalColor.a = isVisible ? 1f : 0f;
        spriteRenderer.color = finalColor;
    }
}
