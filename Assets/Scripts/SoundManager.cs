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

    private bool isBGMPlaying = false; // Track if BGM is currently playing

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

    private void Start()
    {
        bgm.clip = GameDataManager.Instance.settings.bgm;
        bgm.volume = GameDataManager.Instance.settings.volumeMaster;
        vfxSound.volume = GameDataManager.Instance.settings.volumeVFX;
        PlayBGM();
    }

    private void Update()
    {
        if (isBGMPlaying && !bgm.isPlaying)
        {
            isBGMPlaying = false;
            if (UIManager.Instance != null)
            {
                // UIManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("UIManager Instance is null, cannot trigger GameOver!");
            }
        }
        isBGMPlaying = bgm.isPlaying;
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
        isBGMPlaying = true;
    }

    public void PauseBGM()
    {
        bgm.Pause();
        isBGMPlaying = false;
    }

    public void ResumeBGM()
    {
        bgm.UnPause();
        isBGMPlaying = true;
    }
}