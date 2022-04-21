using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Kick Skill", menuName = "ScriptableObject/Skills/Kick")]
public class KickSkill : SkillsObject {
    public float knockback = 10;

    public override SkillsObject CreateInstance(float multiplier) {
        KickSkill instance = CreateInstance<KickSkill>();
        SettingBaseValues(instance, multiplier);
        instance.knockback = knockback;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isEnemy.isPassive && !isActivating && isEnemy.angleToPlayerHorz < 30 && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < isEnemy.height + 0.75f && !isEnemy.isChoking;
        }
        return false;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            if (isEnemy.skillLagRoutine != null) {
                isEnemy.StopCoroutine(isEnemy.skillLagRoutine);
            }
            isEnemy.ResetSpeed();
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isKicking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyKick(Movement enemy) {
        enemy.eWeaponStats.SpherecastDamage(enemy.height + 1f, damage * enemy.damageMultiplier, knockback * enemy.damageMultiplier, SceneController.Instance.soundController.GetSound("Dash"), SceneController.Instance.soundController.GetSound("Impact"), enemy);
    }
}
