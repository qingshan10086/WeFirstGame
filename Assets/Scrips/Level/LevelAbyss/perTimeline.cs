using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class perTimeline : MonoBehaviour
{
    public Transform MusicTrigger;
    public float triggerR = 3f;
    private bool isPlaying = false;

    public PlayableDirector director;
    public GameObject Text;
    public GameObject Redtext;

    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject TextButton;//控制对话框的激活与失活
    [SerializeField] private GameObject[] texts;
    private int currentText;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    private bool hasTrigger;
    private bool textActive;

    private bool played = false;
    private void Update()
    {
        if (!isPlaying)
        {
            float distance = Vector3.Distance(player.transform.position, MusicTrigger.position);
            if (distance <= triggerR)
            {
                AudioManager.instance.PlayBGM(5);
                isPlaying = true;
            }
        }

        if (!hasTrigger && textActive)
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
                    hasTrigger = true;
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
            Debug.Log("Playing");
            director.Play();
        }
        StartCoroutine(ActivateTextAfterDelay(26f));
    }
    private IEnumerator ActivateTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(1f);
        Text.SetActive(true);
        Text.GetComponent<Image>().enabled = false;
        Text.GetComponentInChildren<Text>().enabled = false;
        yield return new WaitForSeconds(delay);
        Redtext.SetActive(true);
        Text.GetComponentInChildren<Text>().enabled = true;
        Text.GetComponent<Image>().enabled = true;
        Text.SetActive(false);
        yield return new WaitForSeconds(4f);
        textActive = true;
    }
    
}
