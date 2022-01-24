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
            PlayerMovement.soundController.Play("Eating");
            if (PlayerMovement.isConsuming) {
                PlayerMovement.Consuming();
            }
        }

        public void AttackDamage() {
            PlayerMovement.AlertRadius(PlayerMovement.myWeaponStats.alertRadius);
            PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.attackRange, PlayerMovement.myWeaponStats.attackDamage * PlayerMovement.damageMultiplier, PlayerMovement.myWeaponStats.attackKnockback, PlayerMovement.myWeaponStats.attackSound, PlayerMovement.myWeaponStats.hurtSound);   
        }

        public void ShootDamage() {
            PlayerMovement.AlertRadius(PlayerMovement.myWeaponStats.shootAlertRadius);
            PlayerMovement.myWeaponStats.bullets -= 1;
            PlayerMovement.bulletsTextBox.text = ("BULLETS x" + PlayerMovement.myWeaponStats.bullets);
            PlayerMovement.myWeaponStats.muzzleFlash.Play();
            PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.shootRange, PlayerMovement.myWeaponStats.shootDamage, PlayerMovement.myWeaponStats.shootKnockback, PlayerMovement.myWeaponStats.shootSound, PlayerMovement.myWeaponStats.shootHurtSound); 
        }

        public void ShootDamageEnd() {
            if (PlayerMovement.myWeaponStats.bullets == 0) {
                PlayerMovement.weaponOverrideController["Attack"] = PlayerMovement.myWeaponStats.fpAttackAnimation;
            }
        }
    }
}