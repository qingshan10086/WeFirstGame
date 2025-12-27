using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk_Abyss : MonoBehaviour
{
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活
    [SerializeField] private Enemy[] enemy;//杀死敌人后，激活对话框
    private bool hasDie = false;


    [Header("区域范围")]
    [SerializeField] private int region1L;
    [SerializeField] private int region2L;
    [SerializeField] private int region3L;
    [SerializeField] private int region4L;
    [SerializeField] private int region5L;


    //通过拖拽获取文本信息
    [SerializeField] private GameObject[] text;
    private int[] currentText = new int[] { 0,0,0,0,0};//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    [SerializeField] private bool[] canTrigger = new bool[] { true, true, true, true, true };//判断是否能触发文本
    private float textCooldown = 10f;//第二次文本触发时间
    [SerializeField] private float[] textCooldownTimer = new float[] { 10 };//文本冷却辅助
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (canTrigger[0])
        {

            if (player.transform.position.x < region1L+2 && player.transform.position.x > region1L)
            {
                if (currentText[0] <= 4)
                {
                    Text.SetActive(true);
                    text[currentText[0]].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText[0]].SetActive(false);
                        currentText[0]++;
                        if (currentText[0] == 4)
                        {
                            Text.SetActive(false);
                            canTrigger[0] = false;
                            currentText[1] = 4;
                        }
                    }
                }

            }
            

        }

        //解决怪物以触发第二段剧情
        if (!hasDie)
        {
            if (isEnemyDead())
            {
                canTrigger[1] = true;
                hasDie = true;
            }
        }
        if (canTrigger[1])
        { 
            if (currentText[1] <= 12)
            { 
                Text.SetActive(true);
                text[currentText[1]].SetActive(true);
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText[1]].SetActive(false);
                    currentText[1]++;
                    if (currentText[1] == 12)
                    {
                        Text.SetActive(false);
                        canTrigger[1] = false;
                        currentText[1] = 4;
                    }
                }

            }
        }


    }
    private bool isEnemyDead()
    {
        foreach (var enemy in enemy)
        {
            if (enemy.stats.currentHealth > 0)
            {
                return false;
            }
        }
        return true;
    }
}
