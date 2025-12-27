using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLoveTown : MonoBehaviour
{
    public Collider2D collider2D;
    public OneTimeMechanism oneTimeMechanism;

    // Start is called before the first frame update
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        oneTimeMechanism = GetComponent<OneTimeMechanism>();
        // 确保Collider2D是触发器
        if (collider2D != null)
        {
            collider2D.isTrigger = true;
        }
       
    }

    // OnTriggerEnter2D是Unity的内置方法，首字母必须大写
    void OnTriggerEnter2D(Collider2D other)
    {
        if (oneTimeMechanism.isTriggered)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("播放新音乐");

                // 音乐改变后
                if (collider2D != null)
                {
                    collider2D.enabled = false;
                }

                // 可以选择销毁对象或禁用渲染
                // Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}
