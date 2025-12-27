using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneHero : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        randomPlay();
    }

    private void randomPlay()
    {
        //生成一个1到7随机数
        int randomNum = Random.Range(1, 11);
        int finalNum;
        switch (randomNum)
        {
            case 1:
                finalNum = 1;
                break;
            case 2:
                finalNum = 2;
                break;
            case 3:
                finalNum = 3;
                break;
            case 4:
                finalNum = 4;
                break;
            case 5:
                finalNum = 1;
                break;
            case 6:
                finalNum = 1;
                break;
            case 7:
                finalNum = 2;
                break;
            case 8:
                finalNum = 5;
                break;
            case 9:
                finalNum = 6;
                break;
            case 10:
                finalNum = 7;
                break;

        }
        anim.SetInteger("int", randomNum);
    }
}
