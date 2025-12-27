using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToAbyss2 : MonoBehaviour
{
    public Transform player;
    public float goalRaidus = 3;

    public void To2Scene()
    {
        SceneManager.LoadScene("Abyss2");
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
            To2Scene();
        }
    }
}
