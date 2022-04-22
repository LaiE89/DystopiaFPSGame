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
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animator.SetTrigger("isKicking");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyKick(Movement enemy) {
        SceneController.Instance.soundController.PlayClipAtPoint("Punch", enemy.transform.position, 0.25f, 1);
        enemy.isRotating = false;
        RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.directionToTarget.y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z), enemy.height + 1f, enemy.enemyLayers);
        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Player") {
                    enemy.CombatCalculation(damage * enemy.damageMultiplier, knockback * enemy.damageMultiplier, null);
                    SceneController.Instance.soundController.PlayClipAtPoint("Impact 2", enemy.transform.position, 0.25f, 1);
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
}
