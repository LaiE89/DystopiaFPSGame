using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image icon;
    public Image descriptionBox;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI titleText;
    SkillsObject skill;

    public void AddSkill (SkillsObject newSkill) {
        skill = newSkill;
        titleText.text = skill.title;
        descriptionText.text = skill.description;
        icon.sprite = skill.icon;
        icon.enabled = true;
    }

    public void ClearSlot() {
        skill = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!descriptionBox.gameObject.activeSelf && !ingameMenus.pausedGame) {
            descriptionBox.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (descriptionBox.gameObject.activeSelf && !ingameMenus.pausedGame) {
            descriptionBox.gameObject.SetActive(false);
        }
    }

    public void Equip() {
        if (skill != null && !ToolMethods.checkInArray(skill, EquipmentManager.instance.currentSkills)) {
            EquipmentManager.instance.Equip(skill);
        }
        SceneController.Instance.soundController.Play("UI Click");
    } 
}
