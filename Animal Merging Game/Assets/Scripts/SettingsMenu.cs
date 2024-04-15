using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Resolution")]
   // Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    [Header("Audio")]
    public AudioMixer musicAudioMixer;
    public Slider musicSlider;
    public AudioMixer sfxAudioMixer;
    public Slider sfxSlider;

    [Header("Other")]
    private JsonPlayerPrefs jsonPlayerSettings;

    void Awake()
    {
        // Initialize JsonPlayerPrefs with the appropriate file path
        string jsonFilePath;
#if UNITY_EDITOR
        jsonFilePath = Application.persistentDataPath + "/EditorPlayerSettings.json";
#else
    jsonFilePath = Application.persistentDataPath + "/PlayerSettings.json";
#endif


        jsonPlayerSettings = new JsonPlayerPrefs(jsonFilePath);

        //GetResolution();
        LoadValues();
    }

    private void LoadValues()
    {
        // Load sfx volume
        if (jsonPlayerSettings.HasKey("SFXVolume"))
        {
            float sfxVolume = jsonPlayerSettings.GetFloat("SFXVolume");
            StartCoroutine(SetSFXVolumeDelayed(sfxVolume));
            sfxSlider.value = sfxVolume;
        }
        // Load music volume
        if (jsonPlayerSettings.HasKey("MusicVolume"))
        {
            float musicVolume = jsonPlayerSettings.GetFloat("MusicVolume");
            StartCoroutine(SetMusicVolumeDelayed(musicVolume));
            musicSlider.value = musicVolume;
        }

        //Load fullscreen value
        if (jsonPlayerSettings.HasKey("isFullscreen"))
        {
            int isFull = jsonPlayerSettings.GetInt("isFullscreen");
            if (isFull == 1)
            {
                fullScreenToggle.isOn = true;
            }
            else fullScreenToggle.isOn = false;

            SetFullscreen(fullScreenToggle.isOn);
        }
    }

    private IEnumerator SetSFXVolumeDelayed(float volume)
    {
        yield return new WaitForSeconds(0.1f); // You can adjust the delay time if needed
        SetSFXVolume(volume);
    }

    private IEnumerator SetMusicVolumeDelayed(float volume)
    {
        yield return new WaitForSeconds(0.1f); // You can adjust the delay time if needed
        SetMusicVolume(volume);
    }



    // private void GetResolution()
    // {
    //     resolutions = Screen.resolutions;

    //     resolutionDropdown.ClearOptions();

    //     int currentResIndex = 0;

    //     List<string> resolutionNames = new List<string>();

    //     for (int i = 0; i < resolutions.Length; i++)
    //     {
    //         string option = resolutions[i].width + "x" + resolutions[i].height;
    //         resolutionNames.Add(option);

    //         if (resolutions[i].width == Screen.currentResolution.width
    //         && resolutions[i].height == Screen.currentResolution.height) //??? why do they make it like this??
    //         {
    //             currentResIndex = i;
    //         }
    //     }

    //     resolutionDropdown.AddOptions(resolutionNames);
    //     resolutionDropdown.value = currentResIndex;
    //     resolutionDropdown.RefreshShownValue();
    //     //why do they make it so haaard
    // }


    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.SetFloat("Volume", volume);
        jsonPlayerSettings.SetFloat("MusicVolume", volume);
        jsonPlayerSettings.Save();
    }
    public void SetSFXVolume(float volume)
    {
        sfxAudioMixer.SetFloat("Volume", volume);
        jsonPlayerSettings.SetFloat("SFXVolume", volume);
        jsonPlayerSettings.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        int full;
        if (isFullscreen)
        {
            full = 1;
        }
        else full = 0;

        Screen.fullScreen = isFullscreen;

        jsonPlayerSettings.SetInt("isFullscreen", full);
        jsonPlayerSettings.Save();
    }

    // public void SetResolution(int resolutionIndex)
    // {
    //     Resolution resolution = resolutions[resolutionIndex];

    //     Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        
        
    //     Debug.Log(jsonPlayerSettings.GetInt("ResWidth") + "x" + jsonPlayerSettings.GetInt("ResHeight"));


    //     jsonPlayerSettings.SetInt("ResWidth", resolution.width);
    //     jsonPlayerSettings.SetInt("ResHeight", resolution.height);
    //     jsonPlayerSettings.Save();
    // }
}
