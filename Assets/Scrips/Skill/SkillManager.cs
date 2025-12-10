using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour//技能管理单例
{
    public static SkillManager instance {  get; private set; }


    public Dash_Skill dash { get; private set; }//冲刺技能
    public Clone_Skill clone { get; private set; }//克隆技能


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
    }
}
