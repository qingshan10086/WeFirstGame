using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;//获取所有敌人，将敌人全部消灭才可前往下一个场景
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject enemyObj in enemy)
        {
            if (enemyObj == null)
            {
                anim.SetBool("Open", true);
            }
        }
    }
}
