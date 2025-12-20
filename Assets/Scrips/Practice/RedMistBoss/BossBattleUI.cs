// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class BossBattleUI : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public GameObject bossBattleUIPanel;
//     public Image bossHealthBar;
//     public Text bossNameText;
//     public Text countdownText;
//     public GameObject bossWarningPanel;
    
//     [Header("Settings")]
//     public string bossName = "Red Mist Boss";
//     public float warningDuration = 3f;
//     public float fadeDuration = 0.5f;
    
//     private CanvasGroup canvasGroup;
//     private CanvasGroup warningCanvasGroup;
//     private bool isBossBattleActive = false;
    
//     private void Awake()
//     {
//         // 初始化CanvasGroup组件用于淡入淡出效果
//         if (bossBattleUIPanel != null)
//         {
//             canvasGroup = bossBattleUIPanel.GetComponent<CanvasGroup>();
//             if (canvasGroup == null)
//             {
//                 canvasGroup = bossBattleUIPanel.AddComponent<CanvasGroup>();
//             }
//         }
        
//         if (bossWarningPanel != null)
//         {
//             warningCanvasGroup = bossWarningPanel.GetComponent<CanvasGroup>();
//             if (warningCanvasGroup == null)
//             {
//                 warningCanvasGroup = bossWarningPanel.AddComponent<CanvasGroup>();
//             }
//         }
//     }
    
//     private void OnEnable()
//     {
//         // 订阅Boss战事件
//         if (EventManager.Instance != null)
//         {
//             EventManager.Instance.OnBossBattleStart.AddListener(StartBossBattleUI);
//             EventManager.Instance.OnBossBattleEnd.AddListener(EndBossBattleUI);
//         }
//     }
    
//     private void OnDisable()
//     {
//         // 取消订阅Boss战事件
//         if (EventManager.Instance != null)
//         {
//             EventManager.Instance.OnBossBattleStart.RemoveListener(StartBossBattleUI);
//             EventManager.Instance.OnBossBattleEnd.RemoveListener(EndBossBattleUI);
//         }
//     }
    
//     private void Start()
//     {
//         // 初始状态：隐藏所有Boss战UI
//         if (bossBattleUIPanel != null)
//         {
//             bossBattleUIPanel.SetActive(false);
//             canvasGroup.alpha = 0f;
//         }
        
//         if (bossWarningPanel != null)
//         {
//             bossWarningPanel.SetActive(false);
//             warningCanvasGroup.alpha = 0f;
//         }
        
//         // 设置Boss名称
//         if (bossNameText != null)
//         {
//             bossNameText.text = bossName;
//         }
        
//         // 初始化血条
//         if (bossHealthBar != null)
//         {
//             bossHealthBar.fillAmount = 1f;
//         }
//     }
    
//     private void StartBossBattleUI()
//     {
//         isBossBattleActive = true;
        
//         // 显示Boss警告面板
//         StartCoroutine(ShowBossWarning());
//     }
    
//     private void EndBossBattleUI()
//     {
//         isBossBattleActive = false;
        
//         // 隐藏Boss战UI面板
//         StartCoroutine(FadeOutUI());
//     }
    
//     private IEnumerator ShowBossWarning()
//     {
//         if (bossWarningPanel == null)
//             yield break;
            
//         bossWarningPanel.SetActive(true);
        
//         // 淡入警告面板
//         yield return StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 0f, 1f, fadeDuration));
        
//         // 显示倒计时
//         if (countdownText != null)
//         {
//             for (int i = 3; i > 0; i--)
//             {
//                 countdownText.text = i.ToString();
//                 yield return new WaitForSeconds(1f);
//             }
//             countdownText.text = "FIGHT!";
//             yield return new WaitForSeconds(1f);
//         }
        
//         // 淡出警告面板
//         yield return StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 1f, 0f, fadeDuration));
//         bossWarningPanel.SetActive(false);
        
//         // 显示Boss战主UI
//         ShowBossBattleMainUI();
//     }
    
//     private void ShowBossBattleMainUI()
//     {
//         if (bossBattleUIPanel == null)
//             return;
            
//         bossBattleUIPanel.SetActive(true);
//         StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));
//     }
    
//     private IEnumerator FadeOutUI()
//     {
//         if (bossBattleUIPanel == null)
//             yield break;
            
//         yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration));
//         bossBattleUIPanel.SetActive(false);
//     }
    
//     // 通用的CanvasGroup淡入淡出协程
//     private IEnumerator FadeCanvasGroup(CanvasGroup target, float startAlpha, float endAlpha, float duration)
//     {
//         if (target == null)
//             yield break;
            
//         float elapsedTime = 0f;
//         target.alpha = startAlpha;
        
//         while (elapsedTime < duration)
//         {
//             elapsedTime += Time.deltaTime;
//             float progress = Mathf.Clamp01(elapsedTime / duration);
//             target.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
//             yield return null;
//         }
        
//         target.alpha = endAlpha;
//     }
    
//     // 更新Boss生命值的方法（可由Boss脚本调用）
//     public void UpdateBossHealth(float currentHealth, float maxHealth)
//     {
//         if (bossHealthBar != null)
//         {
//             float healthPercentage = Mathf.Clamp01(currentHealth / maxHealth);
//             bossHealthBar.fillAmount = healthPercentage;
//         }
//     }
    
//     // 显示Boss技能警告的方法（可由Boss脚本调用）
//     public void ShowSkillWarning(string warningText, float duration)
//     {
//         StartCoroutine(DisplaySkillWarning(warningText, duration));
//     }
    
//     private IEnumerator DisplaySkillWarning(string text, float duration)
//     {
//         // 这里可以扩展显示技能警告的逻辑
//         Debug.Log("Boss Skill Warning: " + text);
//         yield return new WaitForSeconds(duration);
//     }
// }
