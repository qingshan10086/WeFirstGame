using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class timelineTrigger : MonoBehaviour
{
    public PlayableDirector director;
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject TextButton;//控制对话框的激活与失活
    [SerializeField] private GameObject[] texts;
    private int currentText=0;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    private bool hasTrigger;
    private bool textActive;

    private bool played = false;

    public Transform trigger;
    public float triggerRadius = 3f;
    private bool cantrigger2 = false;

    private void Update()
    {
        if (!hasTrigger && textActive)
        {
            TextButton.SetActive(true);
            texts[currentText].SetActive(true);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                texts[currentText].SetActive(false);
                currentText++;
                if (currentText == 2)
                {
                    TextButton.SetActive(false);
                    hasTrigger = true;
                }
            }
        }
        if (!cantrigger2 && hasTrigger)
        {
            float distance = Vector3.Distance(trigger.position, player.transform.position);
            if (distance <= triggerRadius)
            {
                cantrigger2 = true;
                AudioManager.instance.PlayBGM(4);
            }
        }
        if(cantrigger2 && hasTrigger)
        {
            if (currentText < texts.Length)
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
                        cantrigger2 = false;
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (played) return;

        if (other.CompareTag("Player"))
        {
            played = true;
            director.Play();
        }
        StartCoroutine(ActivateTextAfterDelay(5f));
    }
    private IEnumerator ActivateTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        textActive = true;
    }
}

