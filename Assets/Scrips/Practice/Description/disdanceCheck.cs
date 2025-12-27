using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disdanceCheck : MonoBehaviour
{
 
    public Player player;
    public float disdance = 1f;
    public Transform target;
    public float width=5f;
    public float height=4f;
    void Start()
    {
       
    }
    
    // 在编辑器中可视化检测范围
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            // 根据Player是否在范围内设置不同颜色
            bool isPlayerInRange = detectPlayer();
            
            // 内部填充色：在范围内为半透明绿色，不在范围内为半透明红色
            Gizmos.color = isPlayerInRange ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.2f);
            
            // 绘制检测范围矩形
            Vector3 center = new Vector3(target.position.x, target.position.y, 0);
            Vector3 size = new Vector3(width, height, 0.1f);
            Gizmos.DrawCube(center, size);
            
            // 边框颜色：在范围内为绿色，不在范围内为红色
            Gizmos.color = isPlayerInRange ? Color.green : Color.red;
            Gizmos.DrawWireCube(center, size);
            
            // 如果Player存在，绘制Player位置到target的连线
            if (player != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(player.transform.position, target.position);
            }
        }
    }

   
    void Update()
    {
        if(player != null)
        {
            disdance = transform.position.x -player.transform.position.x;
        }
        else{
            Debug.Log("Player not found!");
        }
    }
    public bool detectPlayer()
    {
        if (player != null && target != null)
        {
            // 检查Player是否在target的矩形范围内
            Vector2 playerPos = player.transform.position;
            Vector2 targetPos = target.position;
            
            float left = targetPos.x - width / 2f;
            float right = targetPos.x + width / 2f;
            float bottom = targetPos.y - height / 2f;
            float top = targetPos.y + height / 2f;
            
            return playerPos.x >= left && playerPos.x <= right && playerPos.y >= bottom && playerPos.y <= top;
        }
        return false;
    }
    public float getDistance()
    {
        // 检查Player是否在target范围内
        if (!detectPlayer())
        {
            // 如果不在范围内，返回极大值
            return float.MaxValue;
        }
        
        // 如果在范围内，返回正常的距离值（取绝对值）
        return Mathf.Abs(disdance);
    }
}
    
