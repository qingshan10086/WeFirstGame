using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
public class GainEnermySys : MonoBehaviour
{
    public Collider2D collider2D;
    public Canvas canvasone;
    public Canvas canvasTwo;
    public PlayerEnergySystem playerEnergySystem;
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
        
        // 确保canvas初始状态是禁用的
        if (canvasone != null)
        {
            canvasone.gameObject.SetActive(false);
        }
        if (canvasTwo != null)
        {
            canvasTwo.gameObject.SetActive(false);
        }
    }

    // OnTriggerEnter2D是Unity的内置方法，首字母必须大写
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("玩家获得了技能");
            
            // 添加空引用检查
            if (playerEnergySystem != null)
            {
                // 解锁技能
                playerEnergySystem.isSkillEnabled = true;
            }
            
            if (canvasone != null)
            {
                canvasone.gameObject.SetActive(true);
            }
            
            if (canvasTwo != null)
            {
                canvasTwo.gameObject.SetActive(true);
            }
            
            // 技能获得后禁用触发器和自身
            if (collider2D != null)
            {
                collider2D.enabled = false;
            }
            
            // 可以选择销毁对象或禁用渲染
            // Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if(oneTimeMechanism!=null)
        {
            if(oneTimeMechanism.isTriggered)
            {
                playerEnergySystem.isSkillEnabled = true;
                canvasone.gameObject.SetActive(true);
                canvasTwo.gameObject.SetActive(true);
                collider2D.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }

}
