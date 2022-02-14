using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Right Hook Skill", menuName = "ScriptableObject/Skills/RightHook")]
public class RightHookSkill : SkillsObject {
    public float knockback = 3;

    public override SkillsObject CreateInstance(float multiplier) {
        RightHookSkill instance = CreateInstance<RightHookSkill>();
        SettingBaseValues(instance, multiplier);
        instance.knockback = knockback;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isActivating && !isEnemy.alreadyAttacked && isEnemy.canSeePlayer && isEnemy.angleToPlayerHorz < 30 && !isEnemy.isDying && useTime + cooldown < Time.time && !isEnemy.isChoking;
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
            isEnemy.skillLagRoutine = isEnemy.StartCoroutine(isEnemy.SkillEndingLag(1.25f, 0.25f));
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isRightHooking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyRightHook(Movement enemy, PlayerMovement player) {
        enemy.eWeaponStats.attackSound.Play();
        enemy.isRotating = false;
        RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(enemy.transform.position, 0, enemy.height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(enemy.transform.TransformDirection(Vector3.forward).x, enemy.directionToTarget.y, enemy.transform.TransformDirection(Vector3.forward).z), enemy.eWeaponStats.attackRange, enemy.enemyLayers);
        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Player") {
                    CombatCalculation(enemy, player, enemy.eWeaponStats.attackDamage * enemy.damageMultiplier, knockback, enemy.eWeaponStats.hurtSound);
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                    destructable.Interact(); 
                    }
                    ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    ground.Play();
                }
            }
        }
    }

    private void CombatCalculation(Movement enemy, PlayerMovement player, float damage, float knockback, AudioSource hurtSound) {
        GameObject thePlayer = SceneController.Instance.playerObject;
        ToolMethods.AlertRadius(enemy.alertRadius, thePlayer.transform.position, player.enemyMask);
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
                }else {
                    CameraShaker.Instance.ShakeOnce(damage, damage * 2, 0.1f, 0.5f);
                    player.UsingStamina(damage * 10f);
                    if (player.hand.transform.childCount > 1) {
                        player.myWeapon.GetComponent<Holdable>().DroppingWeapon(thePlayer.transform);
                        player.selectedWeapon = 1;
                        player.SwitchWeapon(player.selectedWeapon);
                    }
                    player.BlockingDamage(damage);
                }
            }
        }else {
            player.TakeDamage(damage);
        }
    }
}
