using UnityEngine;
using UnityEngine.UI;

public class clickSound : MonoBehaviour
{
    public GameObject sound;

    private AudioClip click;
    private AudioSource audioSource;

    void Start()
    {
        click = Resources.Load<AudioClip>("UI/sound/click");
        audioSource = sound.GetComponent<AudioSource>();
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (click != null)
        {
            audioSource.PlayOneShot(click);
        }
    }
}