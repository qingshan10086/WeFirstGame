using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour//玩家技能的父类,
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        Debug.Log("技能正在冷却");
        return false;
    }


    public virtual void UseSkill()
    {
        //其中添加相应技能逻辑
    }
}
