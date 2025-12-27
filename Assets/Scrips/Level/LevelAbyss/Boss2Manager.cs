using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Playables;

public class Boss2Manager : MonoBehaviour
{
    public PlayableDirector director;
    public Boss_Death Boss;
    public GameObject TextButton;
    public GameObject[] texts;
    private int currentText = 0;
    private bool textActive = false;

    private bool iskill = false;
    private bool isDead = false;

    private void Update()
    {
        if (textActive)
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
                    textActive = false;
                    director.Play();
                    StartCoroutine(PlayBGMAfter(20f));
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
        if (isDead) return;
        if (!isDead)
        {
            if (Boss.stats.currentHealth <= 0f)
            {
                KillAllMinions();
                isDead = true;
                textActive = true;
                AudioManager.instance.PlayBGM(4);
            }
        }
    }
    public void KillAllMinions()
    {

        Enemy[] enemys;

        enemys = GameObject.FindObjectsOfType<Enemy>();

        foreach (var e in enemys)
        {
            if (e == null) continue;

            e.stats.TakeDamage(200);
        }
    }
    private IEnumerator PlayBGMAfter(float _time)
    {
        yield return new WaitForSeconds(_time);
        AudioManager.instance.PlayBGM(5);
    }
}
