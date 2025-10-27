using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour  //光效类，用来完成受到攻击变色之类的
{
    private SpriteRenderer sr;//精灵渲染器组件

    [Header("Flash FX")]//闪光效果
    [SerializeField] private float flashDuration;  //闪光持续时间
    [SerializeField] private Material hitMat;     //击打材质
    private Material originalMat;               //初始材质

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();//获取子物体上的精灵渲染器组件
        originalMat = sr.material;                      //获取初始子物体材质
    }

    private IEnumerator FlashFX()       //闪光协程
    {
        sr.material = hitMat;//变成击打材质

        yield return new WaitForSeconds(flashDuration);//持续闪光

        sr.material = originalMat;//恢复
    }

    private void RedColorBlink()//变红协程
    {
        if(sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }


    private void CancelRedBlink()//取消变红函数
    {
        CancelInvoke();
        sr.color=Color.white;
    }
}
