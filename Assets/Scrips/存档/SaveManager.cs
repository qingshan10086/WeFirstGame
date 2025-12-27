using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System;


/// <summary>
/// 存档管理器 - 核心单例类
/// 负责整个游戏的存档/加载逻辑，包括文件操作、加密、机关状态管理等
/// 使用单例模式确保全局唯一访问点
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("存档设置")]
    [SerializeField] private string saveFileName = "save";//存档文件名
    [SerializeField] private string saveFileExtension = ".sav";//存档文件扩展名
    [SerializeField] private bool autoSave = false;//是否启用自动存储，否
    [SerializeField] private float autoSaveInterval = 300f; //自动保存间隔时间（秒）
    [SerializeField] private bool saveOnSceneChange = false; //场景切换时是否自动保存


    private GameSaveData currentSaveData;//当前加载的存档数据
    private string saveFilePath;//存档文件路径
    
    private float autoSaveTimer = 0f; // <summary>自动保存计时器</summary>
    private bool isInitialized = false;

    /// <summary>
    /// 游戏保存完成事件
    /// </summary>
    public static event Action<GameSaveData> OnGameSaved;
    /// <summary>
    /// 游戏加载完成事件
    /// </summary>
    public static event Action<GameSaveData> OnGameLoaded;
    /// <summary>
    /// 初始化存档管理器
    /// </summary>
    private void Initialize()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName + saveFileExtension);
        
        bool hasSave=File.Exists(saveFilePath);//尝试加载现有存档
        if (hasSave)
        {   //加载现有存档
            if (!LoadGameInternal())
            {
                //失败就创建新存档
                CreateNewSave();
            }
        }
        else
        {   //没有存档文件，创建新存档
            CreateNewSave();
        }

        isInitialized = true;
        Debug.Log($"存档管理器初始化完成，存档路径:{saveFilePath}");


    }




    // Start is called before the first frame update
    void Start()
    {
        
        if (saveOnSceneChange)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (autoSave && currentSaveData != null)
        {
            //更新玩家场景信息
            currentSaveData.PlayData.sceneName = scene.name;
            SaveGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized || !autoSave || currentSaveData == null) return;

        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            SaveGame();
            autoSaveTimer = 0f;
        }

        // 更新游戏时间
        currentSaveData.playTime += Time.deltaTime;
    }

    private void OnDestroy()
    {
        if (saveOnSceneChange)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded-= OnSceneLoaded;
        }
    }


    /// <summary>
    /// 创建新存档
    /// </summary>

    public  void CreateNewSave()
    {
        if (PlayerManager.instance == null)
        {
            Debug.LogError("请把PlayerManager在ProjectSetting中放在SaveManager前");
            return;
        }
        
        currentSaveData = new GameSaveData("默认存档");
        currentSaveData.saveTime = DateTime.Now;

        currentSaveData.PlayData.xPosition = -1;
        currentSaveData.PlayData.yPosition = -3;
        currentSaveData.PlayData.zPosition = 0;
        currentSaveData.PlayData.currentHealth= PlayerManager.instance.playerStats.currentHealth;
        currentSaveData.PlayData.sceneName = "NewUserGuider";

        SaveGame();
        Debug.Log("创建新存档");
    }
    /// <summary>
    /// 保存游戏
    /// 将当前数据写入文件
    /// </summary>
    public void SaveGame()
    {
        if (currentSaveData == null)
        {
            Debug.Log("没有可保存的数据");
            return;
        }

        currentSaveData.saveTime = DateTime.Now;

        try
        {
            //序列化JSON
            string jsonData = JsonConvert.SerializeObject(currentSaveData, Formatting.Indented);
            //写入文件
            File.WriteAllText(saveFilePath, jsonData);

            Debug.Log($"游戏已保存：{saveFilePath}");
            OnGameSaved?.Invoke(currentSaveData);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存失败：{e.Message}");
        }
    }
    /// <summary>
    /// 加载游戏
    /// 从文件中读取数据
    /// </summary>
    /// <returns></returns>
    public bool LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError($"存档文件不存在：{saveFilePath}");
            return false;
        }

        return LoadGameInternal();
    }
    /// <summary>
    /// 内部加载游戏方法
    /// </summary>
    /// <returns></returns>
    private bool LoadGameInternal()
    {
        try
        {
            string jsonData = File.ReadAllText(saveFilePath);
            currentSaveData = JsonConvert.DeserializeObject<GameSaveData>(jsonData);

            if (currentSaveData == null)
            {
                Debug.LogError("存档数据为空");
                return false;
            }
            Debug.Log($"游戏已经加载：{saveFilePath}");
            OnGameLoaded?.Invoke(currentSaveData);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 删除存档文件
    /// 用于重新开始游戏
    /// </summary>
    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                currentSaveData = null;
                Debug.Log("存档已删除");
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"删除存档失败: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 检查存档是否存在
    /// </summary>
    public bool SaveExists()
    {
        return File.Exists(saveFilePath);
    }


    /// <summary>
    /// 标记机关已经触发
    /// </summary>
    /// <param name="mechanismID"></param>
    /// <param name="isGlobal"></param>
    public void MarkMechanismTriggered(string mechanismID,bool isGlobal = false)
    {
        if (currentSaveData == null) return;

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        currentSaveData.MarkMechanismTriggered(sceneName,mechanismID,isGlobal);

        if (autoSave)
        {
            SaveGame();
        }
    }
    /// <summary>
    /// 检查机关是否已触发
    /// </summary>
    /// <param name="mechanismID"></param>
    /// <param name="isGlobal"></param>
    /// <returns></returns>
    public bool IsMechanismTriggered(string mechanismID,bool isGlobal = false)
    {
        if(currentSaveData == null) return false;
        string sceneName=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        return currentSaveData.IsMechanismTriggered(sceneName,mechanismID,isGlobal);
    }
    /// <summary>
    /// 重置当前场景所有机关状态
    /// </summary>
    public void ResetCurrentSceneMechanisms()
    {
        if (currentSaveData == null) return;

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentSaveData.sceneStates.ContainsKey(sceneName))
        {
            currentSaveData.sceneStates[sceneName].triggeredMechanisms.Clear();
        }
    }

    #region 数据访问接口
    /// <summary>
    /// 获取当前存档数据
    /// </summary>
    /// <returns></returns>
    public GameSaveData GetCurrentSaveData()
    {
        return currentSaveData;
    }
    /// <summary>
    /// 获取玩家数据
    /// </summary>
    /// <returns></returns>
    public PlayData GetPlayData()
    {
        return currentSaveData?.PlayData;
    }
    public void SetPlayData(PlayData data)
    {
        if (currentSaveData != null)
        {
            currentSaveData.PlayData = data;
        }
    }
    /// <summary>
    /// 更新玩家位置
    /// </summary>
    public void UpdatePlayerPosition(Vector3 position)
    {
        if (currentSaveData?.PlayData != null)
        {
            currentSaveData.PlayData.xPosition = position.x;
            currentSaveData.PlayData.yPosition = position.y;
            currentSaveData.PlayData.zPosition = position.z;
        }
    }

    /// <summary>
    /// 更新玩家生命值
    /// </summary>
    public void UpdatePlayerHealth(int health)
    {
        if (currentSaveData?.PlayData != null)
        {
            currentSaveData.PlayData.currentHealth = PlayerManager.instance.playerStats.GetMaxHealthValue();
        }
    }
    /// <summary>
    /// 更新场景名字
    /// </summary>
    /// <param name="sceneName"></param>
    public void UpdateGameScene(string sceneName)
    {
        if (currentSaveData?.PlayData != null)
        {
            currentSaveData.PlayData.sceneName = sceneName;
        }
    }
    /// <summary>
    /// 获取当前游戏时间
    /// </summary>
    public float GetPlayTime()
    {
        return currentSaveData?.playTime ?? 0f;
    }
    #endregion
}
