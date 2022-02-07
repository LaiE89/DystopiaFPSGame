using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour {
    public Image icon;
    SkillsObject skill;

    public void AddSkill (SkillsObject newSkill) {
        skill = newSkill;
        icon.sprite = newSkill.icon;
        icon.enabled = true;
    }

    public void ClearSlot() {
        skill = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void Unequip() {
        for (int i = 0; i < EquipmentManager.instance.slots.Length; i++) {
            if (EquipmentManager.instance.slots[i] == this) {
                EquipmentManager.instance.Unequip(i);
                ClearSlot();
            }
        }
        SceneController.Instance.soundController.Play("UI Click");
    } 
}
