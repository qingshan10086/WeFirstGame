using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk_Level1 : MonoBehaviour
{
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活
   

    //通过拖拽获取文本信息
    [SerializeField] private GameObject[] text;
    private int[] currentText = new int[] { 0, 8 };//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    [SerializeField] private bool[] canTrigger = new bool[] { true, true };//判断是否能触发文本
    private float textCooldown = 10f;//下一次文本触发时间
    [SerializeField] private float[] textCooldownTimer = new float[] { 10, 10};//文本冷却辅助
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++)//判断能否触发文本
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

            if (player.transform.position.x < 0&& player.transform.position.x > -2)
            {
                if (currentText[0] <= 7)
                {
                    Text.SetActive(true);
                    text[currentText[0]].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[0]].SetActive(false);
                        currentText[0]++;
                        if (currentText[0] == 8)
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

        if (canTrigger[1])
        {

            if (player.transform.position.x < 60.8f && player.transform.position.x >59)
            {
                if (currentText[1] <= 8)
                {   Text.SetActive(true);
                
                    text[currentText[1]].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[1]].SetActive(false);
                        currentText[1]++;
                        if (currentText[1] == 9)
                        {
                            Text.SetActive(false);
                            canTrigger[1] = false;
                            currentText[1] = 8;
                        }

                    }
                }

            }
        }



        

    }
}
