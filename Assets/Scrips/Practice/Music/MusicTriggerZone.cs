


using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音乐触发区域
/// 当玩家进入特定范围时播放对应的音乐
/// </summary>
public class MusicTriggerZone : MonoBehaviour
{
    [Header("基本设置")]
    [Tooltip("触发区域的宽度")]
    public float triggerWidth = 10f;
    
    [Tooltip("触发区域的高度")]
    public float triggerHeight = 5f;
    
    [Header("音乐设置")]
    [Tooltip("进入区域时播放的音乐")]
    public AudioClip zoneMusic;
    
    [Tooltip("是否使用淡入淡出效果")]
    public bool useFadeEffects = true;
    
    [Tooltip("淡入淡出持续时间")]
    public float fadeDuration = 1f;
    
    [Tooltip("离开区域时是否恢复之前的音乐")]
    public bool restorePreviousMusic = true;
    
    [Header("玩家设置")]
    [Tooltip("玩家标签")]
    public string playerTag = "Player";
    
    [Tooltip("是否需要指定特定玩家对象")]
    public bool useSpecificPlayer = false;
    
    [Tooltip("特定玩家对象(仅当useSpecificPlayer为true时有效)")]
    public GameObject specificPlayer;
    
    // 内部变量
    private bool playerInZone = false;
    private AudioClip previousMusic = null;
  
    private void Awake()
    {
        // 设置碰撞体
        SetupCollider();
    }
    
    private void SetupCollider()
    {
        // 如果没有2D碰撞体，添加一个BoxCollider2D
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        
        if (boxCollider2D == null)
        {
            boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
        }
        else
        {
            boxCollider2D.isTrigger = true;
        }
        
        // 设置矩形大小
        boxCollider2D.size = new Vector2(triggerWidth, triggerHeight);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"MusicTriggerZone: OnTriggerEnter2D called by {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (IsPlayer(other.gameObject))
        {
            Debug.Log($"MusicTriggerZone: {other.gameObject.name} is a valid player!");
            PlayerEnteredZone();
        }
        else
        {
            Debug.Log($"MusicTriggerZone: {other.gameObject.name} is not a valid player (Tag: {other.gameObject.tag}, UseSpecificPlayer: {useSpecificPlayer})");
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"MusicTriggerZone: OnTriggerExit2D called by {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (IsPlayer(other.gameObject))
        {
            Debug.Log($"MusicTriggerZone: {other.gameObject.name} is a valid player!");
            PlayerExitedZone();
        }
        else
        {
            Debug.Log($"MusicTriggerZone: {other.gameObject.name} is not a valid player (Tag: {other.gameObject.tag}, UseSpecificPlayer: {useSpecificPlayer})");
        }
    }
    
    private bool IsPlayer(GameObject obj)
    {
        if (useSpecificPlayer)
        {
            return obj == specificPlayer;
        }
        return obj.CompareTag(playerTag);
    }
    
    private void PlayerEnteredZone()
    {
        
        // if (playerInZone || musicManager == null)
        //     return;
        Debug.Log("快要有音乐了");
        playerInZone = true;
        
        // 播放区域音乐
        if (zoneMusic != null)
        {
            Debug.Log("真的快来了");
            if (useFadeEffects)
            {
                Debug.Log("播放第二层音乐");
                BloodMusicManager.Instance.SwitchBackgroundMusic(zoneMusic, fadeDuration);
            }
            else
            {
                Debug.Log("播放第二层音乐");
                BloodMusicManager.Instance.SwitchBackgroundMusic(zoneMusic, 0f);
            }
            
            Debug.Log("MusicTriggerZone: 玩家进入区域，播放音乐: " + zoneMusic.name);
        }
        else
        {
            Debug.LogWarning("MusicTriggerZone: 区域音乐未设置");
        }
    }

    private void PlayerExitedZone()
    {
        if (!playerInZone || BloodMusicManager.Instance == null)
            return;

        playerInZone = false;

        // // 恢复之前的音乐或停止播放
        if (restorePreviousMusic && previousMusic != null)
        {
            //     if (useFadeEffects)
            //     {
            //         musicManager.SwitchBackgroundMusic(previousMusic, fadeDuration);
            //     }
            //     else
            //     {
            //         musicManager.SwitchBackgroundMusic(previousMusic, fadeDuration);

            // else
            // {
            //     // 如果不恢复之前的音乐，可以选择停止背景音乐或继续播放
            //     Debug.Log("MusicTriggerZone: 玩家离开区域");
            // }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 在编辑器中绘制矩形触发区域的范围
        Gizmos.color = Color.cyan;
        
        // 计算矩形的大小和位置（以transform为中心）
        Vector3 cubeSize = new Vector3(triggerWidth, triggerHeight, 0.1f);
        Vector3 cubePosition = transform.position;
        
        Gizmos.DrawWireCube(cubePosition, cubeSize);
    }
    
    [ContextMenu("测试：玩家进入区域")]
    private void TestPlayerEnter()
    {
        PlayerEnteredZone();
    }
    
    [ContextMenu("测试：玩家离开区域")]
    private void TestPlayerExit()
    {
        PlayerExitedZone();
    }
}
