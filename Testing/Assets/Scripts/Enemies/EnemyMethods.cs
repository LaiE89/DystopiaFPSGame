using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

namespace Enemies {
    public class EnemyMethods : MonoBehaviour {
        [SerializeField] Movement enemyMovement;

        public void walkFootStep() {
            enemyMovement.walkSound.Play();
        }

        public void runFootStep() {
            enemyMovement.runSound.Play();
        }

        public void PlayerDamage() {
            enemyMovement.AttackDamage(enemyMovement.eWeaponStats.attackRange, enemyMovement.eWeaponStats.attackDamage, enemyMovement.eWeaponStats.attackKnockback, enemyMovement.eWeaponStats.attackSound, enemyMovement.eWeaponStats.hurtSound);
        }

        public void StartRotation() {
            enemyMovement.isRotating = true;
        }

        public void DestroyEnemy() {
            Destroy(gameObject);
        }

        public void ShootDamage() {
            enemyMovement.eWeaponStats.bullets -= 1;
            enemyMovement.eWeaponStats.muzzleFlash.Play();
            enemyMovement.AttackDamage(enemyMovement.eWeaponStats.shootRange, enemyMovement.eWeaponStats.shootDamage, enemyMovement.eWeaponStats.shootKnockback, enemyMovement.eWeaponStats.shootSound, enemyMovement.eWeaponStats.shootHurtSound);
        }

        public void ShootDamageEnd() {
            if (enemyMovement.eWeaponStats.bullets == 0) {
                enemyMovement.animatorOverrideController["Punch"] = enemyMovement.eWeaponStats.attackAnimation;
            }
        }
    }
}