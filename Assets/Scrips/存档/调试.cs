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
        if (Input.GetKeyDown(KeyCode.Alpha1)){SceneManager.LoadScene(1);}
        if (Input.GetKeyDown(KeyCode.Alpha2)){SceneManager.LoadScene(2);}
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SceneManager.LoadScene(3); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { SceneManager.LoadScene(4); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { SceneManager.LoadScene(5); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { SceneManager.LoadScene(6); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { SceneManager.LoadScene(7); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { SceneManager.LoadScene(8); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { SceneManager.LoadScene(9); }
        if (Input.GetKeyDown(KeyCode.Z)) { SceneManager.LoadScene(10); }
        if (Input.GetKeyDown(KeyCode.X)) { SceneManager.LoadScene(11); }
        if (Input.GetKeyDown(KeyCode.C)) { SceneManager.LoadScene(12); }
        if (Input.GetKeyDown(KeyCode.V)) { SceneManager.LoadScene(13); }
        if (Input.GetKeyDown(KeyCode.B)) { SceneManager.LoadScene(14); }
        if (Input.GetKeyDown(KeyCode.N)) { SceneManager.LoadScene(15); }
    }
}
