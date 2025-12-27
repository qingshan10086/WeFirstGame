using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
    public GameObject Guider;
    public Boss_Death Boss;
    public Animator animator;
    private bool isDead = false;
    private bool iskill = false;
    public GameObject timelineAnim;


    [Tooltip("如果为 true，会直接 Destroy 小怪（作为兜底）。通常建议先通过 SendMessage 让小怪自行死亡动画/逻辑处理。")]
    public bool forceDestroyAsFallback = true;

    public PlayableDirector timeline;
    [SerializeField] private GameObject player;//通过拖拽获取玩家信息
    [SerializeField] private GameObject TextButton;//控制对话框的激活与失活
    [SerializeField] private GameObject[] texts;
    private int currentText = 0;//记录当前是哪段文本,其分成几个部分，每部分的初始文本值不同，根据你在text中拖拽的来看
    private bool hasTrigger = false;
    private bool textActive= false;

    

    private void Update()
    {
        if(!hasTrigger && textActive)
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
                    PlayTimeline();
                }
            }
        }
        if (!iskill)
        {
            if (Boss.stats.currentHealth <= Boss.stats.GetMaxHealthValue() * 0.15f)
            {
                KillAllMinions();
                iskill = true;
            }
        }
        if(isDead) return;
        if (!isDead)
        {
            if (Boss.stats.currentHealth <= 0f)
            {
                KillAllMinions();
                isDead = true;
                textActive = true;
                Guider.SetActive(true);
                AudioManager.instance.PlayBGM(4);
            }
        }
    }
    void PlayTimeline()
    {
        // ④ 彻底停用 Animator，防止冲突
        if (animator != null)
            animator.enabled = false;

        timelineAnim.SetActive(true);
        // ⑤ 播放 Timeline
        timeline.gameObject.SetActive(true);
        timeline.enabled = true;
        timeline.time = 0;
        timeline.Play();
        StartCoroutine(ToEcho2(6f));
    }
    // 查找所有带 minionTag 的物体，优先发送 Die / TakeDamage，让小怪脚本自己处理死亡；
    // 如果小怪没有响应，则根据 forceDestroyAsFallback 决定是否直接 Destroy。
    public void KillAllMinions()
    {

        Enemy[] enemys;

        enemys = GameObject.FindObjectsOfType<Enemy>();

        foreach (var e in enemys)
        {
            if (e == null) continue;

            e.stats.TakeDamage(200);
            // 兜底：如果需要直接销毁
            if (forceDestroyAsFallback)
            {
                // 如果对象仍然存在且没有被小怪自身销毁，则销毁它
                if (e != null)
                    Destroy(e);
            }
        }
    }
    private IEnumerator ToEcho2(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Echo2");
    }
}
