using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

namespace Player {
    public class FirstPersonMethods : MonoBehaviour {
        [SerializeField] PlayerMovement PlayerMovement;

        public void StopParrying() {
            PlayerMovement.StopParrying();
        }

        public void Consuming() {
            PlayerMovement.soundController.PlayOneShot("Eating");
            if (PlayerMovement.isConsuming) {
                PlayerMovement.Consuming();
            }
        }

        public void Reloading() {
            // PlayerMovement.soundController.PlayOneShot("Eating");
            PlayerMovement.myWeaponStats.reloadSound.Play();
            if (PlayerMovement.isReloading) {
                PlayerMovement.Reloading();
            }
        }

        public void AttackDamage() {
            ToolMethods.AlertRadius(PlayerMovement.myWeaponStats.alertRadius, transform.position, transform.position, PlayerMovement.enemyMask);
            // PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.attackRange, PlayerMovement.myWeaponStats.attackDamage * PlayerMovement.damageMultiplier, PlayerMovement.myWeaponStats.attackKnockback, PlayerMovement.myWeaponStats.attackSound, PlayerMovement.myWeaponStats.hurtSound); 
            PlayerMovement.myWeaponStats.AttackDamage(PlayerMovement.myWeaponStats.attackRange, PlayerMovement.myWeaponStats.attackDamage * PlayerMovement.damageMultiplier, PlayerMovement.myWeaponStats.attackKnockback, PlayerMovement.myWeaponStats.attackSound, PlayerMovement.myWeaponStats.hurtSound, PlayerMovement);   
        }

        public void ShootDamage() {
            ToolMethods.AlertRadius(PlayerMovement.myWeaponStats.shootAlertRadius, transform.position, transform.position, PlayerMovement.enemyMask);
            PlayerMovement.myWeaponStats.muzzleFlash.Play();
            PlayerMovement.myWeaponStats.AttackDamage(PlayerMovement.myWeaponStats.shootRange, PlayerMovement.myWeaponStats.shootDamage, PlayerMovement.myWeaponStats.shootKnockback, PlayerMovement.myWeaponStats.shootSound, PlayerMovement.myWeaponStats.shootHurtSound, PlayerMovement);
            if (PlayerMovement.myWeaponStats.bullets > 0) {
                StopCoroutine(waitForBulletCheck());
                StartCoroutine(waitForBulletCheck());
                PlayerMovement.myWeaponStats.bullets -= 1;
            }
            PlayerMovement.bulletsTextBox.text = ("BULLETS x " + PlayerMovement.myWeaponStats.bullets);
        }

        IEnumerator waitForBulletCheck() {
            PlayerMovement.isMidAttack = true;
            yield return new WaitForEndOfFrame();
            if (PlayerMovement.myWeaponStats.bullets <= 0) {
                AnimatorStateInfo stateInfo = PlayerMovement.weaponAnimator.GetNextAnimatorStateInfo(0);
                AnimatorStateInfo stateInfo2 = PlayerMovement.weaponAnimator.GetCurrentAnimatorStateInfo(0);
                Debug.Log("State Info Name: " + stateInfo.IsName("Attack") + ", Length: " + stateInfo.length + ", NormalizedTime: " + stateInfo.normalizedTime + ", State Info2 Name: " + stateInfo2.IsName("Attack") + ", Length: " + stateInfo2.length + ", NormalizedTime: " + stateInfo2.normalizedTime);
                if (stateInfo2.IsName("Attack")) {
                    yield return new WaitForSeconds((1 - stateInfo2.normalizedTime) * stateInfo2.length);    
                }else {
                    yield return new WaitForSeconds((1 - stateInfo.normalizedTime) * stateInfo.length);
                }
                PlayerMovement.isMidAttack = false;
                PlayerMovement.weaponOverrideController["Attack"] = PlayerMovement.myWeaponStats.fpAttackAnimation;
            }else {
                PlayerMovement.isMidAttack = false;
            }
        }
        
        public void ExtraWeaponSound() {
            PlayerMovement.myWeaponStats.extraSound.Play();
        }

        public void Slam() {
            foreach (SkillsObject skills in PlayerMovement.skills) {
                if (skills.GetType() == typeof(SlamSkill)) {
                    SlamSkill slam = skills as SlamSkill;
                    slam.PlayerSlam(PlayerMovement);
                }
            }
        }

        public void Molotov() {
            foreach (SkillsObject skills in PlayerMovement.skills) {
                if (skills.GetType() == typeof(MolotovSkill)) {
                    MolotovSkill molotov = skills as MolotovSkill;
                    molotov.PlayerMolotov(PlayerMovement);
                }
            }
        }

        public void Shit() {
            foreach (SkillsObject skills in PlayerMovement.skills) {
                if (skills.GetType() == typeof(ShitSkill)) {
                    ShitSkill shit = skills as ShitSkill;
                    shit.PlayerShit(PlayerMovement);
                }
            }
        }

        public void QuickAttack() {
            foreach (SkillsObject skills in PlayerMovement.skills) {
                if (skills.GetType() == typeof(QuickAttackSkill)) {
                    QuickAttackSkill qAttack = skills as QuickAttackSkill;
                    qAttack.PlayerAttack(PlayerMovement);
                }
            }
        }

        public void Cleave() {
            foreach (SkillsObject skills in PlayerMovement.skills) {
                if (skills.GetType() == typeof(CleaveSkill)) {
                    CleaveSkill cleave = skills as CleaveSkill;
                    cleave.PlayerAttack(PlayerMovement);
                }
            }
        }
    }
}