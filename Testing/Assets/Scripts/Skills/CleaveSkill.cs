using System.Collections;
using Enemies;
using Player;
using UnityEngine;

[CreateAssetMenu(fileName = "Cleave Skill", menuName = "ScriptableObject/Skills/Cleave")]
public class CleaveSkill : SkillsObject {
    public float addedStaminaCost = 10;

    public override SkillsObject CreateInstance(float multiplier) {
        CleaveSkill instance = CreateInstance<CleaveSkill>();
        SettingBaseValues(instance, multiplier);
        instance.addedStaminaCost = addedStaminaCost;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isEnemy.isPassive && !isActivating && isEnemy.angleToPlayerHorz < 30 && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < isEnemy.eWeaponStats.attackRange + 1 && !isEnemy.isChoking;
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerStamina > isPlayer.myWeaponStats.staminaCost + addedStaminaCost && useTime + cooldown < Time.time && !isPlayer.isChoking;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            // isEnemy.ResetSpeed();
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isCleaving");
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
            isPlayer.weaponAnimator.SetTrigger("isCleaving");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyAttack(Movement enemy) {
        if (enemy.eWeaponStats.isGun) {
            enemy.eWeaponStats.MeleeAttack(enemy.eWeaponStats.attackRange + enemy.height - 1.5f, enemy.eWeaponStats.attackDamage, enemy.eWeaponStats.attackKnockback, enemy.eWeaponStats.attackSound, enemy.eWeaponStats.hurtSound, enemy);
        }else {
            enemy.eWeaponStats.SpherecastDamage(enemy.eWeaponStats.attackRange + enemy.height - 1.5f, enemy.eWeaponStats.attackDamage * enemy.damageMultiplier, enemy.eWeaponStats.attackKnockback, enemy.eWeaponStats.attackSound, enemy.eWeaponStats.hurtSound, enemy);
        }
    }

    public void PlayerAttack(PlayerMovement player) {
        Vector3 mousePos = player.attackCam.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = player.attackCam.ScreenPointToRay(Input.mousePosition);
        Collider[] rangeChecks = Physics.OverlapSphere(mousePos, player.myWeaponStats.attackRange, player.playerLayers);
        Debug.Log("Mouse position: " + mousePos);
        PrintChecks(rangeChecks);
        if (rangeChecks.Length != 0) {
            foreach (Collider collider in rangeChecks) {
                Transform enemyTarget = collider.transform;
                Vector3 directionToTarget = (enemyTarget.position - mousePos).normalized;
                float angleToPlayer = Vector3.Angle(ray.direction.normalized, ToolMethods.SettingVector(directionToTarget.x, ray.direction.y, directionToTarget.z));
                float distanceToTarget = Vector3.Distance(mousePos, enemyTarget.position);
                // Debug.Log("Target Name: " + enemyTarget.gameObject.name + ", AngleToPlayer: " + angleToPlayer + ", DistanceToTarget: " + distanceToTarget);
                if (angleToPlayer < 180 / 2) {
                    if (Physics.Raycast(mousePos, ToolMethods.SettingVector(directionToTarget.x, ray.direction.y, directionToTarget.z), out RaycastHit hit, distanceToTarget, player.playerLayers)){
                        player.myWeaponStats.PlayerDealDamage(player.myWeaponStats.attackDamage, player.myWeaponStats.attackKnockback, player.myWeaponStats.attackSound, player.myWeaponStats.hurtSound, hit, player);
                    }
                }
            }
        }
    }

    private void PrintChecks(Collider[] rangeChecks) {
        string result = "";
        foreach (Collider collider in rangeChecks) {
            result += collider.gameObject.name + ", ";
        }
        Debug.Log("Checks: " + result);
    }
}
