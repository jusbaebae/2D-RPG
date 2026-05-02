using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ResolutionSetting : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    List<Resolution> sortedResolutions = new List<Resolution>();
    void Start()
    {
        var rawResolutions = Screen.resolutions.OrderByDescending(res => res.width).ThenByDescending(res => res.height).ToList();
        Resolution currentRes = Screen.currentResolution;

        resolutionDropdown.ClearOptions();

        sortedResolutions.Add(currentRes);

        foreach (var res in rawResolutions)
        {
            //현재 해상도와 동일한 항목은 건너뛰고 추가 (중복 방지)
            if (res.width == currentRes.width && res.height == currentRes.height)
                continue;

            sortedResolutions.Add(res);
        }

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < sortedResolutions.Count; i++)
        {
            string option = sortedResolutions[i].width + " X " + sortedResolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        LoadResolution();
    }

    public void SetResolution()
    {
        int index = resolutionDropdown.value;
        Resolution targetRes = sortedResolutions[index];

        Screen.SetResolution(targetRes.width, targetRes.height, FullScreenMode.Windowed);
        SaveResolution(index);
        Debug.Log("해상도 적용 : " + targetRes.width + " x " + targetRes.height);
    }

    private void SaveResolution(int index) //해상도 저장
    {
        PlayerPrefs.SetInt("SavedResolutionIndex", index);
        PlayerPrefs.Save(); // 디스크에 즉시 기록
    }

    private void LoadResolution() //해상도 로드
    {
        //저장된 값이 없으면 현재해상도로
        int savedIndex = PlayerPrefs.GetInt("SavedResolutionIndex", 0);

        //드롭다운 UI 값 동기화
        if (savedIndex < sortedResolutions.Count)
        {
            resolutionDropdown.value = savedIndex;
            resolutionDropdown.RefreshShownValue();

            // 실제 해상도 적용
            Resolution targetRes = sortedResolutions[savedIndex];
            Screen.SetResolution(targetRes.width, targetRes.height, FullScreenMode.Windowed);
        }
    }
}
