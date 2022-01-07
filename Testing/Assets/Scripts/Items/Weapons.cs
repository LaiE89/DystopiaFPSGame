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
    public AnimationClip attackAnimation;
    public AnimationClip blockAnimation;
    public AnimationClip fpAttackAnimation;
    public AnimationClip fpBlockAnimation;
    public string attackSound;
    public string hurtSound;
    public string blockSound;
    public string breakSound;

    [Header("Gun Parameters")]
    public float shootDamage;
    public float shootRange;
    public float shootCooldown;
    public float shootKnockback;
    public int maxBullets;
    public int bullets;
    public ParticleSystem muzzleFlash;
    public AnimationClip enemyShootAnimation;
    public AnimationClip fpShootAnimation;
    public string shootSound;
    public string shootHurtSound;
}
        