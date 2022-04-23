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
    InventoryManager inventoryManager;

    private void Start() {
        inventoryManager = FindObjectOfType<InventoryManager>();
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        deathScreenUI.SetActive(false);
        inGameUI.SetActive(true);
        pausedGame = false;
        Time.timeScale = 1;
    }

    private void Update() {
        if (Input.GetKeyDown(pauseKey) && !pausedGame) {
            if (inventoryManager != null && InventoryManager.instance.gameObject.activeSelf) {
                foreach (InventorySlot slot in InventoryManager.instance.slots) {
                    slot.descriptionBox.gameObject.SetActive(false);
                }
            }
            SceneController.Instance.soundController.PauseAll();
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
        SceneController.Instance.soundController.Play("UI Click");
    }

    public void Pause() {
        SceneController.Instance.soundController.Play("UI Click");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
    }

    public void BackToMainMenu() {
        // soundController.Play("UI Click");
        SceneManager.LoadScene("Menu");
    }

    public void PlayUISound() {
        SceneController.Instance.soundController.Play("UI Click");
    }

    public void Respawn() {
        SceneController.Instance.soundController.Play("UI Click");
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
