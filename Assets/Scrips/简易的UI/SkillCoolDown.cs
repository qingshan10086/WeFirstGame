using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image skillIcon;  // 技能图标
    [SerializeField] private Text cooldownText; // 冷却时间文本


    [Header("冷却设置")]
    [SerializeField] private float cooldownDuration = 5f; // 冷却总时长
    [SerializeField] private KeyCode useKey = KeyCode.Q; // 使用按键


    [Header("颜色设置")]
    [SerializeField] private Color readyColor = Color.white; // 就绪状态颜色
    [SerializeField] private Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 冷却中颜色


    private float currentCooldown = 0f;
    private bool isCooling = false;


    private void Update()
    {
        // 按键检测
        if (Input.GetKeyDown(useKey) && !isCooling)
        {
            StartCooldown();
        }


        // 冷却更新
        if (isCooling)
        {
            UpdateCooldown();
        }
    }


    private void StartCooldown()
    {
        isCooling = true;
        currentCooldown = cooldownDuration;
        UpdateUI();
    }


    private void UpdateCooldown()
    {
        currentCooldown -= Time.deltaTime;

        if (currentCooldown <= 0f)
        {
            currentCooldown = 0f;
            isCooling = false;
        }

        UpdateUI();
    }


    private void UpdateUI()
    {
        // 更新图标颜色
        skillIcon.color = isCooling ? cooldownColor : readyColor;

        // 更新冷却文本
        if (cooldownText != null)
        {
            cooldownText.text = isCooling ? Mathf.Ceil(currentCooldown).ToString() : "";
        }
    }


    // 外部调用方法
    public void TriggerCooldown()
    {
        if (!isCooling) StartCooldown();
    }


    public bool IsSkillReady()
    {
        return !isCooling;
    }
}

