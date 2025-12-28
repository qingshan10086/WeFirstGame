using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Abyss4 : MonoBehaviour
{
    public PlayableDirector director;
    [Header("References")]
    public Transform player;      // 玩家 Transform
    public Transform trigger1;     // 触发器 Transform1
    public Transform triggerText;  
    public GameObject Door;       // 要开启的门

    [Header("Trigger")]
    public float triggerRadius = 3f;
    public bool cantriggerNext = false;

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
    [SerializeField] private GameObject[] texts2;
    private int currentText = 0;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    private int currentText2 = 0;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    private bool cantrigger2 = false;

    private MonoBehaviour MusicTrigger;
    private void Start()
    {
        DoorC = Door.GetComponent<ToEcho3>();
        MusicTrigger = GetComponent<MusicTrigger>();
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
                cantriggerNext = true;
                imageO1.sprite = image1;
                imageO1.transform.localScale = new Vector3(-2.3f, -2f, -1.5f);
                imageO2.sprite = image2;
                imageO2.transform.localScale = new Vector3(3.9f, 3.5f, 2.5f);

            }
        }
        if (cantriggerNext)
        {
            float distance = Vector3.Distance(player.position, triggerText.position);
            if (distance <= triggerRadius)
            {
                DoorC.enabled = true;
                MusicTrigger.enabled = true;
                TextButton.SetActive(true);
                texts[currentText].SetActive(true);

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    texts[currentText].SetActive(false);
                    currentText++;
                    if (currentText == texts.Length)
                    {
                        TextButton.SetActive(false);
                        cantriggerNext = false;
                        hasTriggered2 = true;
                        director.Play();
                        StartCoroutine(ActivateTextAfterDelay(20f));
                    }
                }
            }
        }
        if(cantrigger2)
        {
            TextButton.SetActive(true);
            texts2[currentText2].SetActive(true);

            if (Input.GetKeyUp(KeyCode.Space))
            {
                texts2[currentText2].SetActive(false);
                currentText2++;
                if (currentText2 == texts2.Length)
                {
                    TextButton.SetActive(false);
                    cantrigger2 = false;
                }
            }
        }
        
    }
    private IEnumerator ActivateTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cantrigger2 = true;
        
    }


}
