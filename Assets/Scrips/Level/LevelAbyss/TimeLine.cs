using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLine : MonoBehaviour
{
    private OneTimeMechanism OneTimeMechanism;
    public PlayableDirector director;
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject Text;//控制对话框的激活与失活
    [SerializeField] private GameObject[] texts;
    [SerializeField] private GameObject SkillManager;
    private int currentText;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    private bool hasTrigger;
    private bool textActive;
    private MonoBehaviour skillO;

    private bool played = false;
    private void Start()
    {
        currentText = 0;
        hasTrigger = false;
        textActive = false;
        skillO = SkillManager.GetComponent<HeroKnightTransformSkill>();
        OneTimeMechanism = GetComponent<OneTimeMechanism>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (played) return;

        if (other.CompareTag("Player"))
        {
            played = true;
            Debug.Log("Playing");
            director.Play();
        }
        //10秒后激活对话框
        StartCoroutine(ActivateTextAfterDelay(10f));
    }
    private IEnumerator ActivateTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        textActive = true;
        skillO.enabled = true;
        OneTimeMechanism.TriggerMechanism();
    }
    
    private void Update()
    {
        if (OneTimeMechanism.isTriggered)
        {
            skillO.enabled = true;
        }
        if (!hasTrigger && textActive)
        {
            Debug.Log("2");
            Text.SetActive(true);
            texts[currentText].SetActive(true);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                texts[currentText].SetActive(false);
                currentText++;
                if (currentText == texts.Length)
                {
                    Text.SetActive(false);
                    hasTrigger = true;
                }
            }
        }
    }
}
