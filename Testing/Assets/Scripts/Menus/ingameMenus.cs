using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ingameMenus : MonoBehaviour {
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject inGameUI;
    public GameObject deathScreenUI;
    public static bool pausedGame = false;

    private void Start() {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        deathScreenUI.SetActive(false);
        inGameUI.SetActive(true);
        pausedGame = false;
        Time.timeScale = 1;
    }

    private void Update() {
        if (Input.GetKeyDown(pauseKey) && !pausedGame) {
            inGameUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Pause();
        }
    }

    public void Resume() {
        inGameUI.SetActive(true);
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

    public void Respawn() {
        string path = Application.persistentDataPath + "/player.dat";
        if (File.Exists(path)){
            MainMenu.loading = true;
        }
        SceneManager.LoadScene(SceneController.sceneIndex);
    }

    public void ToggleDeathScreen() {
        inGameUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DeathPause();
    }

    public void DeathPause() {
        deathScreenUI.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
    }
}
