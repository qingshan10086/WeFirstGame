using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk_BOOS1 : MonoBehaviour
{
    [SerializeField] private GameObject player;//获取玩家
    [SerializeField] private GameObject Text;//控制对话框的激活与失活
    [SerializeField] private GameObject Guider;//引导者
    [SerializeField] private Enemy_ShadowMage BOSS;//获取BOSS
    [SerializeField] private GameObject Door;//控制出口

    //通过拖拽获取文本信息
    [SerializeField] private GameObject[] text;
    private int[] currentText = new int[] { 0, 8};//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    [SerializeField] private bool[] canTrigger = new bool[] { true, true};//判断是否能触发文本
    private float textCooldown = 100000f;//第二次文本触发时间//BOOS战不需要二次文本
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
            if(Vector2.Distance(player.transform.position, new Vector2(38f, 3)) < 1)
            {

                if (currentText[0] <= 7)
                {
                    Text.SetActive(true);
                    text[currentText[0]].SetActive(true);

                        if (Input.GetKeyUp(KeyCode.Space))
                        {
                            text[currentText[0]].SetActive(false);
                            currentText[0]++;
                            if (currentText[0] == 7)
                            {
                                Guider.SetActive(false);
                            }
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

        if (BOSS.dieState.canNextText)
            {
                if (currentText[1] <= 19)
                {
                    Text.SetActive(true);
                    text[currentText[1]].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[1]].SetActive(false);
                        currentText[1]++;
                        if (currentText[1] == 9)
                        {
                            Guider.SetActive(true);
                            Guider.transform.position=new Vector2(player.transform.position.x+5,player.transform.position.y);
                        }
                        if (currentText[1] == 20)
                        {
                            Door.SetActive(true);
                            Text.SetActive(false);
                        }
                    }
                }
            }


    }
}


