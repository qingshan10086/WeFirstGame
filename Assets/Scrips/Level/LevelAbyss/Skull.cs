using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 2f; // 闪烁速度
    [SerializeField] private float minAlpha = 0.2f; // 最小透明度

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isBlinking = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // 开始闪烁协程
        StartCoroutine(BlinkCoroutine());
    }

    System.Collections.IEnumerator BlinkCoroutine()
    {
        while (isBlinking)
        {
            // 计算正弦波值来控制透明度
            float alpha = Mathf.Lerp(minAlpha, 1f,
                (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);

            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );

            yield return null;
        }
    }

    // 停止闪烁并恢复原色
    public void StopBlinking()
    {
        isBlinking = false;
        spriteRenderer.color = originalColor;
    }
}
