using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneTimeMechanism : MonoBehaviour
{
    [Header("机关设置")]
    //机关唯一标识符，必填，且必须不一样
    [SerializeField] private string mechanismID = "";
    //是否为全局机关,默认为否
    [SerializeField] private bool isGlobal = false;
    
    //机关是否被触发
    public  bool isTriggered=false;

    private void Awake()
    {
        if (string.IsNullOrEmpty(mechanismID))
        {
            mechanismID = GenerateMechanismID();
        }
    }


    private void Start()
    {
        LoadTriggerStateFromSave();
    }

    /// <summary>
    /// 从存档加载机关的触发状态
    /// </summary>
    private void LoadTriggerStateFromSave()
    {
        // 等待SaveManager初始化完成
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning($"机关 {mechanismID}：存档管理器未初始化，延迟检查...");
            StartCoroutine(DelayedLoadTriggerState());
            return;
        }


        // 检查存档中此机关是否已触发
        isTriggered = SaveManager.Instance.IsMechanismTriggered(mechanismID, isGlobal);

    }

    /// <summary>
    /// 延迟加载触发状态（等待SaveManager初始化）
    /// </summary>
    private IEnumerator DelayedLoadTriggerState()
    {
        // 最多等待2秒
        float timeout = 2f;
        float timer = 0f;

        while (SaveManager.Instance == null && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (SaveManager.Instance != null)
        {
            isTriggered = SaveManager.Instance.IsMechanismTriggered(mechanismID, isGlobal);

        }
        else
        {
            Debug.LogWarning($"机关 {mechanismID}：等待存档管理器超时");
        }
    }

    /// <summary>
    /// 生成机关唯一标识符
    /// 格式:场景名_对象名_X_Y_Z
    /// </summary>
    /// <returns></returns>
    private string GenerateMechanismID()
    {
        string sceneName=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string objectName = gameObject.name;
        Vector3 pos=transform.position;

        return $"{sceneName}_{objectName}_{pos.x:F1}_{pos.y:F1}_{pos.z:F1}";
    }


    public void TriggerMechanism()
    {
        if (isTriggered)
        {
            Debug.Log($"机关{mechanismID}已经触发过，不再触发");
            return;
        }

        isTriggered = true;
        if(SaveManager.Instance != null)
        {
            SaveManager.Instance.MarkMechanismTriggered(mechanismID, isGlobal);
        }
        Debug.Log($"机关 {mechanismID} 已触发并保存到存档");
    }
}
