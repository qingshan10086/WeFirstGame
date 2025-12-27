using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    private MonsterDeathDetector monsterDeathDetector;
    public OneTimeMechanism oneTimeMechanism;
    public int openNum = 4;
    // Start is called before the first frame update
    void Start()
    {
        monsterDeathDetector = GetComponentInParent<MonsterDeathDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        if(monsterDeathDetector.GetDeadMonsterCount()>=openNum)
        {
            gameObject.SetActive(false);
            Debug.Log("所有怪物死亡，门开启");
            // gameObject.GetComponent<SpriteRenderer>().sprite = openDoorSprite;
            if(oneTimeMechanism != null)
            {
                oneTimeMechanism.TriggerMechanism();
            }
        }
        else if(oneTimeMechanism != null && oneTimeMechanism.isTriggered)
        {
            gameObject.SetActive(false);
        }
    }
}
