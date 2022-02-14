using System.Collections;
using Enemies;
using Player;
using UnityEngine;
using EZCameraShake;

///// CANNOT BE ANIMATION CANCELLED & THIS ABILITY IS POSSIBLY EXTREMELY BUGGY & CODE LOOKS LIKE SHIT /////
[CreateAssetMenu(fileName = "Ambush Skill", menuName = "ScriptableObject/Skills/Ambush")]
public class AmbushSkill : SkillsObject {
    public float staminaCost;
    public float range;

    public override SkillsObject CreateInstance(float multiplier) {
        AmbushSkill instance = CreateInstance<AmbushSkill>();
        SettingBaseValues(instance, multiplier);
        instance.staminaCost = staminaCost;
        instance.range = range;
        return instance;
    }
    
    public override bool CanUseSkill(GameObject user) {
        Movement isEnemy = user.GetComponent<Movement>();
        if (isEnemy) {
            return !isActivating && isEnemy.canSeePlayer && !isEnemy.isDying && !isEnemy.alreadyAttacked && useTime + cooldown < Time.time && Vector3.Distance(isEnemy.gameObject.transform.position, SceneController.Instance.playerObject.transform.position) < isEnemy.eWeaponStats.attackKnockback;
            //isEnemy.canSeePlayer && useTime + cooldown < Time.time && distance <= maxChargeDistance && distance >= minChargeDistance
        }
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        return !isActivating && isPlayer.playerStamina > staminaCost && useTime + cooldown < Time.time && !isPlayer.isChoking;
    }

    public override void UseSkill(GameObject user) {
        base.UseSkill(user);
        PlayerMovement isPlayer = user.GetComponent<PlayerMovement>();
        if (isPlayer) {
            RaycastHit hit;
            Ray ray = isPlayer.attackCam.ScreenPointToRay(Input.mousePosition);
            Weapons myWeaponStats = isPlayer.myWeaponStats;
            if (Physics.Raycast(ray, out hit, range, isPlayer.playerLayers)) {
                if (hit.collider.tag == "Enemy") {
                    GameObject enemy = hit.collider.gameObject;
                    Vector3 playerRelativeToEnemy = enemy.transform.InverseTransformPoint(isPlayer.transform.position);
                    Movement eMovement = enemy.GetComponent<Enemies.Movement>();
                    if (playerRelativeToEnemy.z <= 0) {
                        isPlayer.isChoking = true;
                        eMovement.isChoking = true;
                        Vector3 enemyOriginalPosition = enemy.transform.position;
                        Quaternion enemyOriginalRotation = enemy.transform.rotation;
                        enemy.layer = LayerMask.NameToLayer("Ignore Raycast");
                        foreach (Transform child in enemy.transform) {
                            child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }
                        ToolMethods.ResetAllAnimatorTriggers(eMovement.animator);
                        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ignore Raycast"), LayerMask.NameToLayer("Player"), true);
                        enemy.transform.SetParent(isPlayer.firstPersonView.transform.GetChild(isPlayer.firstPersonView.transform.childCount - 1));
                        eMovement.animator.SetTrigger("isUsingSkills");
                        eMovement.animator.SetBool("isChoking", true);
                        float originalSpeed = isPlayer.speedMultiplier;
                        isPlayer.speedMultiplier = 0;
                        isPlayer.weaponAnimator.SetTrigger("isUsingSkills");
                        isPlayer.weaponAnimator.SetTrigger("isAmbushing");
                        isPlayer.StartCoroutine(PlayerChoke(isPlayer, enemy, eMovement, enemyOriginalPosition, enemyOriginalRotation, originalSpeed));
                        isPlayer.UsingStamina(staminaCost);
                    }
                }
            }
        }
        useTime = Time.time;
        isActivating = false;
    }

    public IEnumerator PlayerChoke(PlayerMovement player, GameObject enemy, Movement eMovement, Vector3 originalPosition, Quaternion originalRotation, float originalSpeed) {
        var instruction = new WaitForEndOfFrame();
        eMovement.isRotating = false;
        eMovement.rb.isKinematic = true;
        eMovement.agent.enabled = false;
        yield return instruction;
        eMovement.rb.velocity = Vector3.zero;
        eMovement.rb.angularVelocity = Vector3.zero; 
        yield return instruction;
        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localRotation = Quaternion.identity;
        enemy.transform.localScale = Vector3.one;
        enemy.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        ParticleSystem blood = Instantiate(SceneController.Instance.bloodParticles, ToolMethods.OffsetPosition(player.firstPersonView.transform.GetChild(player.firstPersonView.transform.childCount - 1).transform.position, 0, 0.65f, 0.6f), Quaternion.identity);
        blood.Play();
        SceneController.Instance.soundController.PlayClipAtPoint("Death", enemy.transform.position);
        enemy.layer = LayerMask.NameToLayer("Enemy");
        foreach (Transform child in enemy.transform) {        
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        eMovement.animator.SetBool("isChoking", false);
        eMovement.animator.ResetTrigger("isUsingSkills");
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ignore Raycast"), LayerMask.NameToLayer("Player"), false);
        enemy.transform.SetParent(null);
        enemy.GetComponent<CapsuleCollider>().enabled = true;
        eMovement.agent.enabled = true;
        enemy.transform.position = originalPosition;
        enemy.transform.rotation = originalRotation;
        enemy.transform.localScale = Vector3.one;
        eMovement.TakeDamage(player.myWeaponStats.attackDamage * player.damageMultiplier + damage);
        player.speedMultiplier = originalSpeed;
        eMovement.isChoking = false;
        player.isChoking = false;
    }
}