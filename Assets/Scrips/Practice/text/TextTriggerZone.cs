using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 文本触发区域
/// 当玩家进入特定范围时显示对应的文本
/// </summary>
public class TextTriggerZone : MonoBehaviour
{
    [Header("基本设置")]
    [Tooltip("触发区域的宽度")]
    public float triggerWidth = 5f;
    
    [Tooltip("触发区域的高度")]
    public float triggerHeight = 5f;
    
    [Header("文本设置")]
    [Tooltip("使用TextData ScriptableObject")]
    public bool useTextData = false;
    
    [Tooltip("文本数据对象")]
    public TextData textData;
    
    [Tooltip("要显示的文本")]
    public string displayText = "这是一个文本触发区域！";
    
    [Tooltip("连续对话文本数组")]
    public string[] dialogueTextArray;
    
    [Tooltip("文本显示时间")]
    public float displayTime = 3f;
    
    [Tooltip("文本颜色")]
    public Color textColor = Color.white;
    
    [Tooltip("背景颜色")]
    public Color backgroundColor = new Color(0, 0, 0, 0.5f);
    
    [Tooltip("是否使用连续对话")]
    public bool useContinuousDialogue = false;
    
    [Header("触发设置")]
    [Tooltip("是否只触发一次")]
    public bool triggerOnce = true;
    
    [Tooltip("玩家离开后是否重置触发状态")]
    public bool resetOnExit = false;
    
    [Tooltip("是否立即显示文本（清除队列）")]
    public bool displayImmediately = false;
    
    [Header("玩家设置")]
    [Tooltip("玩家标签")]
    public string playerTag = "Player";
    
    [Tooltip("是否需要指定特定玩家对象")]
    public bool useSpecificPlayer = false;
    
    [Tooltip("特定玩家对象(仅当useSpecificPlayer为true时有效)")]
    public GameObject specificPlayer;
    
    // 内部变量
    private bool playerInZone = false;
    private bool hasTriggered = false;
    
    private void Awake()
    {
        // 设置碰撞体
        SetupCollider();
    }
    
    private void SetupCollider()
    {
        // 如果没有2D碰撞体，添加一个BoxCollider2D
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        
        if (boxCollider2D == null)
        {
            boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
        }
        else
        {
            boxCollider2D.isTrigger = true;
        }
        
        // 设置矩形大小
        boxCollider2D.size = new Vector2(triggerWidth, triggerHeight);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"TextTriggerZone: OnTriggerEnter2D called by {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (IsPlayer(other.gameObject))
        {
            Debug.Log($"TextTriggerZone: {other.gameObject.name} is a valid player!");
            PlayerEnteredZone();
        }
        else
        {
            Debug.Log($"TextTriggerZone: {other.gameObject.name} is not a valid player (Tag: {other.gameObject.tag}, UseSpecificPlayer: {useSpecificPlayer})");
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"TextTriggerZone: OnTriggerExit2D called by {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (IsPlayer(other.gameObject))
        {
            Debug.Log($"TextTriggerZone: {other.gameObject.name} is a valid player!");
            PlayerExitedZone();
        }
    }
    
    private bool IsPlayer(GameObject obj)
    {
        if (useSpecificPlayer)
        {
            return obj == specificPlayer;
        }
        return obj.CompareTag(playerTag);
    }
    
    private void PlayerEnteredZone()
    {
        if (playerInZone)
            return;
            
        playerInZone = true;
        
        // 检查是否可以触发
        if (CanTriggerText())
        {
            TriggerTextDisplay();
        }
    }
    
    private void PlayerExitedZone()
    {
        playerInZone = false;
        
        // 如果需要在离开时重置触发状态
        if (resetOnExit)
        {
            hasTriggered = false;
            Debug.Log($"TextTriggerZone: 玩家离开区域，重置触发状态");
        }
    }
    
    private bool CanTriggerText()
    {
        // 检查是否已经触发过且只允许触发一次
        if (triggerOnce && hasTriggered)
        {
            Debug.Log($"TextTriggerZone: 已触发过文本，不再重复触发");
            return false;
        }
        
        // 检查TextDisplayManager是否存在
        if (TextDisplayManager.Instance == null)
        {
            Debug.LogError($"TextTriggerZone: TextDisplayManager.Instance not found!");
            return false;
        }
        
        return true;
    }
    
    public void TriggerTextDisplay()
    {
        if (useTextData && textData != null)
        {
            // 使用TextData模式
            if (textData.useContinuousDialogue)
            {
                // TextData连续对话模式
                if (textData.dialogueTextArray == null || textData.dialogueTextArray.Length == 0)
                {
                    Debug.LogWarning($"TextTriggerZone: TextData dialogueTextArray is empty");
                    return;
                }
                
                if (displayImmediately)
                {
                    // 立即显示模式：显示第一条，其余排队
                    TextDisplayManager.Instance.DisplayTextImmediately(
                        textData.dialogueTextArray[0],
                        textData.displayTime,
                        textData.textColor,
                        textData.backgroundColor
                    );
                    
                    // 添加剩余文本到队列
                    for (int i = 1; i < textData.dialogueTextArray.Length; i++)
                    {
                        TextDisplayManager.Instance.DisplayText(
                            textData.dialogueTextArray[i],
                            textData.displayTime,
                            textData.textColor,
                            textData.backgroundColor
                        );
                    }
                }
                else
                {
                    // 普通排队模式：所有文本按顺序排队
                    foreach (string text in textData.dialogueTextArray)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            TextDisplayManager.Instance.DisplayText(
                                text,
                                textData.displayTime,
                                textData.textColor,
                                textData.backgroundColor
                            );
                        }
                    }
                }
                
                Debug.Log($"TextTriggerZone: 玩家进入区域，显示TextData连续对话，共{textData.dialogueTextArray.Length}条消息");
            }
            else
            {
                // 使用TextData的单行文本显示
                if (string.IsNullOrEmpty(textData.displayText))
                {
                    Debug.LogWarning($"TextTriggerZone: TextData displayText is empty");
                    return;
                }
                
                // 显示文本
                if (displayImmediately)
                {
                    TextDisplayManager.Instance.DisplayTextImmediately(
                        textData.displayText,
                        textData.displayTime,
                        textData.textColor,
                        textData.backgroundColor
                    );
                }
                else
                {
                    TextDisplayManager.Instance.DisplayText(
                        textData.displayText,
                        textData.displayTime,
                        textData.textColor,
                        textData.backgroundColor
                    );
                }
                
                Debug.Log($"TextTriggerZone: 玩家进入区域，显示TextData文本: {textData.displayText}");
            }
        }
        else if (useContinuousDialogue)
        {
            // 本地连续对话模式
            if (dialogueTextArray == null || dialogueTextArray.Length == 0)
            {
                Debug.LogWarning($"TextTriggerZone: dialogueTextArray is empty");
                return;
            }
            
            if (displayImmediately)
            {
                // 立即显示模式：显示第一条，其余排队
                TextDisplayManager.Instance.DisplayTextImmediately(
                    dialogueTextArray[0],
                    displayTime,
                    textColor,
                    backgroundColor
                );
                
                // 添加剩余文本到队列
                for (int i = 1; i < dialogueTextArray.Length; i++)
                {
                    TextDisplayManager.Instance.DisplayText(
                        dialogueTextArray[i],
                        displayTime,
                        textColor,
                        backgroundColor
                    );
                }
            }
            else
            {
                // 普通排队模式：所有文本按顺序排队
                foreach (string text in dialogueTextArray)
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        TextDisplayManager.Instance.DisplayText(
                            text,
                            displayTime,
                            textColor,
                            backgroundColor
                        );
                    }
                }
            }
            
            Debug.Log($"TextTriggerZone: 玩家进入区域，显示本地连续对话，共{dialogueTextArray.Length}条消息");
        }
        else
        {
            // 直接文本显示
            if (string.IsNullOrEmpty(displayText))
            {
                Debug.LogWarning($"TextTriggerZone: displayText is empty");
                return;
            }
            
            // 显示文本
            if (displayImmediately)
            {
                TextDisplayManager.Instance.DisplayTextImmediately(
                    displayText, 
                    displayTime, 
                    textColor, 
                    backgroundColor
                );
            }
            else
            {
                TextDisplayManager.Instance.DisplayText(
                    displayText, 
                    displayTime, 
                    textColor, 
                    backgroundColor
                );
            }
            
            Debug.Log($"TextTriggerZone: 玩家进入区域，显示文本: {displayText}");
        }
        
        // 标记为已触发
        hasTriggered = true;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 在编辑器中绘制矩形触发区域的范围
        Gizmos.color = Color.yellow;
        
        // 计算矩形的大小和位置（以transform为中心）
        Vector3 cubeSize = new Vector3(triggerWidth, triggerHeight, 0.1f);
        Vector3 cubePosition = transform.position;
        
        Gizmos.DrawWireCube(cubePosition, cubeSize);
        
        // 添加文本标签
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.fontSize = 12;
        UnityEditor.Handles.Label(transform.position + Vector3.up * (triggerHeight / 2 + 0.5f), 
            "Text Trigger: " + (string.IsNullOrEmpty(displayText) ? "[No Text]" : displayText.Substring(0, Mathf.Min(20, displayText.Length)) + (displayText.Length > 20 ? "..." : "")), 
            style);
    }
    
    // 调试功能
    [ContextMenu("测试：触发文本显示")]
    private void TestTriggerText()
    {
        TriggerTextDisplay();
    }
    
    [ContextMenu("重置触发状态")]
    private void ResetTriggerState()
    {
        hasTriggered = false;
        Debug.Log($"TextTriggerZone: 已重置触发状态");
    }
#endif
}
