using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsObject : ScriptableObject {
    public float cooldown = 10f;
    public float damage = 1;

    public bool isActivating = false;

    public int skillNumber;
    [HideInInspector] public float useTime = 0;

    public virtual SkillsObject CreateInstance(float multiplier) {
        SkillsObject instance = CreateInstance<SkillsObject>();
        return instance;
    }

    protected void SettingBaseValues(SkillsObject instance, float multiplier) {
        instance.name = name;
        instance.cooldown = cooldown * multiplier;
        instance.damage = damage;
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
