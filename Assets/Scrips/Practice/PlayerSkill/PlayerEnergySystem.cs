using System.Configuration.Assemblies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 玩家能量系统
/// 管理三级能量状态，当能量达到阈值时触发事件
/// </summary>
public class PlayerEnergySystem : MonoBehaviour
{
    // 能量等级枚举
    public enum EnergyLevel
    {
        Level1 = 1,  // 第一级
        Level2 = 2,  // 第二级
        Level3 = 3,   // 第三级
        Level4 = 4  // 第四级
    }

    // 能量事件定义
    [Header("能量事件")]
    public UnityEvent<EnergyLevel> OnEnergyLevelUp = new UnityEvent<EnergyLevel>();    // 能量升级事件
    public UnityEvent<EnergyLevel> OnEnergyLevelDown = new UnityEvent<EnergyLevel>();  // 能量降级事件
    public UnityEvent<float> OnEnergyChanged = new UnityEvent<float>();                // 能量值变化事件
    [Header("能量可视化")]
    public Slider energySlider;
    // 能量设置
    [Header("能量设置")]
    [Tooltip("当前能量值")]
    [Range(0f, 1f)]
    public float currentEnergy = 0f;

    [Header("能量阈值")]
    [Tooltip("升级到第二级所需的能量阈值")]
    [Range(0f, 1f)]
    public float level2Threshold = 0.3f;

    [Tooltip("升级到第三级所需的能量阈值")]
    [Range(0f, 1f)]
    public float level3Threshold = 0.6f;

    [Tooltip("升级到第四级所需的能量阈值")]
    [Range(0f, 1f)]
    public float level4Threshold = 1.0f; // 满能量时进入第四级

    [Tooltip("降级到第一级所需的能量阈值")]
    [Range(0f, 1f)]
    public float level1Threshold = 0.2f;

    [Tooltip("降级到第二级所需的能量阈值")]
    [Range(0f, 1f)]
    public float level2DownThreshold = 0.5f;

    [Tooltip("降级到第三级所需的能量阈值")]
    [Range(0f, 1f)]
    public float level3DownThreshold = 0.9f;

    [Header("能量损失")]
    public float energyLossRate = 0.1f;
    public float lossTimer = 0f;
    public float lossTimerMax = 5f;
    public float lossFrequency = 1f;
    public float lossFrequencyTimer = 1f;
    // 当前能量等级
    [Header("当前状态")]
    [ReadOnlyInspector]
    public EnergyLevel currentLevel = EnergyLevel.Level1;

    [Header("能量图")]
    public Image energySliderImage;
    [Header("是否开启技能系统")]
    public bool isSkillEnabled = true;
    // 私有变量
    private EnergyLevel previousLevel = EnergyLevel.Level1;
    private void Start()
    {
        OnEnergyLevelUp.AddListener(HandleEnergyLevelUp);
        energySlider.maxValue = 1.0f;
    }
    /// <summary>
    /// 设置当前能量值
    /// 由外部调用，用于增加或减少能量
    /// </summary>
    /// <param name="amount">要设置的能量值（0-1之间）</param>
    public void SetEnergy(float amount)
    {
        // 限制能量值在0-1之间
        float newEnergy = Mathf.Clamp01(amount);
        
        // 如果能量值没有变化，直接返回
        if (Mathf.Approximately(currentEnergy, newEnergy))
            return;

        // 更新当前能量值
        currentEnergy = newEnergy;
        
        // 触发能量值变化事件
        OnEnergyChanged.Invoke(currentEnergy);
        
        // 检查能量等级变化
        CheckEnergyLevelChange();
    }
    
    private void Update()
    {
        CheckEnergyLevelChange();
        lossTimer -= Time.deltaTime;
        lossFrequencyTimer -= Time.deltaTime;
        if (lossTimer <0f)
        {
            if(lossFrequencyTimer <0f)
            {
                lossFrequencyTimer = lossFrequency;
                if(currentEnergy > 0.0f)
                {
                    ReduceEnergy(energyLossRate);
                }
            }
        }
        energySlider.value = currentEnergy;
       
    }



    /// <summary>
    /// 增加能量值
    /// 由外部调用
    /// </summary>
    /// <param name="amount">要增加的能量值（0-1之间）</param>
    public void AddEnergy(float amount)
    {
        SetEnergy(currentEnergy + amount);
    }

    /// <summary>
    /// 减少能量值
    /// 由外部调用
    /// </summary>
    /// <param name="amount">要减少的能量值（0-1之间）</param>
    public void ReduceEnergy(float amount)
    {
        SetEnergy(currentEnergy - amount);
    }

    /// <summary>
    /// 检查能量等级变化
    /// </summary>
    private void CheckEnergyLevelChange()
    {
        // 确定当前应该处于的能量等级
        EnergyLevel newLevel = DetermineCurrentLevel();
        
        // 如果等级没有变化，直接返回
        if (newLevel == currentLevel)
            return;

        // 记录之前的等级
        previousLevel = currentLevel;
        
        // 更新当前等级
        currentLevel = newLevel;
        if(currentLevel == EnergyLevel.Level1)
        {
           energySliderImage.color = Color.white;
        }
        else if(currentLevel == EnergyLevel.Level2)
        {
           energySliderImage.color = Color.yellow;
        }
        else if(currentLevel == EnergyLevel.Level3)
        {
            energySliderImage.color = Color.red;
        }
        else if(currentLevel == EnergyLevel.Level4)
        {
            energySliderImage.color = Color.magenta; // 第四级使用洋红色
        }
        
        // 触发相应的事件
        if (newLevel > previousLevel)
        {
            // 升级事件
            OnEnergyLevelUp.Invoke(newLevel);
            Debug.Log($"Energy level up to {newLevel}");
        }
        else
        {
            // 降级事件
            OnEnergyLevelDown.Invoke(newLevel);
            Debug.Log($"Energy level down to {newLevel}");
        }
    }

    /// <summary>
    /// 根据当前能量值确定能量等级
    /// </summary>
    /// <returns>当前能量等级</returns>
    private EnergyLevel DetermineCurrentLevel()
    {
        if (currentEnergy >= level4Threshold)
        {
            return EnergyLevel.Level4;
        }
        else if (currentEnergy >= level3Threshold)
        {
            return EnergyLevel.Level3;
        }
        else if (currentEnergy >= level2Threshold)
        {
            return EnergyLevel.Level2;
        }
        else if (currentEnergy <= level1Threshold && currentLevel > EnergyLevel.Level1)
        {
            return EnergyLevel.Level1;
        }
        else if (currentEnergy <= level2DownThreshold && currentLevel > EnergyLevel.Level2)
        {
            return EnergyLevel.Level2;
        }
        else if (currentEnergy <= level3DownThreshold && currentLevel > EnergyLevel.Level3)
        {
            return EnergyLevel.Level3;
        }
        
        // 默认返回当前等级
        return currentLevel;
    }

    /// <summary>
    /// 获取当前能量等级
    /// </summary>
    /// <returns>当前能量等级</returns>
    public EnergyLevel GetCurrentEnergyLevel()
    {
        return currentLevel;
    }
public void HandleEnergyLevelUp(EnergyLevel level)
{
    switch(level)
    {
        case EnergyLevel.Level2:
            // 升级到等级2的逻辑
            break;
        case EnergyLevel.Level3:
            // 升级到等级3的逻辑
            break;
        case EnergyLevel.Level4:
            // 升级到等级4的逻辑
            Debug.Log("Player reached maximum energy level! Special abilities unlocked!");
            break;
    }
}
    /// <summary>
    /// 获取当前能量值
    /// </summary>
    /// <returns>当前能量值（0-1）</returns>
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

}

/// <summary>
/// 只读Inspector属性
/// </summary>
public class ReadOnlyInspector : PropertyAttribute { }
