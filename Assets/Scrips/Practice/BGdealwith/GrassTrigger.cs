using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prx;
    private pallrex prxScript;

    private void Awake()
    {
        prxScript = prx.GetComponent<pallrex>();
    }
    
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入了草地");
            SceneFade.Instance.QuickFadeIn();
        }

        prxScript.sign = !prxScript.sign;
    }
}
