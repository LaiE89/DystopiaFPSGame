using System;
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

        public void AttackDamage() {
            ToolMethods.AlertRadius(PlayerMovement.myWeaponStats.alertRadius, transform.position, PlayerMovement.enemyMask);
            PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.attackRange, PlayerMovement.myWeaponStats.attackDamage * PlayerMovement.damageMultiplier, PlayerMovement.myWeaponStats.attackKnockback, PlayerMovement.myWeaponStats.attackSound, PlayerMovement.myWeaponStats.hurtSound);   
        }

        public void ShootDamage() {
            ToolMethods.AlertRadius(PlayerMovement.myWeaponStats.shootAlertRadius, transform.position, PlayerMovement.enemyMask);
            if (PlayerMovement.myWeaponStats.bullets > 0) {
                PlayerMovement.myWeaponStats.bullets -= 1;
            }
            PlayerMovement.bulletsTextBox.text = ("BULLETS x" + PlayerMovement.myWeaponStats.bullets);
            PlayerMovement.myWeaponStats.muzzleFlash.Play();
            PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.shootRange, PlayerMovement.myWeaponStats.shootDamage, PlayerMovement.myWeaponStats.shootKnockback, PlayerMovement.myWeaponStats.shootSound, PlayerMovement.myWeaponStats.shootHurtSound); 
        }

        public void ShootDamageEnd() {
            if (PlayerMovement.myWeaponStats.bullets <= 0) {
                PlayerMovement.weaponOverrideController["Attack"] = PlayerMovement.myWeaponStats.fpAttackAnimation;
            }
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
    }
}