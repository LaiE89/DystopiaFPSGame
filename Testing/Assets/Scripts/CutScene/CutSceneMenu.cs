using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneMenu : MonoBehaviour {
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public static bool pausedGame = false;

    private void Start() {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        pausedGame = false;
        Time.timeScale = 1;
    }

    private void Update() {
        if (Input.GetKeyDown(pauseKey) && !pausedGame) {
            CutSceneController.Instance.soundController.StopAll();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Pause();
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Player.PlayerMovement.SettingChanges();
        Time.timeScale = 1;
        pausedGame = false;
    }

    public void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void Skip() {
        NextLevel();
    }

    IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        CutSceneController.Instance.loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            CutSceneController.Instance.slider.value = progress;
            CutSceneController.Instance.progressText.text = progress * 100f + "%";
            yield return null;
        }
    }
    
    public void NextLevel() {
        SceneController.sceneIndex += 1;
        Debug.Log(SceneController.sceneIndex);
        if (SceneController.sceneIndex >= SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(0);
        }else {
            StartCoroutine(LoadAsyncronously(SceneController.sceneIndex));
        }
    }
}