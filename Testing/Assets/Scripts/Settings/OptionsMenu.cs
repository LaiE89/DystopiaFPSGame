using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System;
using System.IO;

public class OptionsMenu : MonoBehaviour {
    public static float sens;
    public static int qualityIndex;
    public static float volume;
    public static int resolutionIndex;
    public static bool isFullscreen;
    public static float brightness;
    public static int targetFPSIndex;
    Resolution[] resolutions;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullScreenToggle;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown targetFPSDropdown;

    private void Start() {
        InitializeSettings();
    }

    public void InitializeSettings() {

        // Getting Resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Loading settings
        LoadSettings();
    }

    public void SetVolume(float newVolume) {
        volume = newVolume;
        audioMixer.SetFloat("volume", Mathf.Log10(newVolume) * 20);
    }

    public void SetResolution(int newResolutionIndex) {
        resolutionIndex = newResolutionIndex;
        Resolution resolution = resolutions[newResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool newFullscreen) {
        isFullscreen = newFullscreen;
        Screen.fullScreen = newFullscreen;
    }

    public void AdjustSensitivity(float newSens) {
        sens = newSens;
    }

    public void SetBrightness(float newBrightness) {
        brightness = newBrightness;
        RenderSettings.ambientLight = new Color(brightness, brightness, brightness, 1.0f);
    }

    public void ChangeQuality(int newIndex) {
        qualityIndex = newIndex;
        QualitySettings.SetQualityLevel(newIndex, false);
    }

    public void SetTargetFPS(int newIndex) {
        targetFPSIndex = newIndex;
        switch (newIndex) {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 120;
                break;
            case 3:
                Application.targetFrameRate = -1;
                break;
            default:
                Application.targetFrameRate = -1;
                break; 
        }
    }

    public void SaveSettings() {
        OptionsSaveSystem.SaveSettings();
    }

    public void LoadSettings() {
        string path = Application.persistentDataPath + "/settings.dat";
        if (File.Exists(path)) {
            OptionsData settings = OptionsSaveSystem.LoadSettings();

            sens = settings.sens;
            sensSlider.value = settings.sens;

            qualityIndex = settings.qualityIndex;
            qualityDropdown.value = settings.qualityIndex;

            volume = settings.volume;
            volumeSlider.value = settings.volume;

            resolutionIndex = settings.resolutionIndex;
            resolutionDropdown.value = settings.resolutionIndex;

            isFullscreen = settings.isFullscreen;
            fullScreenToggle.isOn = settings.isFullscreen;

            brightness = settings.brightness;
            brightnessSlider.value = settings.brightness;

            targetFPSIndex = settings.targetFPSIndex;
            targetFPSDropdown.value = settings.targetFPSIndex;

            ControlsMenu.keybinds = settings.keybinds;

        }else {
            sens = 60;
            sensSlider.value = 60;

            qualityIndex = 2;
            qualityDropdown.value = 2;

            volume = 1;
            volumeSlider.value = 1;

            resolutionIndex = resolutions.Length;
            resolutionDropdown.value = resolutions.Length;

            isFullscreen = true;
            fullScreenToggle.isOn = true;

            brightness = 0.1f;
            brightnessSlider.value = 0.1f;

            targetFPSIndex = 3;
            targetFPSDropdown.value = 3;

            ControlsMenu.keybinds = new Dictionary<string, KeyCode>();
            ControlsMenu.DefaultKeybinds();
        }
        AdjustSensitivity(sens);
        ChangeQuality(qualityIndex);
        SetVolume(volume);
        SetResolution(resolutionIndex);
        SetFullscreen(isFullscreen);
        SetBrightness(brightness);
        SetTargetFPS(targetFPSIndex);
    }
}
