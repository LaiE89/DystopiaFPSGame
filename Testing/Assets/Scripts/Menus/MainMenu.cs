using System.Collections;
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

    [Header("Player Default Values")]
    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float sprintSpeed = 4f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] int playerDrugs = 1;
    [SerializeField] int pickUpRange = 3;
    [SerializeField] int sceneIndex = 1;
    [SerializeField] string myWeapon = "Fist";

    private void Awake() {
        options.InitializeSettings();
        saving = false;
    }

    public void PlayGame() {
        DeleteFile();
        NewPlayer();
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

    public void NewPlayer() {
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/player.dat";

        Player.PlayerData data = new Player.PlayerData(walkSpeed, sprintSpeed, jumpForce, playerDrugs, pickUpRange, sceneIndex, myWeapon);
        
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
