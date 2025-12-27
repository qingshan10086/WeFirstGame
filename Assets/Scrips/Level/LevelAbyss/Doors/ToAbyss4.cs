using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToAbyss4 : MonoBehaviour
{
    public Transform player;
    public float goalRaidus = 3;

    public void To4Scene()
    {
        SceneManager.LoadScene("Abyss4");
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
            To4Scene();
        }
    }
}
