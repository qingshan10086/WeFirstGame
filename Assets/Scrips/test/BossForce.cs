using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossForce : MonoBehaviour
{
    Boss_Death boss;
    private void Start()
    {
        boss = GetComponent<Boss_Death>();
        boss.enabled = true;
    }
    //private void Update()
    //{
    //    boss.enabled = true;
    //}
}
