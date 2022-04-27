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
        SceneController.Instance.soundController.PlayClipAtPoint("Punch", enemy.transform.position, 0.3f, 1);
        enemy.isRotating = false;
        RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.GetDirection().y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z), range + enemy.height - 1.5f, enemy.enemyLayers);
        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Player") {
                    enemy.CombatCalculation(damage * enemy.damageMultiplier, knockback * enemy.damageMultiplier, null);
                    SceneController.Instance.soundController.PlayClipAtPoint("Impact 2", enemy.transform.position, 0.3f, 1);
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

    public void EnemyJump(Movement enemy, PlayerMovement player) {
        enemy.turnNonKinematic();
        SceneController.Instance.soundController.PlayClipAtPoint("PlayerJump", enemy.transform.position, 0.5f, 1f);
        Vector3 eDirection = player.transform.position - enemy.transform.position;
        // distanceToTarget = Vector3.Distance(enemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position);
        enemy.rb.AddForce(ToolMethods.SettingVector(eDirection.x, distanceToTarget - 0.5f + enemy.height - 1.5f, eDirection.z).normalized * (distanceToTarget + 2.5f), ForceMode.VelocityChange);
    }
}

