using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour
{
    public GameObject mainC;
    public GameObject optionC;
    public GameObject audioC;
    public GameObject playC;
    public GameObject duorenC;
    public GameObject languageC;
    public GameObject vedioC;

    public void Start()
    {
        mainC.SetActive(true);
        optionC.SetActive(false);
        audioC.SetActive(false);
        playC.SetActive(false);
        duorenC.SetActive(false);
        languageC.SetActive(false);
        vedioC.SetActive(false);
    }

    public void mainCanva()
    {
        mainC.SetActive(true);
        optionC.SetActive(false);
        audioC.SetActive(false);
        playC.SetActive(false);
        duorenC.SetActive(false);
        languageC.SetActive(false);
        vedioC.SetActive(false);
    }
    public void optionCanva()
    {
        mainC.SetActive(false);
        optionC.SetActive(true);
        audioC.SetActive(false);
        playC.SetActive(false);
        duorenC.SetActive(false);
        languageC.SetActive(false);
        vedioC.SetActive(false);
    }
    public void audioCanva()
    {
        mainC.SetActive(false);
        optionC.SetActive(false);
        audioC.SetActive(true);
        playC.SetActive(false);
        duorenC.SetActive(false);
        languageC.SetActive(false);
        vedioC.SetActive(false);
    }
    public void playCanva()
    {
        mainC.SetActive(false);
        optionC.SetActive(false);
        audioC.SetActive(false);
        playC.SetActive(true);
        duorenC.SetActive(false);
        languageC.SetActive(false);
        vedioC.SetActive(false);
    }
    public void duorenCanva()
    {
        mainC.SetActive(false);
        optionC.SetActive(false);
        audioC.SetActive(false);
        playC.SetActive(false);
        duorenC.SetActive(true);
        languageC.SetActive(false);
        vedioC.SetActive(false);
    }
    public void languageCanva()
    {
        mainC.SetActive(false);
        optionC.SetActive(false);
        audioC.SetActive(false);
        playC.SetActive(false);
        duorenC.SetActive(false);
        languageC.SetActive(true);
        vedioC.SetActive(false);
    }
    public void vedioCanva()
    {
        mainC.SetActive(false);
        optionC.SetActive(false);
        audioC.SetActive(false);
        playC.SetActive(false);
        duorenC.SetActive(false);
        languageC.SetActive(false);
        vedioC.SetActive(true);
    }
}