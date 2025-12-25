using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class BackGroundLoop : MonoBehaviour
{
    [Header("图片设置")]
    [SerializeField] private GameObject[] allImage;//全部图片

    private float loopCooldown=2f;//每张图片循环时间
    private float loopTimer=2f;//循环时间辅助器

    private int currentImage=0;//当前图片
    private int maxImage=3;//最大图片数量

    // Start is called before the first frame update
    void Start()
    {
        allImage[currentImage].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        loopTimer-=Time.deltaTime;

        if (loopTimer < 0)
        {
            allImage[currentImage].SetActive(false);
            currentImage = (currentImage +1) % maxImage;
            allImage[currentImage].SetActive(true);
            loopTimer=loopCooldown;
        }

    }
}
