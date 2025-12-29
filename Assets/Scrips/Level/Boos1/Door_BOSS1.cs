using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_BOSS1 : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private Transform player;//获取玩家位置
    [SerializeField] private float goalRaidus;//检测玩家是否到达的半径


    [SerializeField] private Door2_BOSS1 door2_BOSS1;//异步加载管理器

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        door2_BOSS1 = GetComponent<Door2_BOSS1>();
    }

    // Update is called once per frame
    void Update()
    {
        GotoNextLevel();
    }


    private void GotoNextLevel()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < goalRaidus + 5f)
        {
            anim.SetBool("Open", true);
        }

        if (distance < goalRaidus)
        {
            door2_BOSS1.LoadTargetScene("BloodFields");
         
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, goalRaidus);
    }

}
