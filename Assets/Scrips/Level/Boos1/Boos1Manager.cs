using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boos1Manager : MonoBehaviour
{
    public EnemyStats stat;//获取敌人数据

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (!isDead)
        {
            if (stat.currentHealth < 0)
            {
                isDead=true;
                KillAllMinions();
            }
        }
    }


    public void KillAllMinions()
    {

        Enemy[] enemys;

        enemys = GameObject.FindObjectsOfType<Enemy>();

        foreach (var e in enemys)
        {
            if (e == null) continue;

            e.stats.TakeDamage(200);
            
        }
    }
}
