using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class GrassTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prx;
    private pallrex prxScript;
    public AudioClip grassSound;
    public float fadeDuration = 1f;
    private void Awake()
    {
        prxScript = prx.GetComponent<pallrex>();
    }
    
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
                           BloodMusicManager.Instance.SwitchBackgroundMusic(grassSound,fadeDuration);
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入了草地");
            SceneFade.Instance.QuickFadeIn();
        }
        
        prxScript.sign = !prxScript.sign;
    }
}
