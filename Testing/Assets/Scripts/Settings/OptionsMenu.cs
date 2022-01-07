using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class OptionsMenu : MonoBehaviour {
    public static float sens;
    public static int qualityIndex;
    [SerializeField] Slider sensSlider;
    [SerializeField] TMP_Dropdown qualityDropdown;

    private void Start() {
        InitializeSettings();
    }

    public void InitializeSettings() {
        //sens = Player.PlayerMovement.newSens;
        //sensSlider.value = sens;
        string path = Application.persistentDataPath + "/settings.dat";
        if (File.Exists(path)) {
            OptionsData settings = OptionsSaveSystem.LoadSettings();

            sens = settings.sens;
            qualityIndex = settings.qualityIndex;

            sensSlider.value = settings.sens;
            qualityDropdown.value = settings.qualityIndex;
        }else {
            sens = 60;
            sensSlider.value = 60;
            qualityIndex = 2;
            qualityDropdown.value = 2;
        }
    }

    public void AdjustSensitivity(float newSens) {
        sens = newSens;
    }

    public void ChangeQuality(int index) {
        qualityIndex = index;
        QualitySettings.SetQualityLevel(index, false);
    }

    public void SaveSettings() {
        OptionsSaveSystem.SaveSettings();
    }
}
