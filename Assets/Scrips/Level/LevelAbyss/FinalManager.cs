using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FinalManager : MonoBehaviour
{
    public PlayableDirector director;
    public Transform TimelineTrigger;
    public Transform AudioTrigger;
    public Transform player;
    public float triggerRadius = 3f;
    private bool played = false;
    private bool replaced = false;

    private void Update()
    {
        if (!replaced)
        {
            float distance = Vector3.Distance(AudioTrigger.position, player.position);
            if (distance <= triggerRadius)
            {
                AudioManager.instance.PlayBGM(4);
                replaced = true;
            }
        }
        if (!played)
        {
            float distance = Vector3.Distance(TimelineTrigger.position, player.position);
            if(distance <= triggerRadius)
            {
                director.Play();
                played = true;
            }
        }
    }

}
