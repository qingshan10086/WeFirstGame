using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger: MonoBehaviour
{
    public Transform trigger;
    public Transform player;
    public float triggerRadius = 3f;

    public int index = 4;
    public bool isDelay = false;
    public float delayTime;

    private bool isTrigger = false;

    private void Update()
    {
        if (!isTrigger)
        {
            if (!isDelay)
            {
                float distance = Vector3.Distance(trigger.position, player.position);
                if (distance <= triggerRadius)
                {
                    AudioManager.instance.PlayBGM(index);
                }
                isTrigger = true;
            }
            else
            {
                float distance = Vector3.Distance(trigger.position, player.position);
                if (distance <= triggerRadius)
                {
                    StartCoroutine(ActivateAfterDelay(delayTime));
                }
                isTrigger = true;
            }
        }
    }
    private IEnumerator ActivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.PlayBGM(index);
    }
}
