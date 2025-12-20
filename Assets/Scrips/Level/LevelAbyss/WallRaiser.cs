using System.Collections;
using UnityEngine;

public class WallRaiser : MonoBehaviour
{
    [Header("References")]
    public Transform player;      // 玩家 Transform
    public Transform wall;        // 要升起的墙体 Transform
    public GameObject Door;       // 要开启的门

    [Header("Trigger")]
    public float triggerRadius = 3f;

    [Header("Rise Settings")]
    public float riseHeight = 5f; // 抬升高度（相对于初始位置）
    public float riseSpeed = 2f;  // 抬升速度（单位：世界坐标/秒）
    public bool useLocalPosition = false; // 是否使用 localPosition 而不是 position

    bool isRaising = false;
    Vector3 initialPos;
    Vector3 targetPos;

    MonoBehaviour DoorC;

    void Start()
    {
        DoorC = Door.GetComponent<ToAbyssPlus>();
        if (wall == null)
        {
            Debug.LogError("[WallRaiser] 请在 Inspector 中指定 wall。", this);
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("[WallRaiser] 请在 Inspector 中指定 player。", this);
            enabled = false;
            return;
        }

        initialPos = useLocalPosition ? wall.localPosition : wall.position;
        targetPos = initialPos + new Vector3(0f, riseHeight, 0f);
    }

    void Update()
    {
        if (isRaising) return;
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= triggerRadius)
        {
            DoorC.enabled = true;
            StartCoroutine(RaiseWall());
        }
    }

    IEnumerator RaiseWall()
    {
        isRaising = true;
        while (true)
        {
            Vector3 current = useLocalPosition ? wall.localPosition : wall.position;
            Vector3 next = Vector3.MoveTowards(current, targetPos, riseSpeed * Time.deltaTime);

            if (useLocalPosition)
                wall.localPosition = next;
            else
                wall.position = next;

            if (Vector3.Distance(next, targetPos) <= 0.001f)
                yield break;

            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 在 Scene 视图显示触发半径和连线（便于编辑和调试）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        if (wall != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, useLocalPosition ? transform.TransformPoint(wall.localPosition) : wall.position);
        }
    }
}