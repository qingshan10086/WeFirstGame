using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveAndLoadButton : MonoBehaviour
{
    [Header("UI按钮引用")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button newGameButton;

    [Header("设置")]
    [SerializeField] private string defaultNewGameScene = "NewUserGuider";
    [SerializeField] private float loadTimeout = 5f;
    [SerializeField] private float sceneLoadBuffer = 0.5f; // 场景加载后额外等待时间

    private void Start()
    {
        //绑定按钮事件
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGame);
        }
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadGame);
        }
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(StartNewGame);
        }

        //订阅存档事件
        SaveManager.OnGameSaved += OnGameSaved;
        SaveManager.OnGameLoaded += OnGameLoaded;
    }


    private void OnDestroy()
    {
        SaveManager.OnGameSaved -= OnGameSaved;
        SaveManager.OnGameLoaded -= OnGameLoaded;
    }



    public void SaveGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("存档管理器未初始化");
            return;
        }

        if (PlayerManager.instance == null ||
            PlayerManager.instance.player == null ||
            PlayerManager.instance.playerStats == null)
        {
            Debug.LogError("PlayerManager组件不完整");
            return;
        }


        SaveManager.Instance.UpdateGameScene(SceneManager.GetActiveScene().name);
        SaveManager.Instance.UpdatePlayerPosition(PlayerManager.instance.player.transform.position);
        SaveManager.Instance.UpdatePlayerHealth(PlayerManager.instance.playerStats.currentHealth);

        SaveManager.Instance.SaveGame();
        Debug.Log("游戏已保存，玩家数据和机关状态已保存");
    }



    /// <summary>
    /// 加载游戏
    /// 从存档中恢复玩家数据和机关状态
    /// 核心流程：
    /// 1. 加载存档数据
    /// 2. 如果需要，切换场景
    /// 3. 恢复玩家位置和生命值
    /// 4. 机关会自动从存档加载状态
    /// </summary>
    public void LoadGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("存档管理器未初始化");
            return;
        }

        if (!SaveManager.Instance.SaveExists())
        {
            Debug.LogWarning("没有找到存档文件");
            return;
        }

        StartCoroutine(LoadGameCoroutine());
    }

    /// <summary>
    /// 开始新游戏
    /// 删除旧存档，创建新存档
    /// </summary>
    public void StartNewGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("存档管理器未初始化");
            return;
        }

        // 删除旧存档
        SaveManager.Instance.DeleteSave();

        // 创建新存档
        SaveManager.Instance.CreateNewSave();

        // 加载新游戏场景
        SceneManager.LoadScene(defaultNewGameScene);

        Debug.Log("开始新游戏");
    }


    /// <summary>
    /// 加载游戏的协程
    /// 处理场景切换和玩家状态恢复
    /// </summary>
    private IEnumerator LoadGameCoroutine()
    {
        Debug.Log("开始加载游戏...");

        // 1. 加载存档数据
        bool loadSuccess = SaveManager.Instance.LoadGame();
        if (!loadSuccess)
        {
            Debug.LogError("加载存档数据失败");
            yield break;
        }

        // 2. 获取存档中的玩家数据
        PlayData playData = SaveManager.Instance.GetPlayData();
        if (playData == null)
        {
            Debug.LogError("获取玩家数据失败");
            yield break;
        }

        Debug.Log($"存档信息 - 场景: {playData.sceneName}, 生命值: {playData.currentHealth}");

        // 3. 检查是否需要切换场景
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != playData.sceneName)
        {
            Debug.Log($"切换场景: {currentScene} -> {playData.sceneName}");
            //切换场景前将数据存储在临时变量中
            Vector3 savedPosition = new Vector3(playData.xPosition, playData.yPosition, playData.zPosition);
            int savedHealth = playData.currentHealth;
            // 异步加载场景
            SceneManager.LoadScene(playData.sceneName);
            // 等待场景加载完成
            while (SceneManager.GetActiveScene().name != playData.sceneName)
            {
                yield return null;
            }

            // 等待场景完全初始化
            yield return new WaitForSeconds(sceneLoadBuffer);
            // 4. 等待玩家实例化
            yield return StartCoroutine(WaitForPlayerCoroutine()); 

            // 5. 应用存档数据到玩家
            ApplyLoadedDataFromSave(savedPosition, savedHealth);
            // 重新加载存档，确保机关状态正确
            SaveManager.Instance.LoadGame();
        }
        else
        {
            // 如果已经在目标场景，直接应用数据
            yield return StartCoroutine(WaitForPlayerCoroutine());
            ApplyLoadedData(playData);
        }

        Debug.Log("游戏加载完成，机关状态已从存档自动恢复");
    }


    /// <summary>
    /// 等待玩家实例化的协程
    /// </summary>
    private IEnumerator WaitForPlayerCoroutine()
    {
        Debug.Log("等待玩家实例化...");

        float timer = 0f;

        // 等待PlayerManager实例
        while (PlayerManager.instance == null)
        {
            timer += Time.deltaTime;
            if (timer > loadTimeout)
            {
                Debug.LogError($"等待PlayerManager超时 ({loadTimeout}秒)");
                yield break;
            }
            yield return null;
        }

        // 等待玩家组件
        while (PlayerManager.instance.player == null ||
               PlayerManager.instance.playerStats == null)
        {
            timer += Time.deltaTime;
            if (timer > loadTimeout)
            {
                Debug.LogError($"等待玩家组件超时 ({loadTimeout}秒)");
                yield break;
            }
            yield return null;
        }

        Debug.Log("玩家实例化完成");
    }



    /// <summary>
    /// 应用加载的数据到玩家
    /// </summary>
    public void ApplyLoadedData(PlayData data)
    {
        if (PlayerManager.instance == null ||
            PlayerManager.instance.player == null ||
            PlayerManager.instance.playerStats == null)
        {
            Debug.LogError("玩家组件不完整，无法应用存档数据");
            return;
        }

        // 恢复玩家位置
        Vector3 savedPosition = new Vector3(data.xPosition, data.yPosition, data.zPosition);
        PlayerManager.instance.player.transform.position = savedPosition;

        // 恢复玩家生命值
        PlayerManager.instance.playerStats.currentHealth = data.currentHealth;

        Debug.Log($"玩家状态已恢复 - 位置: {savedPosition}, 生命值: {data.currentHealth}");
    }
    /// <summary>
    /// 从存档数据应用加载的数据到玩家（使用已保存的临时数据）
    /// </summary>
    private void ApplyLoadedDataFromSave(Vector3 position, int health)
    {
        if (PlayerManager.instance == null ||
            PlayerManager.instance.player == null ||
            PlayerManager.instance.playerStats == null)
        {
            Debug.LogError("玩家组件不完整，无法应用存档数据");
            return;
        }


        // 恢复玩家位置
        if (position == Vector3.zero)
        {
            Debug.LogWarning("存档中的位置是Vector3.zero，可能没有正确保存位置");
        }
        else
        {
            // 应用位置
            PlayerManager.instance.player.transform.position = position;
            Debug.Log($"玩家位置已恢复 - 位置: {position}");
        }


        // 恢复玩家生命值
        PlayerManager.instance.playerStats.currentHealth = health;
        Debug.Log($"玩家生命值已恢复 - 生命值: {health}");

        // 调试：输出玩家的当前实际位置
        Debug.Log($"玩家当前实际位置: {PlayerManager.instance.player.transform.position}");
    }

    /// <summary>
    /// 游戏保存完成事件
    /// </summary>
    private void OnGameSaved(GameSaveData data)
    {
        if (data != null)
        {
            int minutes = (int)(data.playTime / 60);
            int seconds = (int)(data.playTime % 60);

            Debug.Log($"游戏保存成功\n" +
                     $"存档名: {data.saveName}\n" +
                     $"保存时间: {data.saveTime:yyyy-MM-dd HH:mm:ss}\n" +
                     $"游戏时间: {minutes:00}:{seconds:00}\n" +
                     $"当前关卡: {data.currentLevel}");
        }
    }

    /// <summary>
    /// 游戏加载完成事件
    /// </summary>
    private void OnGameLoaded(GameSaveData data)
    {
        if (data != null)
        {
            Debug.Log($"游戏加载成功\n" +
                     $"存档名: {data.saveName}\n" +
                     $"当前场景: {data.PlayData.sceneName}");
        }
    }



}