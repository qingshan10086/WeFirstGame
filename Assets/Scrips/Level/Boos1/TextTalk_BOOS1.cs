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
    private int currentText = 0;

    private float currenntText;//记录当前是哪个文本
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

       
            if (currentText <= 6)
            {
                text[currentText].SetActive(true);

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText].SetActive(false);
                    currentText++;
                    if (currentText == 6)
                    {
                    Guider.SetActive(false);
                    }
                    if (currentText == 7)
                    {
                        Text.SetActive(false);
                        
                    }
                }
            }

            if (BOSS.dieState.canNextText)
            {
                if (currentText <= 18)
                {
                    Text.SetActive(true);
                    text[currentText].SetActive(true);

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        text[currentText].SetActive(false);
                        currentText++;
                        if (currentText == 8)
                        {
                            Guider.SetActive(true);
                            Guider.transform.position=new Vector2(player.transform.position.x+5,player.transform.position.y);
                        }
                        if (currentText == 19)
                        {
                            Door.SetActive(true);
                            Text.SetActive(false);
                        }
                    }
                }
            }


    }
}


