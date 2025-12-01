using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk : MonoBehaviour
{
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活
    [SerializeField] private GameObject[] enemy;//获取所有敌人，将敌人全部消灭才可前往下一个场景

    //通过拖拽获取文本信息
    [SerializeField] private GameObject[] text;
    private int currentText = 0;
    

    private float currenntText;//记录当前是哪个文本
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (player.transform.position.x < -1 && player.transform.position.x > -2)
        {
            if (currentText <= 7)
            {
                text[currentText].SetActive(true);

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText].SetActive(false);
                    currentText++;
                    if (currentText == 8)
                    {
                        Text.SetActive(false);
                    }
                }
            }
            
        }



        if (player.transform.position.x > 48 && player.transform.position.x < 50)
        {
            if (currentText == 8)
            {
                Text.SetActive(true);
                text[currentText].SetActive(true);
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText].SetActive(false);
                    currentText++;
                    Text.SetActive(false);
                }
            }
        }

        if (player.transform.position.x > 80 && player.transform.position.x < 82)
        {
            if (currentText <= 10)
            {
                Text.SetActive(true);
                text[currentText].SetActive(true);
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText].SetActive(false);
                    currentText++;
                    if(currentText == 11)
                    Text.SetActive(false);
                }
            }
        }


        if (player.transform.position.x > 92 && player.transform.position.x < 94)
        {
            if (currentText <= 11)
            {
                Text.SetActive(true);
                text[currentText].SetActive(true);
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText].SetActive(false);
                    currentText++;
                    if (currentText == 12)
                        Text.SetActive(false);
                }
            }
        }

        if (player.transform.position.x > 118 && player.transform.position.x < 120)
        {
            foreach(GameObject enemyObj in enemy)
            {
                if (enemyObj == null)
                {
                      if (currentText <= 12)
                      {
                           Text.SetActive(true);
                           text[currentText].SetActive(true);
                           if (Input.GetKeyUp(KeyCode.Space))
                           {
                                text[currentText].SetActive(false);
                                currentText++;
                                if (currentText == 13)
                                Text.SetActive(false);
                           }
                      }

                }


            }
        }
    }

    
    
}
