using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

public class MainMenu : MonoBehaviour {
    [SerializeField] public OptionsMenu options;
    [SerializeField] public GameObject confirmationScreen;
    [SerializeField] public GameObject difficultyScreen; 
    [SerializeField] public GameObject loadingScreen; 
    [SerializeField] public Slider slider;
    [SerializeField] public TextMeshProUGUI progressText;
    [SerializeField] public GameObject devNote;
    private Coroutine AnimationCoroutine;
    public static bool loading;
    public static bool saving;
    public static bool finishedGame;
    public static SoundController soundController;

    [Header("Player Default Values")]
    [SerializeField] float playerHealth = 5;
    [SerializeField] float playerHunger = 100;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] int playerDrugs = 1;
    [SerializeField] int playerAmmo = 0;
    [SerializeField] int pickUpRange = 3;
    [SerializeField] int sceneIndex = 1;
    [SerializeField] List<string> statusEffects = new List<string>();
    [SerializeField] string myWeapon = "Fist";

    private void Awake() {
        if (finishedGame) {
            devNote.SetActive(true);
            finishedGame = false;
        }
        saving = false;
        soundController = GameObject.Find("Sound Controller").GetComponent<SoundController>();
    }

    private void Start() {
        soundController.Play("Menu Song");
        options.InitializeSettings();
    }

    public void PlayGame() {
        string path = Application.persistentDataPath + "/player.dat";
        soundController.Play("UI Click");
        if (File.Exists(path)) {
            confirmationScreen.SetActive(true);
        }else {
            // StartingNewPlayer();
            difficultyScreen.SetActive(true);
        }
    }

    /*IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            progressText.SetText($"{(progress * 100).ToString("N2")}%");
            yield return null;
        }
    }*/

    IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        slider.value = 0;
        float time = 0;
        operation.allowSceneActivation = false;
        while (slider.value < 1f || time < 1f) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = Mathf.Lerp(slider.value, progress, time);
            time += Time.unscaledDeltaTime;
            progressText.SetText($"{(slider.value * 100).ToString("N2")}%");
            yield return null;
        }
        operation.allowSceneActivation = true;
    }

    public void QuitGame() {
        print("Quit!");
        Application.Quit();
    }

    public void LoadGame() {
        soundController.Play("UI Click");
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

    public void PlayUISound() {
        soundController.Play("UI Click");
    }

    public void NewPlayer(int difficulty) {
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/player.dat";

        Player.PlayerData data = new Player.PlayerData(playerHealth, playerHunger, walkSpeed, sprintSpeed, jumpForce, playerDrugs, playerAmmo, pickUpRange, sceneIndex, difficulty, statusEffects, myWeapon);
        
        var serializer = new DataContractSerializer(typeof(Player.PlayerData));
        var settings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
        };
        var writer = XmlWriter.Create(path, settings);
        serializer.WriteObject(writer, data);
        writer.Close();
    }

    public void StartingNewPlayer(int difficulty) {
        string path = Application.persistentDataPath + "/player.dat";
        DeleteFile();
        NewPlayer(difficulty);
        StartCoroutine(LoadAsyncronously(1));
        if (File.Exists(path)) {
            loading = true;
        }
    }
}
