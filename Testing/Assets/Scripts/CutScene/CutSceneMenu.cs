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
            SceneController.Instance.soundController.PauseAll();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Pause();
        }
    }

    public void Resume() {
        SceneController.Instance.soundController.Play("UI Click");
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        pausedGame = false;
    }

    public void Pause() {
        SceneController.Instance.soundController.Play("UI Click");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void Skip() {
        SceneController.Instance.soundController.Play("UI Click");
        NextLevel();
    }

    public void PlayUISound() {
        SceneController.Instance.soundController.Play("UI Click");
    }

    IEnumerator LoadAsyncronously (int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        SceneController.Instance.loadingScreen.SetActive(true);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            SceneController.Instance.loadingSlider.value = progress;
            SceneController.Instance.progressText.text = progress * 100f + "%";
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
