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
            PlayerMovement.Consuming();
        }

        public void AttackDamage() {
            PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.attackRange, PlayerMovement.myWeaponStats.attackDamage, PlayerMovement.myWeaponStats.attackKnockback);   
        }

        public void ShootDamage() {
            PlayerMovement.myWeaponStats.muzzleFlash.Play();
            PlayerMovement.AttackDamage(PlayerMovement.myWeaponStats.shootRange, PlayerMovement.myWeaponStats.shootDamage, PlayerMovement.myWeaponStats.shootKnockback); 
        }

        public void ShootDamageEnd() {
            if (PlayerMovement.myWeaponStats.bullets == 0) {
                PlayerMovement.weaponOverrideController["Attack"] = PlayerMovement.myWeaponStats.fpAttackAnimation;
            }
        }
    }
}