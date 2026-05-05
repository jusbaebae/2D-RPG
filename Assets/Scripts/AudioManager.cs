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
        Sword,Arrow,Hit,Walk,Click,Cancel,Cash,Dash,Heal,Dialogue,StrongHit,Treasure,Drop,Levelup
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
        bgmVolume = PlayerPrefs.GetFloat("BGM", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFX", 0.5f);

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

        RefreshVolume();
    }

    void RefreshVolume()
    {
        if (bgmPlayer != null) bgmPlayer.volume = bgmVolume;
        foreach (var sfx in sfxPlayers)
        {
            if (sfx != null) sfx.volume = sfxVolume;
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (channelindex + i) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying) continue;

            channelindex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClip[(int)sfx];
            sfxPlayers[loopIndex].volume = sfxVolume; // 현재 볼륨 적용
            sfxPlayers[loopIndex].Play();
            return;
        }
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
