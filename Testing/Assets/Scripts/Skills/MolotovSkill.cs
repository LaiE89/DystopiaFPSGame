using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

///// THIS CANNOT BE ANIMATION CANCELLED /////

[CreateAssetMenu(fileName = "Molotov Skill", menuName = "ScriptableObject/Skills/Molotov")]
public class MolotovSkill : SkillsObject {
    public GameObject molotovPrefab;
    public AnimationClip molotovThrowing;
    public AnimationClip enemyMolotovThrowing;
    public float animationSpeed = 1f;
    public float force = 10f;
    public float verticalForce = 5f;
    public float staminaCost = 50;
    public float minDistance = 8;
    public float maxDistance = 20;

    public override SkillsObject CreateInstance(float multiplier) {
        MolotovSkill instance = CreateInstance<MolotovSkill>();
        SettingBaseValues(instance, multiplier);
        instance.force = force;
        instance.animationSpeed = animationSpeed;
        instance.molotovThrowing = molotovThrowing;
        instance.enemyMolotovThrowing = enemyMolotovThrowing;
        instance.verticalForce = verticalForce;
        instance.minDistance = minDistance;
        instance.maxDistance = maxDistance;
        instance.staminaCost = staminaCost;
        instance.molotovPrefab = molotovPrefab;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            float distance = Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position);
            return !isEnemy.isPassive && !isActivating && isEnemy.canSeePlayer && !isEnemy.isDying && !isEnemy.alreadyAttacked && useTime + cooldown < Time.time && distance > minDistance && distance <= maxDistance && !isEnemy.isChoking;
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerStamina > staminaCost && useTime + cooldown < Time.time && !isPlayer.isChoking;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user, target);
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            if (isEnemy.skillLagRoutine != null) {
                isEnemy.StopCoroutine(isEnemy.skillLagRoutine);
            }
            isEnemy.skillLagRoutine = isEnemy.StartCoroutine(isEnemy.SkillEndingLag(1, 0f));
            isEnemy.animator.SetTrigger("isUsingSkills");
            isEnemy.animatorOverrideController["ThrowMolotov"] = enemyMolotovThrowing;
            isEnemy.animator.SetTrigger("isThrowing");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public override void UseSkill(GameObject user) {
        base.UseSkill(user);
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        if (isPlayer) {
            isPlayer.UsingStamina(staminaCost);
            isPlayer.weaponAnimator.SetTrigger("isUsingSkills");
            isPlayer.weaponAnimator.SetFloat("ThrowMultiplier", animationSpeed);
            isPlayer.weaponOverrideController["ThrowMolotov"] = molotovThrowing;
            isPlayer.weaponAnimator.SetTrigger("isThrowing");
        }
        useTime = Time.time;
        isActivating = false;
    }

    public void EnemyMolotov(Movement enemy, PlayerMovement player) {
        GameObject molotov = Instantiate(molotovPrefab, ToolMethods.OffsetPosition(enemy.transform.position, 0, enemy.height - 0.25f, 0f) + enemy.transform.forward, enemy.transform.rotation);
        Rigidbody rb = molotov.GetComponent<Rigidbody>();
        rb.AddForce(ToolMethods.SettingVector(enemy.transform.forward.x * force, enemy.transform.forward.y + verticalForce, enemy.transform.forward.z * force), ForceMode.Impulse);
        float random = Random.Range(-1f,1f);
        rb.AddTorque(ToolMethods.SettingVector(random, random, random) * 10);
        enemy.animator.ResetTrigger("isUsingSkills");
        enemy.animator.ResetTrigger("isThrowing");
    }

    public void PlayerMolotov(PlayerMovement player) {
        GameObject molotov = Instantiate(molotovPrefab, ToolMethods.OffsetPosition(player.transform.position, 0, 1.25f, 0f) + player.transform.forward, player.transform.rotation);
        Rigidbody rb = molotov.GetComponent<Rigidbody>();
        rb.AddForce(ToolMethods.SettingVector(player.attackCam.transform.forward.x * force, player.attackCam.transform.forward.y * force, player.attackCam.transform.forward.z * force), ForceMode.Impulse);
        float random = Random.Range(-1f,1f);
        rb.AddTorque(ToolMethods.SettingVector(random, random, random) * 10); 
    }
}
