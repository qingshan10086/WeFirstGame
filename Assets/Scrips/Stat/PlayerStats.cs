using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats :CharacterStats//玩家的数据
{
    Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();//获取玩家组件
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        player.DamageEffect();//攻击效果
    }

    protected override void Die()
    {
        base.Die();

        player.Die();
    }

}
