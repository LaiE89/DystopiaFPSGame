using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

///// THIS CANNOT BE ANIMATION CANCELLED /////

[CreateAssetMenu(fileName = "Slam Skill", menuName = "ScriptableObject/Skills/Slam")]
public class SlamSkill : SkillsObject {
    public ParticleSystem initialParticle;
    public float staminaCost = 10;
    public float radius = 5;
    public float horizontalForce;
    public float verticalForce;
    public float minDistance = 5;

    public override SkillsObject CreateInstance(float multiplier) {
        SlamSkill instance = CreateInstance<SlamSkill>();
        SettingBaseValues(instance, multiplier);
        instance.initialParticle = initialParticle;
        instance.staminaCost = staminaCost;
        instance.radius = radius;
        instance.horizontalForce = horizontalForce;
        instance.verticalForce = verticalForce;
        instance.minDistance = minDistance;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isActivating && isEnemy.canSeePlayer && !isEnemy.isDying && !isEnemy.alreadyAttacked && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < minDistance;
            //isEnemy.canSeePlayer && useTime + cooldown < Time.time && distance <= maxChargeDistance && distance >= minChargeDistance
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerStamina > staminaCost && useTime + cooldown < Time.time;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            if (isEnemy.agent.isActiveAndEnabled) {
                isEnemy.agent.isStopped = true;
                isEnemy.alreadyAttacked = true;
            }
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isSlamming");
        }
        useTime = Time.time;
        Debug.Log("Slam CD: " + useTime);
        isActivating = false;
    }

    public override void UseSkill(GameObject user) {
        base.UseSkill(user);
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        if (isPlayer) {
            isPlayer.UsingStamina(staminaCost);
            isPlayer.weaponAnimator.SetTrigger("isUsingSkills");
            isPlayer.weaponAnimator.SetTrigger("isSlamming");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemySlam(Movement enemy, PlayerMovement player) {
        ParticleSystem startParticle = Instantiate(initialParticle, ToolMethods.OffsetPosition(enemy.transform.position, 0, 0.3f, 0), enemy.transform.rotation);
        startParticle.Play();
        SceneController.Instance.soundController.PlayClipAtPoint("Impact", enemy.transform.position);
        Collider[] colliders = Physics.OverlapSphere(enemy.transform.position, radius);
        foreach (Collider nearbyObject in colliders) {
            Vector3 directionToTarget = (ToolMethods.OffsetPosition(nearbyObject.gameObject.transform.position, 0, 0.2f, 0) - ToolMethods.OffsetPosition(enemy.transform.position, 0, 0.2f, 0)).normalized;
            float distanceToTarget = Vector3.Distance(ToolMethods.OffsetPosition(enemy.transform.position, 0, 0.2f, 0), ToolMethods.OffsetPosition(nearbyObject.gameObject.transform.position, 0, 0.2f, 0));
            if (!Physics.Raycast(ToolMethods.OffsetPosition(enemy.transform.position, 0, 0.2f, 0), directionToTarget, distanceToTarget, enemy.groundMask)) {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null && rb != enemy.rb) {
                    if (rb == player.rb) {
                        enemy.CombatCalculation(damage * enemy.damageMultiplier, 0, SceneController.Instance.soundController.GetSound("Impact"));
                    }
                    rb.AddExplosionForce(horizontalForce, enemy.transform.position, radius, verticalForce, ForceMode.Impulse);
                }
                Destructable destructable = nearbyObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact();
                }
            }
        }
        if (enemy.agent.isActiveAndEnabled) {
            enemy.agent.isStopped = false;
        }
        enemy.alreadyAttacked = false;
        enemy.animator.ResetTrigger("isSlamming");
    }

    public void PlayerSlam(PlayerMovement player) {
        ParticleSystem startParticle = Instantiate(initialParticle, ToolMethods.OffsetPosition(player.transform.position, 0, 0.3f, 0), player.transform.rotation);
        startParticle.Play();
        CameraShaker.Instance.ShakeOnce(damage * player.damageMultiplier * 4, damage * player.damageMultiplier * 2, 0.1f, 0.5f);
        SceneController.Instance.soundController.PlayClipAtPoint("Impact", player.transform.position);
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, radius);
        foreach (Collider nearbyObject in colliders) {
            Vector3 directionToTarget = (ToolMethods.OffsetPosition(nearbyObject.gameObject.transform.position, 0, 0.2f, 0) - ToolMethods.OffsetPosition(player.transform.position, 0, 0.2f, 0)).normalized;
            float distanceToTarget = Vector3.Distance(ToolMethods.OffsetPosition(player.transform.position, 0, 0.2f, 0), ToolMethods.OffsetPosition(nearbyObject.gameObject.transform.position, 0, 0.2f, 0));
            if (!Physics.Raycast(ToolMethods.OffsetPosition(player.transform.position, 0, 0.2f, 0), directionToTarget, distanceToTarget, player.groundMask)) {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null && rb != player.rb) {
                    Enemies.Movement eMovement = nearbyObject.GetComponent<Enemies.Movement>();
                    if (eMovement != null) {
                        eMovement.TakeDamage(damage * player.damageMultiplier);
                    }
                    rb.AddExplosionForce(horizontalForce, player.transform.position, radius, verticalForce, ForceMode.Impulse);
                }
                Destructable destructable = nearbyObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact();
                }
            }
        }
    }
}
