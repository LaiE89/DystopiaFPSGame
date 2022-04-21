using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Jump Attack Skill", menuName = "ScriptableObject/Skills/JumpAttack")]
public class JumpAttackSkill : SkillsObject {
    public float knockback = 3;
    public float maxDistance = 6;
    public float minDistance = 2;
    public float range = 2;
    float distanceToTarget;

    public override SkillsObject CreateInstance(float multiplier) {
        JumpAttackSkill instance = CreateInstance<JumpAttackSkill>();
        SettingBaseValues(instance, multiplier);
        instance.knockback = knockback;
        instance.maxDistance = maxDistance;
        instance.minDistance = minDistance;
        instance.range = range;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            distanceToTarget = Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position);
            return !isEnemy.isPassive && !isActivating && isEnemy.angleToPlayerHorz < 30 && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time && distanceToTarget >= minDistance && distanceToTarget <= maxDistance && !isEnemy.isChoking;
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
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isJumpAttacking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyJumpAttack(Movement enemy) {
        enemy.eWeaponStats.SpherecastDamage(range, damage * enemy.damageMultiplier, knockback * enemy.damageMultiplier, SceneController.Instance.soundController.GetSound("Dash"), SceneController.Instance.soundController.GetSound("Impact"), enemy);
    }

    public void EnemyJump(Movement enemy, PlayerMovement player) {
        enemy.turnNonKinematic();
        SceneController.Instance.soundController.PlayClipAtPoint("Dash", enemy.transform.position, 1, 0.5f);
        Vector3 eDirection = player.transform.position - enemy.transform.position;
        // distanceToTarget = Vector3.Distance(enemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position);
        enemy.rb.AddForce(ToolMethods.SettingVector(eDirection.x, distanceToTarget - 0.5f, eDirection.z).normalized * (distanceToTarget + 2f), ForceMode.Impulse);
    }
}

