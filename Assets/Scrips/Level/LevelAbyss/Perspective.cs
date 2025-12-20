using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class Perspective : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // 玩家 Transform（用于距离触发或比较）
    public Transform target;                 // 要展示的巨大物体根节点（注意：必须为 Transform）
    public Camera sceneCamera;               // 为 null 则自动使用 Camera.main
    public GameObject cinemachineVirtualCamera; // 可选：指向 Cinemachine Virtual Camera 的 GameObject

    [Header("Trigger")]
    public Transform triggerPoint;           // 当使用距离触发时比较位置
    public float triggerRadius = 3f;         // 距离阈值

    [Header("Transition")]
    public float transitionDuration = 2.0f;  // 平滑过渡时长（秒）
    public bool fitToTarget = true;          // 自动计算合适距离并调整（通过位置计算）
    public float manualDistance = 10f;       // 如果不使用 fitToTarget，使用此距离
    public float targetFOV = 40f;            // 过渡到的 FOV（若使用虚拟相机可能无法生效）
    public Vector3 manualOffset = Vector3.back; // 如果不使用 fitToTarget，基于 target 的方向偏移量（单位向量）

    [Header("Debug / Behaviour")]
    public bool autoUnparentCamera = true;   // 如果摄像机是玩家的子物体，过渡前自动解绑
    public bool drawGizmos = true;           // Scene 视图绘制触发范围与目标包围盒

    private bool triggered = false;

    // Cinemachine 反射相关
    private Type vcamType = null;
    private Component vcamComponent = null;
    private PropertyInfo vcamPriorityProp = null;
    private int originalVcamPriority = 0;
    private Behaviour cinemachineBrain = null;

    void Reset()
    {
        sceneCamera = Camera.main;
    }

    void Start()
    {
        if (sceneCamera == null) sceneCamera = Camera.main;
        if (sceneCamera == null)
        {
            Debug.LogWarning("[Perspective] 找不到 Camera，请在 Inspector 指定 sceneCamera 或确保场景有 Camera 标签为 MainCamera。");
        }

        // 尝试通过反射查找 CinemachineVirtualCamera 类型
        vcamType = FindTypeByName("Cinemachine.CinemachineVirtualCamera") ?? FindTypeByName("CinemachineVirtualCamera");
        if (vcamType != null && cinemachineVirtualCamera != null)
        {
            vcamComponent = cinemachineVirtualCamera.GetComponent(vcamType);
            if (vcamComponent != null)
            {
                vcamPriorityProp = vcamType.GetProperty("Priority");
                if (vcamPriorityProp != null)
                {
                    try
                    {
                        object val = vcamPriorityProp.GetValue(vcamComponent);
                        originalVcamPriority = val != null ? (int)val : 0;
                    }
                    catch { originalVcamPriority = 0; }
                }
                Debug.Log("[Perspective] 检测到 CinemachineVirtualCamera，并已绑定到脚本。将优先使用虚拟相机进行过渡。");
            }
            else
            {
                Debug.LogWarning("[Perspective] 指定的 cinemachineVirtualCamera GameObject 上未找到 CinemachineVirtualCamera 组件，回退到直接控制 Camera 的方法。");
            }
        }

        // 尝试找到 CinemachineBrain（用于回退方案）
        var cmBrain = sceneCamera != null ? sceneCamera.GetComponent("CinemachineBrain") as Behaviour : null;
        if (cmBrain != null)
        {
            cinemachineBrain = cmBrain;
            Debug.Log("[Perspective] 在主摄像机上检测到 CinemachineBrain（将根据需要临时禁用）。");
        }

        Debug.Log("[Perspective] 初始化完成。目标=" + (target ? target.name : "null") + "，摄像机=" + (sceneCamera ? sceneCamera.name : "null"));
    }

    void Update()
    {
        if (triggered) return;
        if (player == null || triggerPoint == null) return;

        float d = Vector3.Distance(player.position, triggerPoint.position);
        if (d <= triggerRadius)
        {
            Debug.Log("[Perspective] 距离触发：d=" + d + " <= " + triggerRadius);
            StartTransition();
        }
    }

    [ContextMenu("Force Start Transition")]
    public void ForceStartTransition()
    {
        Debug.Log("[Perspective] 人为触发 StartTransition()");
        StartTransition();
    }

    public void StartTransition()
    {
        if (triggered) return;
        if (target == null)
        {
            Debug.LogWarning("[Perspective] 需要指定 target（Transform）。");
            return;
        }
        if (sceneCamera == null)
        {
            Debug.LogWarning("[Perspective] 需要指定 sceneCamera 或确保场景有 MainCamera。");
            return;
        }

        // 若摄像机为某个对象的子对象，且用户允许，解绑它以避免父级或跟随脚本覆盖变换（只在回退方案使用）
        if (autoUnparentCamera && sceneCamera.transform.parent != null)
        {
            Debug.Log("[Perspective] 摄像机有父对象 (" + sceneCamera.transform.parent.name + ")，正在解绑以保证过渡生效（回退方案）。");
            sceneCamera.transform.SetParent(null);
        }

        StartCoroutine(TransitionCoroutine());
        triggered = true;
    }

    IEnumerator TransitionCoroutine()
    {
        // 计算目标包围盒与中心
        Bounds bounds = CalculateBounds(target);
        Vector3 boundsCenter = bounds.center;

        // 计算目标位置与方向
        Vector3 goalPos;
        Quaternion goalRot;
        float desiredDistance;

        if (fitToTarget)
        {
            float halfHeight = bounds.size.y * 0.5f;
            float halfWidth = bounds.size.x * 0.5f;
            float vFovRad = sceneCamera.fieldOfView * Mathf.Deg2Rad;
            float distanceForHeight = (halfHeight) / Mathf.Tan(vFovRad * 0.5f);
            float hFovRad = 2f * Mathf.Atan(Mathf.Tan(vFovRad * 0.5f) * sceneCamera.aspect);
            float distanceForWidth = (halfWidth) / Mathf.Tan(hFovRad * 0.5f);
            desiredDistance = Mathf.Max(distanceForHeight, distanceForWidth);
            float radius = bounds.extents.magnitude;
            desiredDistance = Mathf.Max(desiredDistance, radius) * 1.05f;
        }
        else
        {
            desiredDistance = manualDistance;
        }

        // 默认方向为从当前主摄像机指向目标的方向；如果使用虚拟相机且该对象不在场景中合适方向，可根据 manualOffset 调整
        Vector3 dir;
        if (vcamComponent != null)
            dir = (vcamComponent as Component).transform.position != Vector3.zero ? ((vcamComponent as Component).transform.position - boundsCenter).normalized : -target.forward;
        else
            dir = (sceneCamera.transform.position - boundsCenter).normalized;
        if (dir == Vector3.zero) dir = -target.forward;

        goalPos = boundsCenter + dir * desiredDistance;
        goalRot = Quaternion.LookRotation(boundsCenter - goalPos, Vector3.up);

        Debug.Log($"[Perspective] 目标 center={boundsCenter}, goalPos={goalPos}, desiredDistance={desiredDistance}");

        // 使用虚拟相机方案（优先）
        if (vcamComponent != null)
        {
            var vcamTransform = (vcamComponent as Component).transform;

            // 提高优先级使其成为活动虚拟相机（通过反射设置 Priority）
            if (vcamPriorityProp != null)
            {
                try
                {
                    object current = vcamPriorityProp.GetValue(vcamComponent);
                    originalVcamPriority = current != null ? (int)current : 0;
                }
                catch { originalVcamPriority = 0; }

                try
                {
                    vcamPriorityProp.SetValue(vcamComponent, 9999);
                    Debug.Log("[Perspective] 将虚拟相机 Priority 提高到 9999 以确保其成为活动相机。");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("[Perspective] 无法设置虚拟相机 Priority: " + ex.Message);
                }
            }

            // 平滑移动虚拟相机 Transform
            Vector3 startPos = vcamTransform.position;
            Quaternion startRot = vcamTransform.rotation;
            yield return SmoothMove(vcamTransform, startPos, goalPos, startRot, goalRot, transitionDuration);

            // 尝试设置虚拟相机的 FOV（如果存在可写的 Lens 或 FieldOfView，反射尝试）
            TrySetVirtualCameraFOV(vcamComponent, targetFOV);

            // 等待小段时间以便 Cinemachine 完成 blend（可根据需要调整）
            yield return new WaitForSeconds(0.1f);

            // 恢复原始 Priority（可注释以保留该虚拟相机为长期激活）
            if (vcamPriorityProp != null)
            {
                try
                {
                    vcamPriorityProp.SetValue(vcamComponent, originalVcamPriority);
                    Debug.Log("[Perspective] 恢复虚拟相机原始 Priority=" + originalVcamPriority);
                }
                catch { }
            }

            Debug.Log("[Perspective] 使用虚拟相机过渡完成。最终虚拟相机位置=" + vcamTransform.position);
        }
        else
        {
            // 回退：临时禁用 CinemachineBrain（若存在），直接控制主摄像机 Transform
            if (cinemachineBrain != null && cinemachineBrain.enabled)
            {
                Debug.Log("[Perspective] 临时禁用 CinemachineBrain 以允许直接控制摄像机。");
                cinemachineBrain.enabled = false;
            }

            Transform camT = sceneCamera.transform;
            Vector3 startPos = camT.position;
            Quaternion startRot = camT.rotation;
            yield return SmoothMove(camT, startPos, goalPos, startRot, goalRot, transitionDuration);

            // 恢复 CinemachineBrain
            if (cinemachineBrain != null)
            {
                cinemachineBrain.enabled = true;
                Debug.Log("[Perspective] 恢复 CinemachineBrain（已重新启用）。");
            }

            Debug.Log("[Perspective] 直接控制主摄像机过渡完成。最终摄像机位置=" + camT.position);
        }
    }

    IEnumerator SmoothMove(Transform camT, Vector3 fromPos, Vector3 toPos, Quaternion fromRot, Quaternion toRot, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            camT.position = Vector3.Lerp(fromPos, toPos, k);
            camT.rotation = Quaternion.Slerp(fromRot, toRot, k);
            yield return null;
        }
        camT.position = toPos;
        camT.rotation = toRot;
    }

    // 通过反射尝试设置虚拟相机的 FOV（非必要，如果失败不会抛）
    void TrySetVirtualCameraFOV(Component vcamComp, float fov)
    {
        if (vcamComp == null) return;
        try
        {
            // 常见字段/属性名尝试：m_Lens (struct) -> FieldOfView, 或通过 "m_Lens.FieldOfView"（复杂），或属性 "m_Lens" 直接不可写。
            // 这里尝试设置名为 "m_Lens" 的字段的 FieldOfView 子字段（若存在）
            var field = vcamComp.GetType().GetField("m_Lens", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field != null)
            {
                object lens = field.GetValue(vcamComp);
                if (lens != null)
                {
                    var lf = lens.GetType().GetField("FieldOfView") ?? lens.GetType().GetField("fov") ?? lens.GetType().GetField("m_FieldOfView");
                    if (lf != null)
                    {
                        lf.SetValueDirect(__makeref(lens), fov);
                        // 将修改后的 lens 再写回组件字段（Lens 是 struct，需要写回）
                        field.SetValue(vcamComp, lens);
                        Debug.Log("[Perspective] 已通过反射尝试设置虚拟相机的 FOV=" + fov);
                        return;
                    }
                }
            }

            // 备用：尝试属性 "m_Lens" 的 FieldOfView（如果是属性）
            var prop = vcamComp.GetType().GetProperty("m_Lens", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop != null)
            {
                object lens = prop.GetValue(vcamComp);
                if (lens != null)
                {
                    var lf = lens.GetType().GetField("FieldOfView") ?? lens.GetType().GetField("m_FieldOfView");
                    if (lf != null)
                    {
                        lf.SetValueDirect(__makeref(lens), fov);
                        prop.SetValue(vcamComp, lens);
                        Debug.Log("[Perspective] 已通过反射尝试设置虚拟相机的 FOV（备用方式）=" + fov);
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Perspective] 反射设置虚拟相机 FOV 失败：" + ex.Message);
        }
    }

    Bounds CalculateBounds(Transform root)
    {
        Renderer[] rends = root.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
        {
            return new Bounds(root.position, Vector3.one * 0.1f);
        }

        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
        {
            b.Encapsulate(rends[i].bounds);
        }
        return b;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.cyan;
        if (triggerPoint != null)
        {
            Gizmos.DrawWireSphere(triggerPoint.position, triggerRadius);
        }
        if (target != null)
        {
            Bounds b = CalculateBounds(target);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }

    // 小工具：按名称查找类型（遍历已加载程序集）
    private Type FindTypeByName(string typeName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var t = asm.GetType(typeName);
                if (t != null) return t;
            }
            catch { }
        }
        return null;
    }
}