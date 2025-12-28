using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public GameObject mainC;
    public GameObject OptionC;
    public GameObject AudioC;
    public GameObject LanguageC;

    public void Option()
    {
        mainC.SetActive(false);
        OptionC.SetActive(true);
        AudioC.SetActive(false);
        LanguageC.SetActive(false);
    }
    public void Audio()
    {
        mainC.SetActive(false);
        OptionC.SetActive(false);
        AudioC.SetActive(true);
        LanguageC.SetActive(false);
    }
    public void Language()
    {
        mainC.SetActive(false);
        OptionC.SetActive(false);
        AudioC.SetActive(false);
        LanguageC.SetActive(true);
    }
    public void Main()
    {
        mainC.SetActive(true);
        OptionC.SetActive(false);
        AudioC.SetActive(false);
        LanguageC.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
