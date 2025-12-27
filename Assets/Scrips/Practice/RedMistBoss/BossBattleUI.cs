using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossBattleUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject bossBattleUIPanel;
    public Image bossHealthBar;
    public Text bossNameText;
    
    [Header("Second Phase UI")]
    public GameObject phase2Canvas1; // 第二阶段第一张画布
    public GameObject phase2Canvas2; // 第二阶段第二张画布
    
    [Header("Settings")]
    public string bossName = "Red Mist Boss";
    public float fadeDuration = 1f;
    public float phaseTransitionDuration = 1.0f; // 两个阶段之间的过渡时间
    
    private CanvasGroup canvasGroup;
    private CanvasGroup phase2Canvas1Group;
    private CanvasGroup phase2Canvas2Group;
    private bool isBossBattleActive = false;

    [Header("EyeDescription")]
    public SpiritFaderBoss[]spiritFaderBoss;
    public CanvasFaderBoss canvasFaderBoss;
    [Header("NameAndBG")]
    public float NameDurationTime = 1f;
    public float EyeDurationTime = 2f;
    [Header("Music")]
    public AudioClip bossMusicPhase1;        // Boss战第一阶段音乐剪辑
    [Header("Door")]
    public doormoveup doorMoveUp1;
    public doormoveup doorMoveUp2;
    [Header("Boss血条")]
    public GameObject RedMistHealthBar;

    private void Awake()
    {
        // 初始化CanvasGroup组件用于淡入淡出效果
        if (bossBattleUIPanel != null)
        {
            canvasGroup = bossBattleUIPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = bossBattleUIPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // 初始化第二阶段画布的CanvasGroup组件
        if (phase2Canvas1 != null)
        {
            phase2Canvas1Group = phase2Canvas1.GetComponent<CanvasGroup>();
            if (phase2Canvas1Group == null)
            {
                phase2Canvas1Group = phase2Canvas1.AddComponent<CanvasGroup>();
            }
        }
        
        if (phase2Canvas2 != null)
        {
            phase2Canvas2Group = phase2Canvas2.GetComponent<CanvasGroup>();
            if (phase2Canvas2Group == null)
            {
                phase2Canvas2Group = phase2Canvas2.AddComponent<CanvasGroup>();
            }
        }
    }
    
    private void OnEnable()
    {
        // 订阅Boss战事件
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.AddListener(StartBossBattleUI);
            EventManager.Instance.OnBossBattleEnd.AddListener(EndBossBattleUI);
            EventManager.Instance.OnBossDefeated.AddListener(OnBossDefeated);
        }
    }
    
    private void OnDisable()
    {
        // 取消订阅Boss战事件
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.RemoveListener(StartBossBattleUI);
            EventManager.Instance.OnBossBattleEnd.RemoveListener(EndBossBattleUI);
            EventManager.Instance.OnBossDefeated.RemoveListener(OnBossDefeated);
        }
    }
    
    private void Start()
    {
        // 初始状态：隐藏所有Boss战UI
        if (bossBattleUIPanel != null)
        {
            bossBattleUIPanel.SetActive(false);
            canvasGroup.alpha = 0f;
        }
        
        // 初始化第二阶段画布状态
        if (phase2Canvas1 != null)
        {
            phase2Canvas1.SetActive(false);
            phase2Canvas1Group.alpha = 0f;
        }
        
        if (phase2Canvas2 != null)
        {
            phase2Canvas2.SetActive(false);
            phase2Canvas2Group.alpha = 0f;
        }
        
        // 设置Boss名称
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
        
        // 初始化血条
        if (bossHealthBar != null)
        {
            bossHealthBar.fillAmount = 1f;
        }

    }
    
    private void StartBossBattleUI()
    {
        isBossBattleActive = true;
        
        // 直接进入两阶段过渡（跳过警告面板）
        StartCoroutine(PhaseTransition());
    }
    
    private void EndBossBattleUI()
    {
        isBossBattleActive = false;
        
        // 隐藏Boss战UI面板
        StartCoroutine(FadeOutUI());
    }
    
    /// <summary>
    /// Boss被击败时的UI处理
    /// </summary>
    private void OnBossDefeated()
    {
        Debug.Log("Boss has been defeated! Handling UI effects...");
        
        // 标记战斗结束
        isBossBattleActive = false;
        
        // 开始Boss被击败的UI过渡效果
        StartCoroutine(BossDefeatedUI());
    }
    
    /// <summary>
    /// Boss被击败时的UI过渡效果
    /// </summary>
    private IEnumerator BossDefeatedUI()
    {
        // 首先淡出Boss战主UI面板
        if (bossBattleUIPanel != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration));
            bossBattleUIPanel.SetActive(false);
        }
        doorMoveUp1.MoveUpSmooth(-20f);
        doorMoveUp2.MoveUpSmooth(-20f);
        // 淡出第二阶段画布
        if (phase2Canvas1 != null && phase2Canvas1.activeSelf)
        {
            yield return StartCoroutine(FadeCanvasGroup(phase2Canvas1Group, 1f, 0f, fadeDuration));
            phase2Canvas1.SetActive(false);
        }
        
        if (phase2Canvas2 != null && phase2Canvas2.activeSelf)
        {
            yield return StartCoroutine(FadeCanvasGroup(phase2Canvas2Group, 1f, 0f, fadeDuration));
            phase2Canvas2.SetActive(false);
        }
        foreach(var spiritFader in spiritFaderBoss)
        {
             spiritFader.FadeOutExternal();
        }
        // 可以在这里添加Boss被击败的特殊UI效果
        // 例如：显示胜利文字、播放庆祝动画等
        
        Debug.Log("Boss defeated UI effects completed!");
    }
    
    private void ShowBossBattleMainUI()
    {
        if (bossBattleUIPanel == null)
            return;
            
        bossBattleUIPanel.SetActive(true);
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));
    }
    
    // 实现Boss战的两阶段UI过渡
    private IEnumerator PhaseTransition()
    {
        yield return new WaitForSeconds(3f);
        doorMoveUp1.MoveUpSmooth(20f);
        doorMoveUp2.MoveUpSmooth(20f);

        yield return canvasFaderBoss.FadeInExternal();
        foreach(var spiritFader in spiritFaderBoss)
        {
          yield return spiritFader.FadeInExternal();
        }
        // 第一阶段：预备阶段（用户可以在这里添加自己的逻辑）
        // 不显示任何画布，仅作为用户自定义逻辑的阶段
        yield return new WaitForSeconds(EyeDurationTime);
        
        // 第二阶段：显示两张画布和主UI面板
        if (phase2Canvas1 != null)
        {
            phase2Canvas2.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(phase2Canvas2Group, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(NameDurationTime);
            if (phase2Canvas1 != null)
        {
            phase2Canvas1.SetActive(true);
            BloodMusicManager.Instance.SwitchBackgroundMusic(bossMusicPhase1,0.1f);
            yield return StartCoroutine(FadeCanvasGroup(phase2Canvas1Group, 0f, 1f, fadeDuration));
            canvasFaderBoss.gameObject.SetActive(false);
            yield return new WaitForSeconds(NameDurationTime);
        }
            yield return StartCoroutine(FadeCanvasGroup(phase2Canvas1Group, 1f, 0f, fadeDuration));
            EventManager.Instance.activateBoss();
            BloodMusicManager.Instance.player.enabled = true;
        }
        
        
        // 显示Boss战主UI面板
        ShowBossBattleMainUI();
        RedMistHealthBar.SetActive(true);
      
    }
    
    private IEnumerator FadeOutUI()
    {
        // 淡出主UI面板
        if (bossBattleUIPanel != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration));
            bossBattleUIPanel.SetActive(false);
        }
        
        // 淡出并隐藏第二阶段第一张画布
        if (phase2Canvas1 != null && phase2Canvas1.activeSelf)
        {
            yield return StartCoroutine(FadeCanvasGroup(phase2Canvas1Group, 1f, 0f, fadeDuration));
            phase2Canvas1.SetActive(false);
        }
        
        // 隐藏第二阶段第二张画布（作为背景，直接隐藏无需淡出）
        if (phase2Canvas2 != null && phase2Canvas2.activeSelf)
        {
            phase2Canvas2.SetActive(false);
        }
    }
    
    // 通用的CanvasGroup淡入淡出协程
    private IEnumerator FadeCanvasGroup(CanvasGroup target, float startAlpha, float endAlpha, float duration)
    {
        if (target == null)
            yield break;
            
        float elapsedTime = 0f;
        target.alpha = startAlpha;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            target.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            yield return null;
        }
        
        target.alpha = endAlpha;
    }
    
    // 更新Boss生命值的方法（可由Boss脚本调用）
    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        if (bossHealthBar != null)
        {
            float healthPercentage = Mathf.Clamp01(currentHealth / maxHealth);
            bossHealthBar.fillAmount = healthPercentage;
        }
    }
    
    // 显示Boss技能警告的方法（可由Boss脚本调用）
    public void ShowSkillWarning(string warningText, float duration)
    {
        StartCoroutine(DisplaySkillWarning(warningText, duration));
    }
    
    private IEnumerator DisplaySkillWarning(string text, float duration)
    {
        // 这里可以扩展显示技能警告的逻辑
        Debug.Log("Boss Skill Warning: " + text);
        yield return new WaitForSeconds(duration);
    }
}