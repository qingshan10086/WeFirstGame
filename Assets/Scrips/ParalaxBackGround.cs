using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBackGround : MonoBehaviour//该类用来让背景随着主角移动而移动
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect;

    private float xPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
       

        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

       
    }
}
