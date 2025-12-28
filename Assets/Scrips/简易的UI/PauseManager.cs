using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseC;
    public GameObject OptionC;
    public GameObject AudioC;
    public GameObject LanguageC;
    private bool isPause = false;

    public void Option()
    {
        isPause = false;
        PauseC.SetActive(false);
        OptionC.SetActive(true);
        AudioC.SetActive(false);
        LanguageC.SetActive(false);
    }
    public void Audio()
    {
        isPause = false;
        PauseC.SetActive(false);
        OptionC.SetActive(false);
        AudioC.SetActive(true);
        LanguageC.SetActive(false);
    }
    public void Language()
    {
        isPause = false;
        PauseC.SetActive(false);
        OptionC.SetActive(false);
        AudioC.SetActive(false);
        LanguageC.SetActive(true);
    }
    public void Pause()
    {
        isPause = true;
        PauseC.SetActive(true);
        OptionC.SetActive(false);
        AudioC.SetActive(false);
        LanguageC.SetActive(false);
    }
    public void Resume()
    {
        isPause = false;
        PauseC.SetActive(false);
    }
    public void ReturnMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

}
