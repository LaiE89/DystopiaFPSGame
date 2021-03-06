using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour {
    public bool isDefaultItem;
    public bool isGun;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float staminaCost;
    public float attackKnockback;
    public float maxWeaponHealth;
    public float weaponHealth;
    public float alertRadius;
    public AnimationClip attackAnimation;
    public AnimationClip blockAnimation;
    public AnimationClip fpAttackAnimation;
    public AnimationClip fpBlockAnimation;
    public AudioSource attackSound;
    public AudioSource hurtSound;
    public AudioSource blockSound;
    public AudioSource breakSound;

    [Header("Gun Parameters")]
    public float shootDamage;
    public float shootRange;
    public float shootCooldown;
    public float shootKnockback;
    public int maxBullets;
    public int bullets;
    public float shootAlertRadius;
    public ParticleSystem muzzleFlash;
    public AnimationClip enemyShootAnimation;
    public AnimationClip fpShootAnimation;
    public AudioSource shootSound;
    public AudioSource shootHurtSound;
    public AudioSource reloadSound;
    public AudioSource extraSound;

    public virtual void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Player.PlayerMovement player) {
        Ray ray = player.attackCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        attackSound.Play();
        if (Physics.Raycast(ray, out hit, range, player.playerLayers)) {
            if (hit.collider.tag == "Enemy") {
                hurtSound.Play();
                GameObject enemy = hit.collider.gameObject;
                Enemies.Movement eMovement = enemy.GetComponent<Enemies.Movement>();
                //ParticleSystem blood = Instantiate(SceneController.Instance.bloodParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                //blood.Play();
                SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                player.DealDamage(eMovement, enemy, damage, knockback);
            }else {
                Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact(); 
                }
                SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
            }
        }
    }

    public virtual void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
        //new Vector3(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z)
        // Debug.DrawRay(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height-0.5f, 0), ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.directionToTarget.y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z) * 20, Color.green, 3);
        enemy.isRotating = false;
        attackSound.Play();
        if (Physics.Raycast(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), enemy.GetDirection(), out hit, range, enemy.enemyLayers)) {
            if (hit.collider.tag == "Player") {
                bool hasDamaged = enemy.CombatCalculation(damage, knockback, hurtSound);
                if (hasDamaged) {
                    SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                }
            }else {
                Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact(); 
                }
                SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
            }
        }
    }

    public virtual void SpherecastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        attackSound.Play();
        enemy.isRotating = false;
        RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.GetDirection().y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z), range, enemy.enemyLayers);
        if (hits.Length > 0) {
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Player") {
                    bool hasDamaged = enemy.CombatCalculation(damage, knockback, hurtSound);
                    if (hasDamaged) {
                        SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                    }
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                    destructable.Interact(); 
                    }
                    SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                }
            }
        }
    }

    public void MeleeAttack(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Player.PlayerMovement player) {
        Ray ray = player.attackCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        attackSound.Play();
        if (Physics.Raycast(ray, out hit, range, player.playerLayers)) {
            if (hit.collider.tag == "Enemy") {
                hurtSound.Play();
                GameObject enemy = hit.collider.gameObject;
                Enemies.Movement eMovement = enemy.GetComponent<Enemies.Movement>();
                SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                player.DealDamage(eMovement, enemy, damage, knockback);
            }else {
                Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact(); 
                }
                SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
            }
        }
    }

    public void MeleeAttack(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
        //new Vector3(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z)
        // Debug.DrawRay(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height-0.5f, 0), ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.directionToTarget.y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z) * 20, Color.green, 3);
        enemy.isRotating = false;
        attackSound.Play();
        if (Physics.Raycast(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.GetDirection().y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z), out hit, range, enemy.enemyLayers)) {
            if (hit.collider.tag == "Player") {
                bool hasDamaged = enemy.CombatCalculation(damage, knockback, hurtSound);
                if (hasDamaged) {
                    SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                }
            }else {
                Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact(); 
                }
                SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
            }
        }
    }

    public void PlayerDealDamage(float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, RaycastHit hit, Player.PlayerMovement player) {
        if (hit.collider.tag == "Enemy") {
            hurtSound.Play();
            GameObject enemy = hit.collider.gameObject;
            Enemies.Movement eMovement = enemy.GetComponent<Enemies.Movement>();
            SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
            player.DealDamage(eMovement, enemy, damage, knockback);
            // Debug.Log(enemy.gameObject.name + " took damage");
        }else {
            Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
            if (destructable != null) {
                destructable.Interact(); 
            }
            SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
        }
    }
}
        