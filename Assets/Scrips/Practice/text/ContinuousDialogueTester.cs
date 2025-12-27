// 测试连续对话功能的脚本
// 可以添加到任意GameObject上进行测试
using UnityEngine;

public class ContinuousDialogueTester : MonoBehaviour
{
    public TextData testTextData;
    public TextTriggerZone testTriggerZone;
    
    [ContextMenu("测试TextData连续对话")]
    public void TestTextDataContinuousDialogue()
    {
        if (testTextData == null)
        {
            Debug.LogError("请先赋值testTextData");
            return;
        }
        
        Debug.Log("开始测试TextData连续对话");
        Debug.Log("TextData信息: " + testTextData.GetFormattedInfo());
        Debug.Log("是否使用连续对话: " + testTextData.useContinuousDialogue);
        
        if (testTextData.useContinuousDialogue && testTextData.dialogueTextArray != null)
        {
            Debug.Log("连续对话文本数量: " + testTextData.dialogueTextArray.Length);
            for (int i = 0; i < testTextData.dialogueTextArray.Length; i++)
            {
                Debug.Log("第" + (i+1) + "条文本: " + testTextData.dialogueTextArray[i]);
            }
        }
        
        Debug.Log("测试完成");
    }
    
    [ContextMenu("测试TextTriggerZone连续对话")]
    public void TestTriggerZoneContinuousDialogue()
    {
        if (testTriggerZone == null)
        {
            Debug.LogError("请先赋值testTriggerZone");
            return;
        }
        
        Debug.Log("开始测试TextTriggerZone连续对话");
        Debug.Log("是否使用TextData: " + testTriggerZone.useTextData);
        Debug.Log("是否使用连续对话: " + testTriggerZone.useContinuousDialogue);
        
        if (testTriggerZone.useTextData && testTriggerZone.textData != null)
        {
            Debug.Log("TextData是否使用连续对话: " + testTriggerZone.textData.useContinuousDialogue);
        }
        else if (testTriggerZone.useContinuousDialogue)
        {
            Debug.Log("本地连续对话文本数量: " + testTriggerZone.dialogueTextArray.Length);
        }
        
        Debug.Log("测试完成");
    }
    
    [ContextMenu("触发TextTriggerZone")]
    public void TriggerTestZone()
    {
        if (testTriggerZone == null)
        {
            Debug.LogError("请先赋值testTriggerZone");
            return;
        }
        
        Debug.Log("手动触发TextTriggerZone");
        testTriggerZone.TriggerTextDisplay();
    }
}