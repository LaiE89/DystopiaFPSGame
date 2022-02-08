using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsObject : ScriptableObject {
    public float cooldown = 10f;
    public float damage = 1;
    public bool isActivating = false;
    public int skillNumber;
    public Sprite icon;
    [TextArea(1, 1)]
    public string title;
    [TextArea(3, 3)]
    public string description;
    [HideInInspector] public float useTime = 0;

    public virtual SkillsObject CreateInstance(float multiplier) {
        SkillsObject instance = CreateInstance<SkillsObject>();
        return instance;
    }

    protected void SettingBaseValues(SkillsObject instance, float multiplier) {
        instance.name = name;
        instance.cooldown = cooldown * multiplier;
        instance.damage = damage;
        instance.icon = icon;
        instance.useTime = 0;
        instance.isActivating = false;
    }

    public virtual bool CanUseSkill(GameObject user) {
        return true;
    }

    public virtual void UseSkill(GameObject user, GameObject target) {
        isActivating = true;
    }

    public virtual void UseSkill(GameObject user) {
        isActivating = true;
    }
}
