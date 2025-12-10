using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar_UI : MonoBehaviour//玩家使用的血条脚本，挂载在摄像机CM vcam1上面
{
    [SerializeField] private Entity entity;//获取实体
    [SerializeField]private CharacterStats myStats;//获取数据
    private RectTransform rectTransform;//用来保持血条不翻转
    private Slider slider;


    private void Start()
    {
        slider=GetComponentInChildren<Slider>();
        rectTransform = GetComponent<RectTransform>();
        

        
        myStats.onHealthChanged += UpdateHealthUI;//订阅血条更新事件

       
    }

    private void Update()
    {
        UpdateHealthUI();
    }


    private void UpdateHealthUI()//血条更新函数
    {
        slider.maxValue =myStats.GetMaxHealthValue();//血条最大值
        slider.value = myStats.currentHealth;//血条当前值

       
    }



    

    private void OnEnable()
    {
        
    }

    private void OnDisable()//取消订阅
    {
        
        myStats.onHealthChanged-=UpdateHealthUI;
    }
}
