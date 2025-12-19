using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
    //完成上下场景切换
    public void upScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void downScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    //直接跳转到指定场景
    public void change1()
    {
        SceneManager.LoadScene(0);
    }
    public void change2()
    {
        SceneManager.LoadScene(1);
    }

    public void change3()
    {
        SceneManager.LoadScene(2);
    }

    public void change4()
    {
        SceneManager.LoadScene(3);
    }

    public void change5()
    {
        SceneManager.LoadScene(4);
    }

    public void change6()
    {
        SceneManager.LoadScene(5);
    }

    public void change7()
    {
        SceneManager.LoadScene(6);
    }

    public void change8()
    {
        SceneManager.LoadScene(7);
    }

    public void change9()
    {
        SceneManager.LoadScene("场景切换");
    }

    //退出游戏
    public void exitGame()
    {
        Application.Quit();
    }
}
