using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour {
    [SerializeField] public OptionsMenu options; 
    [SerializeField] public GameObject loadingScreen; 
    [SerializeField] public Slider slider;
    [SerializeField] public TextMeshProUGUI progressText;
    public static bool loading;
    public static bool saving;

    private void Awake() {
        options.InitializeSettings();
        saving = false;
    }

    public void PlayGame() {
        DeleteFile();
        StartCoroutine(LoadAsyncronously(1));
    }

    IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    public void QuitGame() {
        print("Quit!");
        Application.Quit();
    }

    public void LoadGame() {
        string path = Application.persistentDataPath + "/player.dat";
        if (File.Exists(path)) {
            int currentLevel = Player.SaveSystem.LoadPlayer().sceneIndex;
            if (currentLevel >= SceneManager.sceneCountInBuildSettings) {
                StartCoroutine(LoadAsyncronously(currentLevel - 1));
            }else {
                StartCoroutine(LoadAsyncronously(currentLevel));
            }
            loading = true;
        }else {
            print("New game!");
            PlayGame();
        }
    }

    public void DeleteFile() {
        string path = Application.persistentDataPath + "/player.dat";
        if (!File.Exists(path)){
            Debug.Log("no " + path + " file exists");
        }else {
            Debug.Log(path + " file exists, deleting...");
             
            File.Delete(path);
             
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }
    }
}
