using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy_SkeletonAnimationTrigger : MonoBehaviour//骷髅兵动画触发器管理，一般挂载在Animator物体上
{
    private SpriteRenderer sr;//精灵渲染组件
    private Animator anim;

    private float colorLoosingSpeed=0.2f;//颜色消失速度
    private float cloneTimer;//辅助计算克隆持续时间的

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




    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();//获取父物体骷髅兵类

    private void AnimationTrigger()//动画触发完成函数
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

    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();//获取开启被弹反
    private void CloseCounterWindow()=>enemy.CloseCounterAttackWindow();//获取关闭被弹反

}
