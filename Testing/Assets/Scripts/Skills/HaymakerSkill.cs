using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Haymaker Skill", menuName = "ScriptableObject/Skills/Haymaker")]
public class HaymakerSkill : SkillsObject {
    public float knockback = 7;
    public float range = 2;

    public override SkillsObject CreateInstance(float multiplier) {
        HaymakerSkill instance = CreateInstance<HaymakerSkill>();
        SettingBaseValues(instance, multiplier);
        instance.knockback = knockback;
        instance.range = range;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isEnemy.isPassive && !isActivating && isEnemy.angleToPlayerHorz < 30 && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < range + isEnemy.height - 1.5f + 1f && !isEnemy.isChoking;
        }
        return false;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isHaymaking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void HaymakerAttack(Movement enemy) {
        SceneController.Instance.soundController.PlayClipAtPoint("Punch", enemy.transform.position, 0.25f, 1);
        enemy.isRotating = false;
        RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.GetDirection().y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z), range + enemy.height - 1.5f, enemy.enemyLayers);
        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Player") {
                    bool hasDamaged = this.CombatCalculation(enemy, SceneController.Instance.player, damage * enemy.damageMultiplier, knockback * enemy.damageMultiplier, null);
                    if (hasDamaged) {
                        SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                    }
                    SceneController.Instance.soundController.PlayClipAtPoint("Impact 2", enemy.transform.position, 0.25f, 1);
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

    public bool CombatCalculation(Movement enemy, PlayerMovement thePlayer, float damage, float knockback, AudioSource hurtSound) {
            ToolMethods.AlertRadius(enemy.alertRadius, enemy.transform.position, thePlayer.transform.position, thePlayer.enemyMask);
            if (hurtSound != null) {
                hurtSound.Play();
            }
            Vector3 direction = thePlayer.transform.position - enemy.transform.position;
            direction.y = (float)(Mathf.Sin(-enemy.transform.rotation.x * Mathf.PI/180) * knockback);
            thePlayer.rb.AddForce((direction.normalized + enemy.transform.right * 2) * knockback, ForceMode.Impulse);
            if (thePlayer.isBlocking) {
                var forward = enemy.transform.TransformDirection(Vector3.forward);
                var playerForward = thePlayer.transform.TransformDirection(Vector3.forward);
                var dotProduct = Vector3.Dot(forward, playerForward);

                if (dotProduct < -0.9) {
                    if (thePlayer.myWeaponStats.weaponHealth <= 0 && thePlayer.statusEffects.Contains("isInjured")) {
                        thePlayer.TakeDamage(damage);
                        return true;
                    }else {
                        CameraShaker.Instance.ShakeOnce(damage/2, damage, 0.1f, 0.5f);
                        thePlayer.UsingStamina(damage * 10f);
                        if (thePlayer.isParrying) {
                            SceneController.Instance.soundController.PlayOneShot("Parry");
                            thePlayer.StartParrying();
                            enemy.TakeDamage(1f);
                            if (enemy.Hand.transform.childCount > 1) {
                                enemy.eWeapon.GetComponent<Holdable>().DroppingWeapon(enemy.transform);
                                enemy.selectedWeapon = 0;
                                enemy.SwitchWeapon(enemy.selectedWeapon);
                            }
                            return false;
                        }else {
                            thePlayer.BlockingDamage(damage);
                            return false;
                        }
                    }
                }else {
                    thePlayer.TakeDamage(damage);
                    return true;
                }
            }else {
                thePlayer.TakeDamage(damage);
                return true;
            }
        }
}
