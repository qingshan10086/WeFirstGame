using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform rectTransform;
    private Slider slider;


    private void Start()
    {
        slider=GetComponentInChildren<Slider>();
        rectTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;

       
    }


   


    private void UpdateHealthUI()
    {
        slider.maxValue =myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }



    private void FlipUI()
    {
        rectTransform.Rotate(0, 180, 0);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged-=UpdateHealthUI;
    }
}
