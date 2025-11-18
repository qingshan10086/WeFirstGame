using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guider : MonoBehaviour
{
    [SerializeField] private GameObject player;//获取玩家信息
    


    [SerializeField] private Transform playerCheck;//玩家检测
    [SerializeField] private float playerCheckRadius;//玩家检测的半径
    [SerializeField] private LayerMask playerLayer;//玩家层级

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDistance = 2f;
    

    public int faceDirection { get; private set; } = -1;       //面对方向，初始默认向左
    protected bool faceRight = true;                          //辅助是否翻转的数据

    private void Start()
    {
        
    }

    private void Update()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <=stopDistance)
            return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void Flip()
    {
        faceDirection = faceDirection * -1;
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
    }

    private bool CheckForPlayer() => Physics2D.OverlapCircle(playerCheck.position, playerCheckRadius, playerLayer);//检测是否检测到玩家
    
    private void OnDrawGizmos()
    {
         Gizmos.DrawWireSphere(playerCheck.position,playerCheckRadius);//在场景中画出玩家检测范围   
    }
}
