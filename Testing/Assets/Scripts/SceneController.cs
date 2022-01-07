using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour {
   // Declare any public variables that you want to be able 
   // to access throughout your scene
   [SerializeField] public GameObject loadingScreen; 
   [SerializeField] public Slider slider;
   [SerializeField] public TextMeshProUGUI progressText;
   [SerializeField] public TextMeshProUGUI savingText;

   [Header("Singletons")]
   public SoundController soundController;
   public Player.PlayerMovement player;
   public GameObject playerObject;

   public static int sceneIndex;
   public static SceneController Instance { get; private set; } // static singleton

    void Awake() {
        // Scene begin stuff
        if (MainMenu.saving) {
            StartCoroutine(FadeOutSaving());
            MainMenu.saving = false;
        }
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Current Scene Index: " + sceneIndex);

        // Singleton Stuff
        if (Instance == null) { 
            Instance = this;
        }else { 
            Destroy(gameObject);
        }

        // Cache references to all desired variables
        soundController = FindObjectOfType<SoundController>();
        player = FindObjectOfType<Player.PlayerMovement>();
        playerObject = GameObject.Find("Player");
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
        MainMenu.saving = true;
        player.SavePlayer();
    }
}