using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Return : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < -200)
        {
            Destroy(transform.root.gameObject);
        }
    }
}
