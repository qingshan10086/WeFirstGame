using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 场景淡入淡出过渡效果 - 单例模式
/// 用于实现进门时的场景切换过渡
/// 可以全局访问和多次使用
/// </summary>
public class SceneFade : MonoBehaviour
{
    #region 单例实例
    private static SceneFade instance;
    public static SceneFade Instance
    {
        get
        {
            if (instance == null)
            {
                // 创建一个新的实例
                GameObject obj = new GameObject("SceneFade");
                instance = obj.AddComponent<SceneFade>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    #endregion

    #region 组件引用
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    #endregion

    #region 淡入淡出参数
    [Header("淡入淡出参数")]
    [SerializeField] private float fadeDuration = 1f;       // 淡入淡出持续时间
    [SerializeField] private float startAlpha = 1f;         // 初始透明度
    [SerializeField] private float fadeInHoldTime = 1f;     // 淡入时保持黑色的时间
    #endregion

    #region 私有变量
    private bool isFading = false;                         // 是否正在进行淡入淡出
    #endregion

    #region 生命周期方法
    private void Awake()
    {
        // 实现单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化CanvasGroup组件
        if (fadeCanvasGroup == null)
        {
            // 如果没有提供CanvasGroup，尝试获取或创建
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingLayerName="UI";
                canvas.sortingOrder = 1000; // 设置为最高层级
            }

            fadeCanvasGroup = GetComponent<CanvasGroup>();
            if (fadeCanvasGroup == null)
            {
                fadeCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        // 添加黑色背景Image组件，确保淡入淡出效果可见
        Image backgroundImage = GetComponent<Image>();
        if (backgroundImage == null)
        {
            backgroundImage = gameObject.AddComponent<Image>();
            backgroundImage.color = Color.black;
            backgroundImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }

        // 设置初始透明度
        fadeCanvasGroup.alpha = startAlpha;
        fadeCanvasGroup.blocksRaycasts = startAlpha > 0;
        fadeCanvasGroup.interactable = startAlpha > 0;
    }

    private void Start()
    {
        // 场景加载完成后自动淡入
        StartCoroutine(FadeIn());
    }
    #endregion

    #region 淡入淡出方法
    /// <summary>
    /// 淡入效果（从黑色到正常）
    /// </summary>
    public IEnumerator FadeIn()
    {
        if (isFading) yield break;
        isFading = true;

        // 确保初始状态是黑色
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;
        
        // 先保持黑色fadeInHoldTime秒不变
        yield return new WaitForSeconds(fadeInHoldTime);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
        isFading = false;
    }

    /// <summary>
    /// 淡出效果（从正常到黑色）
    /// </summary>
    public IEnumerator FadeOut()
    {
        if (isFading) yield break;
        isFading = true;

        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        isFading = false;
    }

    /// <summary>
    /// 淡出后加载场景
    /// </summary>
    /// <param name="sceneName">要加载的场景名称</param>
    public IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 淡入淡出组合效果
    /// 用于进门时的场景过渡
    /// </summary>
    public IEnumerator FadeInOut()
    {
        yield return FadeOut();
        yield return FadeIn();
    }
    #endregion

    #region 快捷方法
    /// <summary>
    /// 快速淡入（实例方法）
    /// </summary>
    public void QuickFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// 快速淡入（静态方法）
    /// </summary>
    public static void StaticQuickFadeIn()
    {
        Instance.StartCoroutine(Instance.FadeIn());
    }

    /// <summary>
    /// 快速淡出（实例方法）
    /// </summary>
    public void QuickFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// 快速淡出（静态方法）
    /// </summary>
    public static void StaticQuickFadeOut()
    {
        Instance.StartCoroutine(Instance.FadeOut());
    }

    /// <summary>
    /// 快速淡入淡出（实例方法）
    /// </summary>
    public void QuickFadeInOut()
    {
        StartCoroutine(FadeInOut());
    }

    /// <summary>
    /// 快速淡入淡出（静态方法）
    /// </summary>
    public static void StaticQuickFadeInOut()
    {
        Instance.StartCoroutine(Instance.FadeInOut());
    }

    /// <summary>
    /// 快速淡出并加载场景（实例方法）
    /// </summary>
    /// <param name="sceneName">要加载的场景名称</param>
    public void QuickFadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    /// <summary>
    /// 快速淡出并加载场景（静态方法）
    /// </summary>
    /// <param name="sceneName">要加载的场景名称</param>
    public static void StaticQuickFadeOutAndLoadScene(string sceneName)
    {
        Instance.StartCoroutine(Instance.FadeOutAndLoadScene(sceneName));
    }
    #endregion
}