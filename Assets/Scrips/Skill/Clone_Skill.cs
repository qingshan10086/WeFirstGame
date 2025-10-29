using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill//克隆一个自身的技能
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;//克隆预制体
    [SerializeField] private float cloneDuration;//克隆持续时间
    [Space]
    [SerializeField] private bool canAttack;

    public void CreatClone(Transform _clonePosition)//创造克隆体函数
    {
        GameObject newClone = Instantiate(clonePrefab);//获取预制件

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition,cloneDuration,canAttack);//克隆相关数据函数
    }

}
