using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToBoss : MonoBehaviour
{
    public Transform player;
    public float goalRaidus=3;

    public void ToBossScene()
    {
        SceneManager.LoadScene("BossDeath");
    }

    private void Start()
    {

    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < goalRaidus)
        {
            ToBossScene();
        }
    }
}
