using UnityEngine;

/// <summary>
/// 玩家能量系统使用示例
/// 展示如何与PlayerEnergySystem交互
/// </summary>
public class PlayerEnergyUsageExample : MonoBehaviour
{
    [Header("能量系统引用")]
    public PlayerEnergySystem energySystem;

    [Header("能量设置")]
    [Tooltip("每帧增加的能量值")]
    public float energyIncreasePerFrame = 0.01f;

    [Tooltip("使用技能消耗的能量值")]
    public float energyCostPerSkill = 0.3f;

    [Header("输入设置")]
    [Tooltip("使用技能的按键")]
    public KeyCode skillKey = KeyCode.Space;

    // 用于演示的技能状态
    private bool isSkillActive = false;

    private void Start()
    {
        // 确保能量系统已初始化
        if (energySystem == null)
        {
            energySystem = GetComponent<PlayerEnergySystem>();
            if (energySystem == null)
            {
                Debug.LogError("PlayerEnergySystem not found!");
                enabled = false;
                return;
            }
        }

        // 订阅能量等级变化事件
        SubscribeToEnergyEvents();
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        UnsubscribeFromEnergyEvents();
    }

    private void Update()
    {
        // 模拟能量增加（实际项目中应该由你的游戏逻辑控制）
        SimulateEnergyIncrease();

        // 技能使用逻辑
        HandleSkillInput();
    }

    /// <summary>
    /// 模拟能量增加
    /// 在实际项目中，你应该替换为自己的能量增加逻辑
    /// </summary>
    private void SimulateEnergyIncrease()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 按住Shift键增加能量
            energySystem.AddEnergy(energyIncreasePerFrame * Time.deltaTime * 10f);
        }
        else
        {
            // 正常情况下缓慢增加能量
            energySystem.AddEnergy(energyIncreasePerFrame * Time.deltaTime);
        }
    }

    /// <summary>
    /// 处理技能输入
    /// </summary>
    private void HandleSkillInput()
    {
        if (Input.GetKeyDown(skillKey))
        {
            UseSkill();
        }
    }

    /// <summary>
    /// 使用技能
    /// 在实际项目中，你应该根据当前能量等级实现不同的技能效果
    /// </summary>
    private void UseSkill()
    {
        // 检查当前能量等级
        switch (energySystem.currentLevel)
        {
            case PlayerEnergySystem.EnergyLevel.Level1:
                Debug.Log("当前能量等级低，无法使用特殊技能");
                break;

            case PlayerEnergySystem.EnergyLevel.Level2:
                Debug.Log("使用二级能量技能");
                // 消耗能量
                energySystem.ReduceEnergy(energyCostPerSkill * 1.5f);
                // 实现二级技能效果...
                break;

            case PlayerEnergySystem.EnergyLevel.Level3:
                Debug.Log("使用三级能量技能");
                // 消耗更多能量
                energySystem.ReduceEnergy(energyCostPerSkill * 2.5f);
                // 实现三级技能效果...
                break;
        }
    }

    /// <summary>
    /// 订阅能量事件
    /// </summary>
    private void SubscribeToEnergyEvents()
    {
        energySystem.OnEnergyLevelUp.AddListener(OnEnergyLevelUp);
        energySystem.OnEnergyLevelDown.AddListener(OnEnergyLevelDown);
        energySystem.OnEnergyChanged.AddListener(OnEnergyChanged);
    }

    /// <summary>
    /// 取消订阅能量事件
    /// </summary>
    private void UnsubscribeFromEnergyEvents()
    {
        energySystem.OnEnergyLevelUp.RemoveListener(OnEnergyLevelUp);
        energySystem.OnEnergyLevelDown.RemoveListener(OnEnergyLevelDown);
        energySystem.OnEnergyChanged.RemoveListener(OnEnergyChanged);
    }

    /// <summary>
    /// 能量等级提升时调用
    /// </summary>
    /// <param name="newLevel">新的能量等级</param>
    private void OnEnergyLevelUp(PlayerEnergySystem.EnergyLevel newLevel)
    {
        Debug.Log($"能量等级提升到: {newLevel}");

        // 根据能量等级启用不同的技能
        switch (newLevel)
        {
            case PlayerEnergySystem.EnergyLevel.Level2:
                // 启用二级技能
                Debug.Log("解锁二级技能");
                isSkillActive = true;
                break;

            case PlayerEnergySystem.EnergyLevel.Level3:
                // 启用三级技能
                Debug.Log("解锁三级技能");
                isSkillActive = true;
                break;
        }
    }

    /// <summary>
    /// 能量等级降低时调用
    /// </summary>
    /// <param name="newLevel">新的能量等级</param>
    private void OnEnergyLevelDown(PlayerEnergySystem.EnergyLevel newLevel)
    {
        Debug.Log($"能量等级降低到: {newLevel}");

        // 根据能量等级禁用相应的技能
        switch (newLevel)
        {
            case PlayerEnergySystem.EnergyLevel.Level1:
                // 禁用所有特殊技能
                Debug.Log("所有特殊技能已禁用");
                isSkillActive = false;
                break;

            case PlayerEnergySystem.EnergyLevel.Level2:
                // 禁用三级技能，保留二级技能
                Debug.Log("三级技能已禁用，保留二级技能");
                isSkillActive = true;
                break;
        }
    }

    /// <summary>
    /// 能量值变化时调用
    /// </summary>
    /// <param name="newEnergy">新的能量值</param>
    private void OnEnergyChanged(float newEnergy)
    {
        // 可以在这里更新UI显示
        // Debug.Log($"当前能量值: {newEnergy * 100f:F0}%");
    }
}
