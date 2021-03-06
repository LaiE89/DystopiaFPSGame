using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Right Hook Skill", menuName = "ScriptableObject/Skills/RightHook")]
public class RightHookSkill : SkillsObject {
    public float knockback = 3;
    public float range = 2;

    public override SkillsObject CreateInstance(float multiplier) {
        RightHookSkill instance = CreateInstance<RightHookSkill>();
        SettingBaseValues(instance, multiplier);
        instance.knockback = knockback;
        instance.range = range;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isEnemy.isPassive && !isActivating && !isEnemy.alreadyAttacked && isEnemy.canSeePlayer && isEnemy.angleToPlayerHorz < 30 && !isEnemy.isDying && useTime + cooldown < Time.time && !isEnemy.isChoking && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < range + isEnemy.height - 1.5f + 1;
            //isEnemy.canSeePlayer && useTime + cooldown < Time.time && distance <= maxChargeDistance && distance >= minChargeDistance
        }
        return !isActivating && useTime + cooldown < Time.time;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            if (isEnemy.skillLagRoutine != null) {
                isEnemy.StopCoroutine(isEnemy.skillLagRoutine);
            }
            isEnemy.skillLagRoutine = isEnemy.StartCoroutine(isEnemy.SkillEndingLag(1f, 0.5f));
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isRightHooking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyRightHook(Movement enemy, PlayerMovement player) {
        enemy.eWeaponStats.attackSound.Play();
        enemy.isRotating = false;
        RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(enemy.transform.position, 0, enemy.height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(enemy.transform.TransformDirection(Vector3.forward).x, enemy.GetDirection().y, enemy.transform.TransformDirection(Vector3.forward).z), range + enemy.height - 1.5f, enemy.enemyLayers);
        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Player") {
                    bool hasDamaged = CombatCalculation(enemy, player, enemy.eWeaponStats.attackDamage * enemy.damageMultiplier, knockback, enemy.eWeaponStats.hurtSound);
                    if (hasDamaged) {
                        SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                    }
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                    destructable.Interact(); 
                    }
                    SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                }
            }
        }
    }

    private bool CombatCalculation(Movement enemy, PlayerMovement player, float damage, float knockback, AudioSource hurtSound) {
        GameObject thePlayer = SceneController.Instance.playerObject;
        ToolMethods.AlertRadius(enemy.alertRadius, enemy.transform.position, thePlayer.transform.position, player.enemyMask);
        if (hurtSound != null) {
            hurtSound.Play();
        }
        Vector3 direction = thePlayer.transform.position - enemy.transform.position;
        direction.y = (float)(Mathf.Sin(-enemy.transform.rotation.x * Mathf.PI/180) * knockback);
        player.rb.AddForce(direction.normalized * knockback, ForceMode.Impulse);
        if (player.isBlocking) {
            var forward = enemy.transform.TransformDirection(Vector3.forward);
            var playerForward = thePlayer.transform.TransformDirection(Vector3.forward);
            var dotProduct = Vector3.Dot(forward, playerForward);

            if (dotProduct < -0.9) {
                if (player.myWeaponStats.weaponHealth <= 0 && player.statusEffects.Contains("isInjured")) {
                    player.TakeDamage(damage);
                    return true;
                }else {
                    CameraShaker.Instance.ShakeOnce(damage, damage * 2, 0.1f, 0.5f);
                    player.UsingStamina(damage * 10f);
                    // if (player.hand.transform.childCount > 1) {
                    if (!player.myWeaponStats.isDefaultItem) {
                        player.myWeapon.GetComponent<Holdable>().DroppingWeapon(thePlayer.transform);
                        player.selectedWeapon = 1;
                        player.SwitchWeapon(player.selectedWeapon);
                    }else {
                        if (!player.statusEffects.Contains("isInjured")) {
                            player.BlockingDamage(damage);
                        }
                    }
                    return false;
                }
            }
            return true;
        }else {
            player.TakeDamage(damage);
            return true;
        }
    }
}
