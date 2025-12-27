using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToAbyss3 : MonoBehaviour
{
    public Transform player;
    public float goalRaidus = 3;

    public void ToOriginalScene()
    {
        SceneManager.LoadScene("Abyss3");
    }

    private void Start()
    {

    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < goalRaidus)
        {
            ToOriginalScene();
        }
    }
}
