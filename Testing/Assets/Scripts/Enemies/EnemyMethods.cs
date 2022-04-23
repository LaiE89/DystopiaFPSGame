using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

namespace Enemies {
    public class EnemyMethods : MonoBehaviour {
        [SerializeField] Movement enemyMovement;
        [SerializeField] Food food;

        public void walkFootStep() {
            enemyMovement.walkSound.Play();
        }

        public void runFootStep() {
            enemyMovement.runSound.Play();
        }

        public void PlayerDamage() {
            // enemyMovement.SpherecastDamage(enemyMovement.eWeaponStats.attackRange, enemyMovement.eWeaponStats.attackDamage * enemyMovement.damageMultiplier, enemyMovement.eWeaponStats.attackKnockback, enemyMovement.eWeaponStats.attackSound, enemyMovement.eWeaponStats.hurtSound);
            enemyMovement.eWeaponStats.SpherecastDamage(enemyMovement.eWeaponStats.attackRange, enemyMovement.eWeaponStats.attackDamage * enemyMovement.damageMultiplier, enemyMovement.eWeaponStats.attackKnockback, enemyMovement.eWeaponStats.attackSound, enemyMovement.eWeaponStats.hurtSound, enemyMovement);
        }

        public void StartRotation() {
            enemyMovement.isRotating = true;
        }

        public void DestroyEnemy() {
            //Destroy(gameObject, 30);
            gameObject.tag = "Food";
            food.enabled = true;
            CapsuleCollider eCollider = gameObject.GetComponent<CapsuleCollider>() as CapsuleCollider;
            eCollider.center = ToolMethods.SettingVector(0, 0.35f, 0); 
            eCollider.height = 0.3f;
            gameObject.layer = LayerMask.NameToLayer("Item");;
            enemyMovement.enabled = false;
            enemyMovement.agent.enabled = false;
        }

        public void ExtraWeaponSound() {
            enemyMovement.eWeaponStats.extraSound.Play();
        }

        public void ShootDamage() {
            enemyMovement.eWeaponStats.muzzleFlash.Play();
            enemyMovement.eWeaponStats.RaycastDamage(enemyMovement.eWeaponStats.shootRange, enemyMovement.eWeaponStats.shootDamage * enemyMovement.damageMultiplier, enemyMovement.eWeaponStats.shootKnockback, enemyMovement.eWeaponStats.shootSound, enemyMovement.eWeaponStats.shootHurtSound, enemyMovement);
            if (enemyMovement.eWeaponStats.bullets > 0) {
                enemyMovement.eWeaponStats.bullets -= 1;
                StartCoroutine(waitForBulletCheck());
            }
        }
        
        IEnumerator waitForBulletCheck() {
            enemyMovement.isMidAttack = true;
            yield return new WaitForEndOfFrame();
            if (enemyMovement.eWeaponStats.bullets <= 0) {
                AnimatorStateInfo stateInfo = enemyMovement.animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log(stateInfo.IsName("Attack"));
                yield return new WaitForSeconds((1 - stateInfo.normalizedTime) * stateInfo.length);
                enemyMovement.isMidAttack = false;
                enemyMovement.animatorOverrideController["Punch"] = enemyMovement.eWeaponStats.attackAnimation;
            }else {
                enemyMovement.isMidAttack = false;
            }
        }

        public void ShootDamageEnd() {
            /*if (enemyMovement.eWeaponStats.bullets <= 0) {
                enemyMovement.animatorOverrideController["Punch"] = enemyMovement.eWeaponStats.attackAnimation;
            }*/
        }

        public void RightHook() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(RightHookSkill)) {
                    RightHookSkill hook = skills as RightHookSkill;
                    hook.EnemyRightHook(enemyMovement, SceneController.Instance.player);
                }
            }
        }

        public void Slam() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(SlamSkill)) {
                    SlamSkill slam = skills as SlamSkill;
                    slam.EnemySlam(enemyMovement, SceneController.Instance.player);
                }
            }
        }

        public void Molotov() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(MolotovSkill)) {
                    MolotovSkill molotov = skills as MolotovSkill;
                    molotov.EnemyMolotov(enemyMovement, SceneController.Instance.player);
                }
            }
        }

        public void Shit() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(ShitSkill)) {
                    ShitSkill shit = skills as ShitSkill;
                    shit.EnemyShit(enemyMovement, SceneController.Instance.player);
                }
            }
        }

        public void Kick() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(KickSkill)) {
                    KickSkill kick = skills as KickSkill;
                    kick.EnemyKick(enemyMovement);
                }
            }
        }

        public void JumpAttack() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(JumpAttackSkill)) {
                    JumpAttackSkill jump = skills as JumpAttackSkill;
                    jump.EnemyJumpAttack(enemyMovement);
                }
            }
        }

        public void Jump() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(JumpAttackSkill)) {
                    JumpAttackSkill jump = skills as JumpAttackSkill;
                    jump.EnemyJump(enemyMovement, SceneController.Instance.player);
                }
            }
        }

        public void QuickAttack() {
            foreach (SkillsObject skills in enemyMovement.skills) {
                if (skills.GetType() == typeof(QuickAttackSkill)) {
                    QuickAttackSkill qAttack = skills as QuickAttackSkill;
                    qAttack.EnemyAttack(enemyMovement);
                }
            }
        }
    }
}