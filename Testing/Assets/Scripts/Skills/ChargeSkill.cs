using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Charge Skill", menuName = "ScriptableObject/Skills/Charge")]
public class ChargeSkill : SkillsObject {
    public float chargeSpeed = 1;
    public float staminaCost = 10;
    public float knockback = 5;
    public float minDistance = 5;

    public override SkillsObject CreateInstance(float multiplier) {
        ChargeSkill instance = CreateInstance<ChargeSkill>();
        SettingBaseValues(instance, multiplier);
        instance.chargeSpeed = chargeSpeed;
        instance.knockback = knockback;
        instance.minDistance = minDistance;
        instance.staminaCost = staminaCost;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isEnemy.isPassive && !isActivating && isEnemy.angleToPlayerHorz < 30 && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < minDistance && !isEnemy.isChoking;
            //isEnemy.canSeePlayer && useTime + cooldown < Time.time && distance <= maxChargeDistance && distance >= minChargeDistance
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerStamina > staminaCost && useTime + cooldown < Time.time && !isPlayer.isChoking;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        SceneController.Instance.groundParticlePool.SpawnDecal(user.transform.forward, user.transform.position, ToolMethods.SettingVector(1f, 1f, 1f));
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            // isEnemy.ResetSpeed();
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isCharging");
            isEnemy.StartCoroutine(EnemyCharge(isEnemy, target.GetComponent<PlayerMovement>()));
        }
        useTime = Time.time;
        isActivating = false;
    }

    public override void UseSkill(GameObject user) {
        base.UseSkill(user);
        SceneController.Instance.groundParticlePool.SpawnDecal(user.transform.forward, user.transform.position, ToolMethods.SettingVector(1f, 1f, 1f));
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        if (isPlayer) {
            isPlayer.UsingStamina(staminaCost);
            isPlayer.weaponAnimator.SetTrigger("isUsingSkills");
            isPlayer.weaponAnimator.SetTrigger("isCharging");
            isPlayer.StartCoroutine(PlayerCharge(isPlayer));
        }
        useTime = Time.time;
        isActivating = false;
    }

    private IEnumerator EnemyCharge(Movement enemy, PlayerMovement player) {
        bool isCharging = true;
        enemy.turnNonKinematic();
        SceneController.Instance.soundController.PlayClipAtPoint("Dash", enemy.transform.position, 1, 0.5f);
        Vector3 eDirection = player.transform.position - enemy.transform.position;
        enemy.rb.AddForce(ToolMethods.SettingVector(eDirection.x, 0, eDirection.z).normalized * chargeSpeed, ForceMode.VelocityChange);
        while (!enemy.rb.isKinematic && isCharging) {
            if (Vector3.Distance(enemy.transform.position, player.transform.position) < 1f) {
                enemy.turnNonKinematic();
                enemy.CombatCalculation(damage * enemy.damageMultiplier, knockback, SceneController.Instance.soundController.GetSound("Impact"));
                enemy.rb.AddForce(-enemy.rb.velocity.normalized * knockback, ForceMode.Impulse);
                isCharging = false;
            }
            yield return null;
        }
        enemy.animator.ResetTrigger("isUsingSkills");
        enemy.animator.ResetTrigger("isCharging");
        isCharging = true;
    }

    private IEnumerator PlayerCharge(PlayerMovement player) {
        float chargingTime = 0;
        bool isCharging = true;
        SceneController.Instance.soundController.PlayClipAtPoint("Dash", player.transform.position, 1, 0.5f);
        player.rb.AddForce(ToolMethods.SettingVector(player.transform.forward.x, 0, player.transform.forward.z).normalized * chargeSpeed, ForceMode.Impulse);
        while (isCharging) {
            chargingTime += Time.deltaTime; 
            foreach (Movement enemy in SceneController.Instance.listOfEnemies) {
                if (enemy != null && !enemy.isChoking && Vector3.Distance(enemy.transform.position, player.transform.position) < 1f) {
                    SceneController.Instance.soundController.PlayClipAtPoint("Impact", player.transform.position, 1, 1);
                    player.DealDamage(enemy, enemy.gameObject, damage * player.damageMultiplier, 0);
                    CameraShaker.Instance.ShakeOnce(damage * player.damageMultiplier * 8, damage * player.damageMultiplier * 4, 0.1f, 0.5f);
                    enemy.rb.AddForce(player.rb.velocity.normalized * knockback, ForceMode.Impulse);
                    player.rb.AddForce(-player.rb.velocity.normalized * knockback, ForceMode.Impulse);
                    isCharging = false;
                }
            }
            if (chargingTime > 0.4f && player.isGrounded) {
                isCharging = false;
            }
            yield return null;
        }
        //player.weaponAnimator.ResetTrigger("isUsingSkills");
        //player.weaponAnimator.ResetTrigger("isCharging");
        isCharging = true;
    }
}
