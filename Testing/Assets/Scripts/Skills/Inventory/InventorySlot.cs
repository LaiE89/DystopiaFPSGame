using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    public Image icon;
    SkillsObject skill;

    public void AddSkill (SkillsObject newSkill) {
        skill = newSkill;
        icon.sprite = skill.icon;
        icon.enabled = true;
    }

    public void ClearSlot() {
        skill = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void Equip() {
        if (skill != null && !ToolMethods.checkInArray(skill, EquipmentManager.instance.currentSkills)) {
            EquipmentManager.instance.Equip(skill);
        }
        SceneController.Instance.soundController.Play("UI Click");
    } 
}
