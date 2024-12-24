using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioSource vfxSound;
    public AudioSource bgm;
    public AudioSource getBGM => bgm;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHitSound()
    {
        vfxSound.PlayOneShot(hitSound);
    }

    public void PlayMissSound()
    {
        vfxSound.PlayOneShot(missSound);
    }

    public void PlayBGM()
    {
        bgm.Play();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
