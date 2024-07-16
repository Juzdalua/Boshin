using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Range(0, 1)][SerializeField] private float bgmVolume = 0.5f;
    public float BgmVolume
    {
        get { return bgmVolume; }
        set { bgmVolume = value; }
    }
    [Range(0, 1)][SerializeField] private float sfxVolume = 0.5f;
    public float SfxVolume
    {
        get { return sfxVolume; }
        set { sfxVolume = value; }
    }

    [SerializeField] private AudioSource bgmSound;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip battleAudioClip;
    [SerializeField] private AudioClip playerDeadAudioClip;

    void Update()
    {
        if (bgmSound) bgmSound.volume = bgmVolume;
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject sound = new GameObject(sfxName + "Sound");
        AudioSource audioSource = sound.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = sfxVolume;
        audioSource.loop = false;
        audioSource.Play();

        GameObject.Destroy(sound, clip.length);
    }

    public void BgmSoundPlay(string bgmName, AudioClip clip)
    {
        if (bgmSound != null && bgmSound.clip != null)
        {
            if (clip.name == bgmSound.clip.name) return;
        }

        if (bgmName != null)
        {
            GameObject.Destroy(bgmSound);
        }
        GameObject sound = new GameObject(bgmName);

        bgmSound = sound.AddComponent<AudioSource>();
        bgmSound.clip = clip;
        bgmSound.loop = true;
        bgmSound.volume = bgmVolume;
        bgmSound.Play();
    }

    public void BattleSfxPlay()
    {
        if (battleAudioClip != null) SFXPlay("Battle", battleAudioClip);
    }

    public void PlayerDeadSfxPlay()
    {
        if (playerDeadAudioClip != null) SFXPlay("PlayerDead", playerDeadAudioClip);
    }
}
