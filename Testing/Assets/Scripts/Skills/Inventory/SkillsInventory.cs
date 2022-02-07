using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skills Inventory", menuName = "ScriptableObject/Inventory/Skills Inventory")]
public class SkillsInventory : ScriptableObject {
    public List<SkillsObject> Container = new List<SkillsObject>();

    public void AddSkill(SkillsObject skill) {
        Container.Add(skill);
    }
}