using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI bgmnum;
    public TextMeshProUGUI sfxnum;

    private void Start()
    {
        // 저장값 불러오기
        bgmSlider.value = PlayerPrefs.GetFloat("BGM", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);

        // 초기 적용
        AudioManager.Instance.SetBGM(bgmSlider.value);
        AudioManager.Instance.SetSFX(sfxSlider.value);

        // 이벤트 연결
        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGM);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFX);

    }

    void Update()
    {
        bgmnum.text = ((int)(AudioManager.Instance.bgmVolume * 100)).ToString();
        sfxnum.text = ((int)(AudioManager.Instance.sfxVolume * 100)).ToString();
    }
}
