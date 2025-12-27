using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadein : MonoBehaviour
{

    private void Awake()
    {
       
    }
    
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneFade.Instance.QuickFadeIn();
        }  
    }
}
