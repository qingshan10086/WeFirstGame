using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour
{
    public Player player;
    [Header("Cinemachine")]
    public CinemachineVirtualCamera vcam;      // 指向虚拟相机
    public Vector3 targetPosition;             // 希望相机移动到的世界坐标
    public float targetOrthoSize = 30f;        // 目标正交大小

    [Header("Transition")]
    public float duration = 0.5f;              // 过渡时间（0 = 立即）
    public bool clearFollow = true;            // 是否清空 vcam.Follow，以便直接使用 vcam.transform.position

    [Header("Activation")]
    public string playerTag = "Player";        // 玩家 tag

    bool activated = false;

    // 保存进入前的相机状态，用于恢复
    float savedOrthoSize;
    Vector3 savedPosition;
    int savedPriority;
    Transform savedFollow;
    bool hasSaved = false;

    Coroutine currentTransition;

    void Reset()
    {
        // 确保碰撞体为触发器（便于快速设置）
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (activated) return;
        if (other.CompareTag(playerTag))
        {
            if (vcam != null)
            {
                // 记录原始状态（只记录一次）
                if (!hasSaved)
                {
                    savedOrthoSize = vcam.m_Lens.OrthographicSize;
                    savedPosition = vcam.transform.position;
                    savedPriority = vcam.Priority;
                    savedFollow = vcam.Follow;
                    hasSaved = true;
                }

                // 停掉正在进行的过渡（如果有）
                if (currentTransition != null)
                {
                    StopCoroutine(currentTransition);
                    currentTransition = null;
                }

                // 提升 priority 确保虚拟相机被激活（可根据项目调整数值）
                vcam.Priority = 1000;
                if (clearFollow) vcam.Follow = null;
                currentTransition = StartCoroutine(DoCameraTransition());
            }
            activated = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!activated) return;
        if (other.CompareTag(playerTag))
        {
            if (vcam != null && hasSaved)
            {
                // 停掉正在进行的过渡（如果有）
                if (currentTransition != null)
                {
                    StopCoroutine(currentTransition);
                    currentTransition = null;
                }

                currentTransition = StartCoroutine(RestoreCameraTransition());
            }
            activated = false;
        }
    }

    IEnumerator DoCameraTransition()
    {
        if (vcam == null)
            yield break;

        // 读取当前值
        float startSize = vcam.m_Lens.OrthographicSize;
        Vector3 startPos = vcam.transform.position;

        if (duration <= 0f)
        {
            vcam.m_Lens.OrthographicSize = targetOrthoSize;
            vcam.transform.position = targetPosition;
            currentTransition = null;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            // 使用平滑插值（可改为线性）
            float smooth = Mathf.SmoothStep(0f, 1f, k);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetOrthoSize, smooth);
            vcam.transform.position = Vector3.Lerp(startPos, targetPosition, smooth);
            yield return null;
        }

        // 确保精确
        vcam.m_Lens.OrthographicSize = targetOrthoSize;
        vcam.transform.position = targetPosition;
        currentTransition = null;
    }

    IEnumerator RestoreCameraTransition()
    {
        if (vcam == null)
            yield break;

        // 当前值作为起点
        float startSize = vcam.m_Lens.OrthographicSize;
        Vector3 startPos = vcam.transform.position;

        // 恢复 Follow（立即恢复，以便后续由 Follow 驱动时位置匹配）
        if (savedFollow != null)
            vcam.Follow = savedFollow;

        if (duration <= 0f)
        {
            vcam.m_Lens.OrthographicSize = savedOrthoSize;
            vcam.transform.position = savedPosition;
            vcam.Priority = savedPriority;
            currentTransition = null;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float smooth = Mathf.SmoothStep(0f, 1f, k);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, savedOrthoSize, smooth);
            vcam.transform.position = Vector3.Lerp(startPos, savedPosition, smooth);
            yield return null;
        }

        // 确保精确并恢复 priority
        vcam.m_Lens.OrthographicSize = savedOrthoSize;
        vcam.transform.position = savedPosition;
        vcam.Priority = savedPriority;
        currentTransition = null;
    }
}