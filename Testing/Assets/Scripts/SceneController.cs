using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour {
    // Declare any public variables that you want to be able 
    // to access throughout your scene
    [SerializeField] public GameObject loadingScreen; 
    [SerializeField] public OptionsMenu optionsMenu;
    [SerializeField] public Slider loadingSlider;
    [SerializeField] public TextMeshProUGUI progressText;
    [SerializeField] public TextMeshProUGUI savingText;
    [SerializeField] public ParticleSystem bloodParticles;
    [SerializeField] public ParticleSystem groundParticles;
    [SerializeField] public ParticleSystem burningParticles;
    [SerializeField] public bool isCutscene;

    [Header("Singletons")]
    public GameObject canvas;
    public SoundController soundController;
    public DialogueController dialogueController;
    public DialogueController objectivesController;
    public Player.PlayerMovement player;
    public GameObject playerObject;
    public List<Enemies.Movement> listOfEnemies;
    //public Enemies.Movement[] listOfEnemies;

    public static int sceneIndex;
    public static SceneController Instance { get; private set; } // static singleton

    void Awake() {
        // Singleton Stuff
        if (Instance == null) { 
            Instance = this;
        }else { 
            Destroy(gameObject);
        }

        // Cache references to all desired variables
        canvas = GameObject.Find("Canvas");
        soundController = FindObjectOfType<SoundController>();
        player = FindObjectOfType<Player.PlayerMovement>();
        playerObject = GameObject.Find("Player");
        dialogueController = GameObject.Find("Dialogue Controller").GetComponent<DialogueController>();
        objectivesController = GameObject.Find("Objectives Controller").GetComponent<DialogueController>();

        // Scene begin stuff
        if (isCutscene) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }else {
            listOfEnemies.Clear();
            //Array.Clear(listOfEnemies, 0, listOfEnemies.Length);
            if (MainMenu.saving) {
                StartCoroutine(FadeOutSaving());
                MainMenu.saving = false;
            }
        }
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        foreach (Transform child in canvas.transform) {
            switch (child.name) {
                case "Loading Screen":
                    loadingScreen = child.gameObject;
                    loadingSlider = loadingScreen.transform.GetComponentInChildren<Slider>();
                    progressText = loadingScreen.transform.GetComponentInChildren<TextMeshProUGUI>();
                    break;
                case "Saving Text":
                    savingText = child.GetComponent<TextMeshProUGUI>();
                    break;
                case "Options Menu":
                    optionsMenu = child.GetComponent<OptionsMenu>();
                    break;
                default:
                    break;
            }
        }
    }

    void Start() {
        optionsMenu.InitializeSettings();
        //listOfEnemies = FindObjectsOfType<Enemies.Movement>() as Enemies.Movement[];
    }

    IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    IEnumerator FadeOutSaving () {
        savingText.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(3f);
        for (float i = 1; i >= 0; i -= Time.deltaTime) {
            savingText.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    public void NextLevel() {
        sceneIndex += 1;
        if (sceneIndex >= SceneManager.sceneCountInBuildSettings) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }else {
            MainMenu.loading = true;
            StartCoroutine(LoadAsyncronously(sceneIndex));
        }
        player.SavePlayer();
        MainMenu.saving = true;
    }
}