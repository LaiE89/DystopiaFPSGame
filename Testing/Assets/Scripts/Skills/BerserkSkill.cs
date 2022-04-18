using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(fileName = "Berserk Skill", menuName = "ScriptableObject/Skills/Berserk")]
public class BerserkSkill : SkillsObject {
    public ParticleSystem initialParticle;
    public float damageMultiplier = 2;
    public float staminaUsageMultiplier = 2;
    public float attackSpeedMultiplier = 2;
    public float speedMultiplier = 2;
    public float lastingTime = 10;
    public float hungerCost = 50;

    public override SkillsObject CreateInstance(float multiplier) {
        BerserkSkill instance = CreateInstance<BerserkSkill>();
        SettingBaseValues(instance, multiplier);
        instance.initialParticle = initialParticle;
        instance.damageMultiplier = damageMultiplier;
        instance.staminaUsageMultiplier = staminaUsageMultiplier;
        instance.attackSpeedMultiplier = attackSpeedMultiplier;
        instance.speedMultiplier = speedMultiplier;
        instance.lastingTime = lastingTime;
        instance.hungerCost = hungerCost;
        return instance;
    }

    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isEnemy.isPassive && !isActivating && isEnemy.canSeePlayer && !isEnemy.isDying && useTime + cooldown < Time.time;
            //isEnemy.canSeePlayer && useTime + cooldown < Time.time && distance <= maxChargeDistance && distance >= minChargeDistance
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerHunger >= hungerCost && useTime + cooldown < Time.time;
    }

    public override void UseSkill(GameObject user) {
        base.UseSkill(user);
        ParticleSystem startParticle = Instantiate(initialParticle, user.transform.position, user.transform.rotation);
        startParticle.Play();
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        if (isPlayer) {
            isPlayer.playerHunger -= hungerCost;
            isPlayer.hungerSlider.value = isPlayer.playerHunger;
            //isPlayer.weaponAnimator.SetTrigger("isUsingSkills");
            //isPlayer.weaponAnimator.SetTrigger("isCharging");
            isPlayer.StartCoroutine(playerBerserk(isPlayer));
        }
        useTime = Time.time;
        isActivating = false;
    }

    public override void UseSkill(GameObject user, GameObject target) {
        base.UseSkill(user);
        ParticleSystem startParticle = Instantiate(initialParticle, user.transform.position, user.transform.rotation);
        startParticle.Play();
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            isEnemy.StartCoroutine(enemyBerserk(isEnemy));
        }
        useTime = Time.time;
        isActivating = false;
    }

    private IEnumerator playerBerserk(PlayerMovement player) {
        SceneController.Instance.soundController.PlayClipAtPoint("Berserk", player.transform.position, 0.5f, 1);
        player.damageMultiplier *= damageMultiplier;
        player.attackSpeedMultiplier /= attackSpeedMultiplier;  
        player.staminaUsageMultiplier /= staminaUsageMultiplier;
        player.speedMultiplier *= speedMultiplier;
        player.berserkScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(lastingTime);
        player.UpdatingStatus(player.statusEffects);
        player.berserkScreen.gameObject.SetActive(false);
    }

    private IEnumerator enemyBerserk(Movement enemy) {
        SceneController.Instance.soundController.PlayClipAtPoint("Berserk", enemy.transform.position, 0.5f, 1);
        enemy.damageMultiplier *= damageMultiplier;
        enemy.attackSpeedMultiplier /= attackSpeedMultiplier;  
        enemy.speedMultiplier *= speedMultiplier;
        yield return new WaitForSeconds(lastingTime);
        enemy.UpdatingStatus();
    }
}
