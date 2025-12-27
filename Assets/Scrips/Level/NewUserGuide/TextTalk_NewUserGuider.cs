using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk : MonoBehaviour
{
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活
    [SerializeField] private GameObject Guider;//引导者
    [SerializeField] private GameObject[] enemy;//获取所有敌人，将敌人全部消灭才可前往下一个场景

    //通过拖拽获取文本信息
    [SerializeField] private GameObject[] text;
    private int[] currentText =new int[] {0,17,18,24,28 };//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    [SerializeField]private bool[] canTrigger = new bool[] { true, true, true, true, true };//判断是否能触发文本
    private float textCooldown = 10f;//第二次文本触发时间
    [SerializeField]private float[] textCooldownTimer = new float[] { 10, 10, 10, 10, 10 };//文本冷却辅助


    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        for(int i = 0; i < 5; i++)//判断能否触发文本
        {
            if (!canTrigger[i])
            {
                textCooldownTimer[i]-=Time.deltaTime;
                if (textCooldownTimer[i] < 0)
                {
                    canTrigger[i] = true;
                    textCooldownTimer[i]=textCooldown;
                }
            }
        }


        if (canTrigger[0])//第一段文本
        {
            
            if (Vector2.Distance(player.transform.position, new Vector2(-1.5f, -3))<1)//小区域触发
            {
            
                if (currentText[0] <= 16)
                {
                    Text.SetActive(true);
                    text[currentText[0]].SetActive(true);


                    if (currentText[0] == 4)
                    {
                        Guider.SetActive(true);
                    }


                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[0]].SetActive(false);
                        currentText[0]++;
                        if (currentText[0] == 17)
                        {
                            Text.SetActive(false);
                            canTrigger[0] = false;
                            currentText[0]=0;//每次触发都从这段文本的初始文本值开始
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

        if (canTrigger[1])//第二段文本
        {

            if (Vector2.Distance(player.transform.position, new Vector2(49f, -3)) < 1)
            {
                if (currentText[1] == 17)
                {
                    Text.SetActive(true);
                    text[currentText[1]].SetActive(true);
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[1]].SetActive(false);
                        currentText[1]++;
                        if (currentText[1] == 18)
                        {
                            Text.SetActive(false);
                            canTrigger[1] = false;
                            currentText[1] = 17;
                        }
                    }
                }
            }
          
        }
        if (canTrigger[2])//第三段文本
        {
            if (Vector2.Distance(player.transform.position, new Vector2(81f, -5f)) < 1)
            {
                if (currentText[2] <= 23)
                {
                    Text.SetActive(true);
                    text[currentText[2]].SetActive(true);
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[2]].SetActive(false);
                        currentText[2]++;
                        if (currentText[2] == 24)
                        {
                            Text.SetActive(false);
                            canTrigger[2] = false;
                            currentText[2] = 18;
                        }
                    }
                }
            }
   
        }

        if (canTrigger[3])//第四段文本
        {

            if (Vector2.Distance(player.transform.position, new Vector2(93f, 42)) < 1)
            {
                if (currentText[3] <= 27)
                {
                    Text.SetActive(true);
                    text[currentText[3]].SetActive(true);
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[3]].SetActive(false);
                        currentText[3]++;
                        if (currentText[3] == 28)
                        {
                            Text.SetActive(false);
                            canTrigger[3] = false;
                            currentText[3] = 24;
                        }
                    }
                }
            }
 
        }
        if (canTrigger[4])//第五段文本
        {

            if (Vector2.Distance(player.transform.position, new Vector2(119f, 38)) < 1)
            {
                foreach (GameObject enemyObj in enemy)
                {
                    if (enemyObj == null)
                    {
                        if (currentText[4] <= 28)
                        {
                            Text.SetActive(true);
                            text[currentText[4]].SetActive(true);
                            if (Input.GetKeyUp(KeyCode.Space))
                            {
                                text[currentText[4]].SetActive(false);
                                currentText[4]++;
                                if (currentText[4] == 29)
                                {
                                    Text.SetActive(false);
                                    canTrigger[4] = false;
                                    currentText[4] = 28;
                                }
                            }
                        }

                    }


                }
            }

        }

    }

    
    
}
