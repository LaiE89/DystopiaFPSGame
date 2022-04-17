using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Quick Attack Skill", menuName = "ScriptableObject/Skills/QuickAttack")]
public class QuickAttackSkill : SkillsObject {
    public float addedStaminaCost = 5;

    public override SkillsObject CreateInstance(float multiplier) {
        QuickAttackSkill instance = CreateInstance<QuickAttackSkill>();
        SettingBaseValues(instance, multiplier);
        instance.addedStaminaCost = addedStaminaCost;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isActivating && isEnemy.angleToPlayerHorz < 30 && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < isEnemy.eWeaponStats.attackRange + 1 && !isEnemy.isChoking;
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerStamina > isPlayer.myWeaponStats.staminaCost + addedStaminaCost && useTime + cooldown < Time.time && !isPlayer.isChoking;
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
            isEnemy.animator.SetTrigger("isQuickAttacking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public override void UseSkill(GameObject user) {
        base.UseSkill(user);
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        if (isPlayer) {
            isPlayer.UsingStamina(isPlayer.myWeaponStats.staminaCost + addedStaminaCost);
            isPlayer.weaponAnimator.SetTrigger("isUsingSkills");
            isPlayer.weaponAnimator.SetTrigger("isQuickAttacking");
        }
        useTime = Time.time;
        isActivating = false;
    }
}
