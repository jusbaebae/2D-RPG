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

        HashSet<string> added = new HashSet<string>();

        resolutionDropdown.ClearOptions();
        sortedResolutions.Add(currentRes);

        string currentKey = currentRes.width + "x" + currentRes.height;
        added.Add(currentKey);

        foreach (var res in rawResolutions)
        {
            float ratio = (float)res.width / res.height;
            
            if (Mathf.Abs(ratio - (16f / 9f)) > 0.01f)
                continue;

            string key = res.width + "x" + res.height;

            //이미 같은 해상도 추가했으면 스킵
            if (added.Contains(key))
                continue;

            sortedResolutions.Add(res);
            added.Add(key);
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
