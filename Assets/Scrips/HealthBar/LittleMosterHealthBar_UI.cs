using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleMosterHealthBar_UI : MonoBehaviour//小怪挂载在血条上面的脚本
{
    private Entity entity;//获取实体
    private CharacterStats myStats;//获取数据
    private RectTransform rectTransform;//用来保持血条不翻转
    private Slider slider;


    private void Start()
    {
        slider=GetComponentInChildren<Slider>();
        rectTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;//订阅不翻转事件
        myStats.onHealthChanged += UpdateHealthUI;//订阅血条更新事件

       
    }


   


    private void UpdateHealthUI()//血条更新函数
    {
        slider.maxValue =myStats.GetMaxHealthValue();//血条最大值
        slider.value = myStats.currentHealth;//血条当前值

       
    }



    private void FlipUI()//防血条翻转
    {
        rectTransform.Rotate(0, 180, 0);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()//取消订阅
    {
        if (entity != null)
        {
            entity.onFlipped -= FlipUI;
        }

        if (myStats != null)
        {
            myStats.onHealthChanged -= UpdateHealthUI;
        }
    }
}
