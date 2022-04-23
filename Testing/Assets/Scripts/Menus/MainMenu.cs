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
    [SerializeField] public GameObject loadingScreen; 
    [SerializeField] public Slider slider;
    [SerializeField] public TextMeshProUGUI progressText;
    public static bool loading;
    public static bool saving;
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
        saving = false;
        soundController = GameObject.Find("Sound Controller").GetComponent<SoundController>();
    }

    private void Start() {
        options.InitializeSettings();
    }

    public void PlayGame() {
        soundController.Play("UI Click");
        DeleteFile();
        NewPlayer();
        StartCoroutine(LoadAsyncronously(1));
        string path = Application.persistentDataPath + "/player.dat";
        if (File.Exists(path)) {
            loading = true;
        }
    }

    IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
            yield return null;
        }
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

    public void NewPlayer() {
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/player.dat";

        Player.PlayerData data = new Player.PlayerData(playerHealth, playerHunger, walkSpeed, sprintSpeed, jumpForce, playerDrugs, playerAmmo, pickUpRange, sceneIndex, statusEffects, myWeapon);
        
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
}
