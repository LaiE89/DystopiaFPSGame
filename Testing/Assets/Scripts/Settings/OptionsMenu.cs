using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class OptionsMenu : MonoBehaviour {
    public static float sens;
    public static int qualityIndex;
    public static float volume;
    public static int resolutionIndex;
    public static bool isFullscreen;
    Resolution[] resolutions;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullScreenToggle;
    [SerializeField] AudioMixer audioMixer;

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
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Loading settings
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

        }else {
            sens = 60;
            sensSlider.value = 60;

            qualityIndex = 2;
            qualityDropdown.value = 2;

            volume = 0;
            volumeSlider.value = 0;

            resolutionIndex = 0;
            resolutionDropdown.value = 0;

            isFullscreen = true;
            fullScreenToggle.isOn = true;
        }
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

    public void ChangeQuality(int newIndex) {
        qualityIndex = newIndex;
        QualitySettings.SetQualityLevel(newIndex, false);
    }

    public void SaveSettings() {
        OptionsSaveSystem.SaveSettings();
    }
}
