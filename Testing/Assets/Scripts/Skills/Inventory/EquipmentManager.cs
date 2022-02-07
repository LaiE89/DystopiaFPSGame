using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    public SkillsObject[] currentSkills;
    public Transform slotsParent;
    public EquipmentSlot[] slots;

    public static EquipmentManager instance;
    
    void Awake() {
        instance = this;
    }

    void Start() {
        currentSkills = new SkillsObject[3];
        slots = slotsParent.GetComponentsInChildren<EquipmentSlot>();
        UpdateUI();
    }

    public void Equip (SkillsObject newSkill) {
        for (int i = 0; i < currentSkills.Length; i++) {
            if (currentSkills[i] == null) {
                currentSkills[i] = newSkill; 
                UpdateUI();
                break;
            }
        }
    }

    public void Unequip (int slotIndex) {
        if (currentSkills[slotIndex] != null) {
            currentSkills[slotIndex] = null;
            UpdateUI();
        }
    }

    public void UpdateUI() {
        for (int i = 0; i < slots.Length; i++) {
            if (i < currentSkills.Length) {
                if (currentSkills[i] != null) {
                    slots[i].AddSkill(currentSkills[i]);
                }else {
                    slots[i].ClearSlot();
                }
            }else {
                slots[i].ClearSlot();
            }
        }
    }
}
