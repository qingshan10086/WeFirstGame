using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FlyingEyesAnimtionTrigger : MonoBehaviour
{
    private SpriteRenderer sr;//获取精灵渲染器组件
    private Animator anim;//获取动画管理机组件

    private float colorLoosingSpeed = 0.2f;//颜色消失速度
    private float cloneTimer;//辅助计算克隆持续时间的

    // Start is called before the first frame update
    private void Awake()
    {

        sr = GetComponent<SpriteRenderer>();//获取精灵渲染器
        anim = GetComponent<Animator>();//获取动画组件
    }


    // 公共方法，在需要开始渐隐时调用
    public void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    // 实现渐隐效果的协程
    private IEnumerator FadeOutCoroutine()
    {
        // 持续循环，直到透明度降至0
        while (sr.color.a > 0)
        {
            // 在每一帧减少透明度
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));
            // 等待下一帧
            yield return null;
        }

        // 当完全透明后，销毁整个物体
        Destroy(transform.parent.gameObject);
    }


    private Enemy_FlyingEyes enemy=>GetComponentInParent<Enemy_FlyingEyes>();//获取大眼睛怪物

    private void AnimationFinish()//动画触发完成函数
    {
        enemy.AnimationFinishTrigger();
    }


    private void AttackTrigger()//攻击触发函数
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);//获取攻击范围内部的碰撞器

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)//如果有玩家则造成相应攻击光效
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target);


            }

        }
    }



}
