using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTalk_Level2 : MonoBehaviour
{

    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活


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
        if (player.transform.position.x < 16&& player.transform.position.x > 14)
        {
            if (currentText <= 4)
            {
                text[currentText].SetActive(true);

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    text[currentText].SetActive(false);
                    currentText++;
                    if (currentText == 5)
                    {
                        Text.SetActive(false);

                    }
                }
            }

        }




    }
}
