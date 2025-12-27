using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToEcho3 : MonoBehaviour
{
    public Transform player;
    public float goalRaidus = 3;

    public void ToEcho3Scene()
    {
        SceneManager.LoadScene("Echo3");
    }

    private void Start()
    {

    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < goalRaidus)
        {
            Debug.Log("½øÈëAbyss2");
            ToEcho3Scene();
        }
    }
}
