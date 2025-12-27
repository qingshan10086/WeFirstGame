using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Level1 : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;//获取所有敌人，将敌人全部消灭才可前往下一个场景
    private bool allDefeated = true;//检测敌人是否全部被消灭
    private Animator anim;

    [SerializeField] private Transform player;//获取玩家位置
    [SerializeField] private float goalRaidus;//检测玩家是否到达的半径
    public OneTimeMechanism oneTimeMechanism;//要删除

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        oneTimeMechanism = GetComponent<OneTimeMechanism>();//要删除
    }

    // Update is called once per frame
    void Update()
    {
        IsAllDefeated();
        GotoNextLevel();
    }

    private void GotoNextLevel()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < goalRaidus && allDefeated)
        {
            SceneManager.LoadScene("Level2");
        }
    }

    private void IsAllDefeated()//检测敌人是否全部被消灭
    {
        allDefeated = true;//检测敌人是否全部被消灭
        foreach (GameObject enemyObj in enemy)
        {
            if (enemyObj != null)
            {
                allDefeated = false;
                return;
            }
        }
        if (allDefeated)
        {
            oneTimeMechanism.TriggerMechanism();//要删除
            anim.SetBool("Open", true);
        }


    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, goalRaidus);
    }




}
