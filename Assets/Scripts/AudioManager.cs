using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("#BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelindex;

    public enum BgmType
    {
        Town,Dungeon
    }
    public enum Sfx
    {
        Sword,Arrow,Hit,Walk,Click,Cancel,Cash,Dash,Heal,Dialogue,StrongHit,Treasure,Drop
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
        bgmVolume = PlayerPrefs.HasKey("BGM") ? PlayerPrefs.GetFloat("BGM") : 0.5f;
        sfxVolume = PlayerPrefs.HasKey("SFX") ? PlayerPrefs.GetFloat("SFX") : 0.5f;

        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.loop = true;

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
    }

    public void PlaySfx(Sfx sfx)
    {
        AudioSource temp = gameObject.AddComponent<AudioSource>();
        temp.clip = sfxClip[(int)sfx];
        temp.volume = sfxVolume;
        temp.Play();

        Destroy(temp, temp.clip.length);
    }

    public void PlayBgm(BgmType type)
    {
        int index = (int)type;

        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogWarning("BGM 없음");
            return;
        }

        AudioClip clip = bgmClips[index];

        if (bgmPlayer.clip == clip && bgmPlayer.isPlaying)
            return;

        StartCoroutine(FadeBgm(clip));
    }

    IEnumerator FadeBgm(AudioClip newClip)
    {
        float t = bgmVolume;

        while (bgmPlayer.volume > 0.01f)
        {
            bgmPlayer.volume -= Time.deltaTime;
            yield return null;
        }

        bgmPlayer.volume = 0f;
        bgmPlayer.clip = newClip;
        bgmPlayer.Play();

        while (bgmPlayer.volume < t)
        {
            bgmPlayer.volume += Time.deltaTime;
            yield return null;
        }
    }

    public void SetBGM(float value)
    {
        bgmVolume = value;
        if (bgmPlayer != null) bgmPlayer.volume = bgmVolume;

        PlayerPrefs.SetFloat("BGM", bgmVolume);
    }

    public void SetSFX(float value)
    {
        sfxVolume = value;

        foreach (var sfx in sfxPlayers)
        {
            if (sfx != null) sfx.volume = sfxVolume;
        }

        PlayerPrefs.SetFloat("SFX", sfxVolume);
    }
    void ApplyVolume()
    {
        SetBGM(bgmVolume);
        SetSFX(sfxVolume);
    }
}
