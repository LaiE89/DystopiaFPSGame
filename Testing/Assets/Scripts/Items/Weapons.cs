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
}
        