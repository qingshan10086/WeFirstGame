using System.Collections;

using UnityEngine;
using Cinemachine;


/// <summary>
/// Boss战相机控制器
/// 负责在Boss战期间控制相机行为
/// 使用Cinemachine虚拟相机系统
/// </summary>
public class BossBattleCameraController : MonoBehaviour
{
    [Header("Cinemachine设置")]
    public Camera Maincamera;
    public CinemachineVirtualCamera normalVirtualCamera;  // 普通虚拟相机
    public CinemachineVirtualCamera bossBattleVirtualCamera;  // Boss战虚拟相机
    public Transform bossBattleCameraTransform;  // Boss战相机的固定位置（通过Transform设置，可可视化调控）
    public bool useFixedPosition = true;  // 是否使用固定位置
    
    [Header("订阅设置")]
    public bool subscribeOnStart = true;  // 是否在Start时订阅
    public bool unsubscribeOnDestroy = true;  // 是否在销毁时取消订阅

    private bool isInBossBattleMode = false;

    private void Start()
    {
        if (subscribeOnStart)
        {
            SubscribeToBossBattleEvents();
        }
        
        // 初始化状态：禁用Boss战相机
        if (bossBattleVirtualCamera != null)
        {
            bossBattleVirtualCamera.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (unsubscribeOnDestroy)
        {
            UnsubscribeFromBossBattleEvents();
        }
    }

    // 在场景视图中可视化Boss战相机的固定位置
    private void OnDrawGizmosSelected()
    {
        if (useFixedPosition && bossBattleVirtualCamera != null)
        {
            if (bossBattleCameraTransform != null)
            {
                // 绘制固定位置的可视化标记
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(bossBattleCameraTransform.position, 0.5f);
                Gizmos.DrawLine(bossBattleCameraTransform.position, bossBattleCameraTransform.position + bossBattleCameraTransform.up * 2f);
                Gizmos.DrawLine(bossBattleCameraTransform.position, bossBattleCameraTransform.position + bossBattleCameraTransform.forward * 2f);
                
                // 绘制当前相机位置到固定位置的连接线
                if (bossBattleVirtualCamera.transform != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(bossBattleVirtualCamera.transform.position, bossBattleCameraTransform.position);
                }
            }
            else
            {
                // 如果没有设置Transform，使用相机当前位置作为参考
                Transform camTransform = bossBattleVirtualCamera.transform;
                if (camTransform != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(camTransform.position, 0.5f);
                    Gizmos.DrawLine(camTransform.position, camTransform.position + camTransform.up * 2f);
                    Gizmos.DrawLine(camTransform.position, camTransform.position + camTransform.forward * 2f);
                }
            }
        }
    }

    // 订阅Boss战事件
    public void SubscribeToBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.AddListener(OnBossBattleStarted);
            EventManager.Instance.OnBossBattleEnd.AddListener(OnBossBattleEnded);
            EventManager.Instance.OnBossDefeated.AddListener(OnBossDefeated);
            Debug.Log("BossBattleCameraController subscribed to events");
        }
        else
        {
            Debug.LogError("EventManager instance not found!");
        }
    }

    // 取消订阅Boss战事件
    public void UnsubscribeFromBossBattleEvents()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnBossBattleStart.RemoveListener(OnBossBattleStarted);
            EventManager.Instance.OnBossBattleEnd.RemoveListener(OnBossBattleEnded);
            EventManager.Instance.OnBossDefeated.RemoveListener(OnBossDefeated);
            Debug.Log("BossBattleCameraController unsubscribed from events");
        }
    }

    // Boss战开始时切换到Boss战相机模式
    private void OnBossBattleStarted()
    {
        Debug.Log("Switching to Boss battle camera mode");
        isInBossBattleMode = true;
        SwitchToCinemachineBossCamera();
    }

    // Boss战结束时切换回普通相机模式
    private void OnBossBattleEnded()
    {
        Debug.Log("Switching back to normal camera mode");
        isInBossBattleMode = false;
        SwitchToCinemachineNormalCamera();
    }

    // Boss被击败时切换回普通相机模式
    private void OnBossDefeated()
    {
        Debug.Log("Boss defeated, switching back to normal camera mode");
        isInBossBattleMode = false;
        SwitchToCinemachineNormalCamera();
    }

    // 切换到Cinemachine Boss战相机
    private void SwitchToCinemachineBossCamera()
    {
        if (bossBattleVirtualCamera != null)
        {
            // 设置相机固定位置
            Transform camTransform = bossBattleVirtualCamera.transform;
            if (camTransform != null)
            {
                if (useFixedPosition && bossBattleCameraTransform != null)
                {
                    // 使用Transform设置的固定位置（保持z轴不变）
                    Vector3 newPosition = camTransform.position;
                    newPosition.x = bossBattleCameraTransform.position.x;
                    newPosition.y = bossBattleCameraTransform.position.y;
                    camTransform.position = newPosition;
                    camTransform.rotation = bossBattleCameraTransform.rotation;
                }

            }
            
            // 禁用相机跟随（如果有跟随组件）
            CinemachineTransposer transposer = bossBattleVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = Vector3.zero;
                transposer.m_XDamping = 0f;
                transposer.m_YDamping = 0f;
                transposer.m_ZDamping = 0f;
            }
            
            // 禁用相机视角跟随（如果有Composer组件）
            CinemachineComposer composer = bossBattleVirtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null)
            {
             
            }
            
            // 设置相机立即移动到目标位置（重置Cinemachine内部状态）
            bossBattleVirtualCamera.PreviousStateIsValid = false;
            bossBattleVirtualCamera.ForceCameraPosition(camTransform.position, camTransform.rotation);
            
            // 先禁用所有相机
            if (normalVirtualCamera != null && normalVirtualCamera != bossBattleVirtualCamera)
            {
                normalVirtualCamera.gameObject.SetActive(false);
            }
            
            // 再启用Boss战相机
            bossBattleVirtualCamera.gameObject.SetActive(true);
            // Maincamera.transform.position = new Vector3(camTransform.position.x, camTransform.position.y, Maincamera.transform.position.z);
            Debug.Log("Cinemachine Boss battle camera activated and fixed");
        }
        else
        {
            Debug.LogError("BossBattleVirtualCamera not assigned!");
        }
    }

    // 切换回Cinemachine普通相机
    private void SwitchToCinemachineNormalCamera()
    {
        if (normalVirtualCamera != null)
        {
            // 先禁用所有相机
            if (bossBattleVirtualCamera != null && bossBattleVirtualCamera != normalVirtualCamera)
            {
                bossBattleVirtualCamera.gameObject.SetActive(false);
            }
            
            // 恢复普通相机的跟随功能
            CinemachineTransposer transposer = normalVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                // 恢复默认阻尼值（可以根据项目需要调整）
                transposer.m_XDamping = 1f;
                transposer.m_YDamping = 1f;
                transposer.m_ZDamping = 1f;
            }
            
            // 再启用普通相机
            normalVirtualCamera.gameObject.SetActive(true);
            
            
            Debug.Log("Cinemachine normal camera activated");
        }
        else
        {
            Debug.LogError("NormalVirtualCamera not assigned!");
        }
    }
}
