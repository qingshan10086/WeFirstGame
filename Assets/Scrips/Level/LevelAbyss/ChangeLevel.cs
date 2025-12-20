using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public Transform player;
    public float goalRaidus;
    
    private bool isSuccess;
    //ÇÐ»»³¡¾°
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ToOriginal()
    {
        SceneManager.LoadScene("EchoAbyss");
    }
    public void ToOriginalPlus()
    {

        SceneManager.LoadScene("EchoAbyss+");
    }
    private void Start()
    {
        isSuccess = false;
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < goalRaidus)
        {
            isSuccess = true;
        }
    }
    
}
