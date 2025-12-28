using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk_Level2 : MonoBehaviour
{

    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活


    //通过拖拽获取文本信息
    [SerializeField] private GameObject[] text;
    private int[] currentText = new int[] { 0};//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    [SerializeField] private bool[] canTrigger = new bool[] { true, true, true, true, true };//判断是否能触发文本
    private float textCooldown = 60f;//第二次文本触发时间
    [SerializeField] private float[] textCooldownTimer = new float[] { 10 };//文本冷却辅助
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 1; i++)//判断能否触发文本
        {
            if (!canTrigger[i])
            {
                textCooldownTimer[i] -= Time.deltaTime;
                if (textCooldownTimer[i] < 0)
                {
                    canTrigger[i] = true;
                    textCooldownTimer[i] = textCooldown;
                }
            }
        }

        if (canTrigger[0])
        {
            
            if (player.transform.position.x < 16&& player.transform.position.x > 14)
            {
                if (currentText[0] <= 6)
                {
                    Text.SetActive(true);
                    text[currentText[0]].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[0]].SetActive(false);
                        currentText[0]++;
                        if (currentText[0] == 7)
                        {
                            Text.SetActive(false);
                            canTrigger[0] = false;
                            currentText[0] = 0;//每次触发都从这段文本的初始文本值开始
                        }
                    }
                }

            }
            else//防止进场景每次都卡在第一段文本
            {
                text[currentText[0]].SetActive(false);
                Text.SetActive(false);
            }

        }




    }
}
