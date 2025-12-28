using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class manToEnemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;      // 玩家 Transform
    public Transform trigger1;     // 触发器 Transform1
    public Transform trigger2;     //
    public GameObject Door;       // 要开启的门

    [Header("Trigger")]
    public float triggerRadius = 3f;

    [Header("ds Settings")]
    //要消失的对象
    [SerializeField] private GameObject[] objectsToDestroy;
    //要显示的对象
    [SerializeField] private GameObject[] objectsToShow;
    //要第二次摧毁的对象
    [SerializeField] private GameObject[] objectsToDestroy2;

    [Header("image")]
    [SerializeField] private Sprite image1;// 要替换的图片
    [SerializeField] private Sprite image2;
    public SpriteRenderer imageO1; 
    public SpriteRenderer imageO2;
    // 3.9 3.5 2.5
    // -2.3 -2 -1.5
    private bool hasTriggered1 = false;
    private bool hasTriggered2 = false;

    MonoBehaviour DoorC;
    [Header("Text")]
    [SerializeField] private GameObject TextButton;//控制对话框的激活与失活
    [SerializeField] private GameObject[] texts;
    private int currentText = 0;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看

    private void Start()
    {
        DoorC = Door.GetComponent<ToAbyssPP>();

    }

    private void Update()
    {
        if(player == null) return;
        if (!hasTriggered1)
        {
            float distance = Vector3.Distance(player.position, trigger1.position);
            if (distance <= triggerRadius)
            {
                foreach (GameObject obj in objectsToDestroy)
                {
                    Destroy(obj);
                }
                foreach (GameObject obj in objectsToShow)
                {
                    obj.SetActive(true);
                }
                hasTriggered1 = true;
                imageO1.sprite = image1;
                imageO1.transform.localScale = new Vector3(-2.3f, -2f, -1.5f);
                imageO2.sprite = image2;
                imageO2.transform.localScale = new Vector3(3.9f, 3.5f, 2.5f);
            }
        }

        if (hasTriggered1)
        {
            if (AllDie())
            {
                foreach (GameObject obj in objectsToDestroy2)
                {
                    Destroy(obj);
                }
                if(currentText < texts.Length)
                {
                    TextButton.SetActive(true);
                    texts[currentText].SetActive(true);
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        texts[currentText].SetActive(false);
                        currentText++;
                        if (currentText == texts.Length)
                        {
                            TextButton.SetActive(false);
                        }
                    }
                }
            }
        }
        if(hasTriggered1  && !hasTriggered2)
        {
            float distance =Vector3.Distance(player.position, trigger2.position);
            if(distance <= triggerRadius)
            {
                DoorC.enabled = true;
                hasTriggered2 = true;
            }
        }
    }

    private bool AllDie()
    {
        bool allDead = true;
        foreach (GameObject obj in objectsToShow)
        {
            // 检查对象是否已被摧毁
            if (obj!= null)
            {
                allDead = false;
                break;
            }
        }
        return allDead;
    }
}
