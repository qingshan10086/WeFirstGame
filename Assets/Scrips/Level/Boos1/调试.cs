using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class 调试 : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene("BloodFields");

        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SceneManager.LoadScene("EchoAbyss");
        }
    }
}
