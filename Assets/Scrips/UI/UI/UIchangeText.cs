using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIchangeText : MonoBehaviour
{
    Animator animator;
    bool bool1 = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        animator.SetBool("isText1", bool1);
    }
    public void change()
    {
        bool1 = !bool1;

    }
}
