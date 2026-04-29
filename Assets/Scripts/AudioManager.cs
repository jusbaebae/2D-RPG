using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelindex;

    public enum Sfx
    {
        Sword,Arrow,Hit,Walk,Click,Cancel
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }
    void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.clip = bgmClip;

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
        }

        ApplyVolume();
        PlayBgm(true);
    }

    public void PlaySfx(Sfx sfx)
    {
        AudioSource temp = gameObject.AddComponent<AudioSource>();
        temp.clip = sfxClip[(int)sfx];
        temp.volume = sfxVolume;
        temp.Play();

        Destroy(temp, temp.clip.length);
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            if (bgmPlayer.clip != null && !bgmPlayer.isPlaying)
            {
                bgmPlayer.Play();
            }
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void SetBGM(float value)
    {
        bgmVolume = value;
        if (bgmPlayer != null)
            bgmPlayer.volume = bgmVolume;

        PlayerPrefs.SetFloat("BGM", bgmVolume);
    }

    public void SetSFX(float value)
    {
        sfxVolume = value;

        foreach (var sfx in sfxPlayers)
        {
            if (sfx != null)
                sfx.volume = sfxVolume;
        }

        PlayerPrefs.SetFloat("SFX", sfxVolume);
    }
    void ApplyVolume()
    {
        SetBGM(bgmVolume);
        SetSFX(sfxVolume);
    }
}
