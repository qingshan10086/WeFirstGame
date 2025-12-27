using UnityEngine;

public class AnimatorForceTest : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Rebind();
        anim.Update(0f);
        anim.Play("Idle", 0, 0f);
    }
}
