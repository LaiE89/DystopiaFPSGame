using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public SkillsInventory inventory;
    public Transform slotsParent;
    public InventorySlot[] slots;

    public static InventoryManager instance;
    
    void Awake() {
        instance = this;
    }
    
    void Start() {
        SceneController.Instance.playerObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
    }

    public void UpdateUI() {
        for (int i = 0; i < slots.Length; i++) {
            if (i < inventory.Container.Count) {
                slots[i].AddSkill(inventory.Container[i]);
            }else {
                slots[i].ClearSlot();
            }
        }
    }

    public void Finished() {
        SceneController.Instance.soundController.Play("UI Click");
        SceneController.Instance.playerObject.SetActive(true);
        SceneController.Instance.player.PlayerStartManually();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        this.gameObject.SetActive(false);
    }
}
