using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;

namespace Enemies {
    public class Movement : MonoBehaviour {
        [Header("=====ESSENTIALS=====")]
        [SerializeField] Transform groundCheck;
        [SerializeField] public NavMeshAgent agent;
        [SerializeField] public Rigidbody rb;
        [SerializeField] LayerMask whatIsGround, whatIsPlayer;
        [SerializeField] float height;
        [SerializeField] GameObject Hand;
        [SerializeField] public AudioSource walkSound;
        [SerializeField] public AudioSource runSound;
        [HideInInspector] public GameObject thePlayer;
        [HideInInspector] public SoundController soundController;
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;
        LayerMask enemyLayers;
        float groundDistance = 0.2f;
        bool isGrounded;
        Animator animator;
        Player.PlayerMovement pMovement;

        [Header("=====STATS=====", order=0)]

        [Header("Patroling Variables", order=1)]
        [Range(0,360)] public float viewAngle;
        [SerializeField] float runningSpeed;
        [SerializeField] float walkingSpeed;
        [SerializeField] public float sightRange;
        // [SerializeField] float startingYRotation;
        [SerializeField] Vector3[] destinations;
        [HideInInspector] bool playerInSightRange;
        [HideInInspector] bool playerInAttackRange;
        [HideInInspector] bool isKnockedDown;
        [HideInInspector] public bool isKnockedBack;
        [HideInInspector] public bool canSeePlayer;
        [HideInInspector] public bool targetLocked;
        bool isRunning;
        bool isWalking;
        int destPoint;
        float angleToPlayer;
        float angleToPlayerHorz;
        float distanceToTarget;
        Vector3 directionToTarget;
        
        [Header("Attacking Variables")]
        [SerializeField] float timeBetweenAttacks;
        [SerializeField] float shootingAccuracyAngle;
        [SerializeField] float meleeAccuracyAngle = 180;
        [SerializeField] float rotationDegPerSec;
        [SerializeField] float alertRadius;
        [SerializeField] public float enemyHealth;
        [HideInInspector] public Weapons eWeaponStats;
        [HideInInspector] public bool isRotating;
        [HideInInspector] public bool alreadyAttacked;
        [HideInInspector] public bool isDying;
        int selectedWeapon;
        GameObject eWeapon;
        bool isInitialRotation;

        [Header("States Variables")]
        [SerializeField] bool isIdle;
        [SerializeField] public bool isAlertable;
        [SerializeField] AnimationClip idleAnimation;

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            SceneController.Instance.listOfEnemies.Add(this);

            foreach (Transform weapon in Hand.transform) {
                if (weapon.GetComponent<Weapons>().isDefaultItem) {
                    weapon.SetAsLastSibling();
                }
            }
            
            // Initializing Variables
            isWalking = false;
            isInitialRotation = false;
            isDying = false;
            targetLocked = false;
            destPoint = 0;
            agent.speed = walkingSpeed;
            soundController = SceneController.Instance.soundController;
            thePlayer = SceneController.Instance.playerObject;
            animator = GetComponent<Animator>();
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
            rb.freezeRotation = true;
            enemyLayers = whatIsGround | whatIsPlayer;
            pMovement = thePlayer.GetComponent<Player.PlayerMovement>();
            selectedWeapon = 0;
            SwitchWeapon(selectedWeapon);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Enemy"), true);
            if (isIdle) {
                animatorOverrideController["Idle"] = idleAnimation;
            }else {
                GoNextPoint();
            }
            StartCoroutine(FOVRoutine());
        }
        
        private void Update() {
            if (isKnockedBack) {
                StartCoroutine(GroundCheckDelay());
            }else {
                if (targetLocked) {
                    Running();
                    if (!alreadyAttacked) {
                        if (agent.isStopped) {
                            agent.isStopped = false;
                        }
                        agent.SetDestination(thePlayer.transform.position);
                    }
                    if (isInitialRotation) {
                        if (canSeePlayer && eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                            if (distanceToTarget <= eWeaponStats.shootRange - 1f && distanceToTarget > eWeaponStats.attackRange && angleToPlayerHorz < shootingAccuracyAngle) {
                                AttackPlayer();
                                isInitialRotation = false;
                            }else if (distanceToTarget <= eWeaponStats.attackRange && angleToPlayerHorz < meleeAccuracyAngle) {
                                AttackPlayer();
                                isInitialRotation = false;
                            }
                        }else {
                            if (canSeePlayer && distanceToTarget <= eWeaponStats.attackRange && angleToPlayerHorz < meleeAccuracyAngle) { 
                                AttackPlayer();
                                isInitialRotation = false;
                            }
                        }
                    }else {
                        if (eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                            if (distanceToTarget <= eWeaponStats.shootRange - 1f && canSeePlayer) {
                                AttackPlayer(); 
                            }
                        }else {
                            if (distanceToTarget <= eWeaponStats.attackRange) {
                                AttackPlayer();
                            }
                        }
                    }
                    if (isRotating) {
                        Vector3 direction = (thePlayer.transform.position - transform.position).normalized;
                        Quaternion lookRotation = Quaternion.LookRotation(ToolMethods.SettingVector(direction.x, 0, direction.z));
                        //new Vector3(direction.x, 0, direction.z)
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDegPerSec);
                    }
                }else {
                    if (!isIdle) {
                        Walking();
                        if (!agent.pathPending && agent.remainingDistance <= 1f) {
                            GoNextPoint();
                        }
                    }else {
                        /*if (Vector3.Distance(transform.position, destinations[0]) > 0.2f && !awayFromIdlePos) {
                            GoNextPoint();
                            Walking();
                            awayFromIdlePos = true;
                        }else if (Vector3.Distance(transform.position, destinations[0]) <= 0.2f && awayFromIdlePos){
                            transform.rotation = Quaternion.Euler(0, startingYRotation, 0);
                            animator.SetBool("isWalking", false);
                            animator.SetBool("isRunning", false);
                            awayFromIdlePos = false;
                        }*/
                        if (!agent.pathPending && agent.remainingDistance <= 1f) {
                            animator.SetBool("isWalking", false);
                            animator.SetBool("isRunning", false);
                        }else {
                            Walking();
                        }
                    }
                }
            }
        }

// Cone Detection
        private IEnumerator FOVRoutine(){
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true) {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck(){
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRange, whatIsPlayer);
            if (rangeChecks.Length != 0) {
                Transform target = rangeChecks[0].transform;
                directionToTarget = (target.position - transform.position).normalized;
                angleToPlayer = Vector3.Angle(transform.forward, directionToTarget);
                angleToPlayerHorz = Vector3.Angle(transform.forward, ToolMethods.SettingVector(directionToTarget.x, 0, directionToTarget.z));
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (angleToPlayer < viewAngle / 2) {
                    if (!Physics.Raycast(ToolMethods.OffsetPosition(transform.position, 0, 0.75f, 0), directionToTarget, distanceToTarget, whatIsGround)){
                        if (!targetLocked) {
                            isInitialRotation = true;
                        }
                        canSeePlayer = true;
                    }else {
                        canSeePlayer = false;
                    }
                }else {
                    canSeePlayer = false;
                }
            }else if (canSeePlayer) {
                canSeePlayer = false;
            }
            if (!targetLocked && canSeePlayer) {
                targetLocked = true;
            }else if (targetLocked && !canSeePlayer && rangeChecks.Length == 0){
                if (agent.isActiveAndEnabled && agent.isStopped) {
                    agent.isStopped = false;
                }
                targetLocked = false;
                isInitialRotation = false;
            }
        }


// Movement Methods
        private void GoNextPoint(){
            if(destinations.Length == 0){
                return;
            }
            agent.SetDestination(destinations[destPoint]);
            //agent.destination = destinations[destPoint];
            destPoint = (destPoint + 1) % destinations.Length;
        }

        private void Running() {
            if (!isRunning) {
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);
                agent.speed = runningSpeed;
                //agent.acceleration = runningSpeed + 5;
                isWalking = false;
                isRunning = true;
            }
        }

        private void Walking() {
            if (!isWalking) {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", true);
                agent.speed = walkingSpeed;
                //agent.acceleration = walkingSpeed + 5;
                isRunning = false;
                isWalking = true;
            }
        }
        

// Combat Methods

        private void AttackPlayer() {
            if (!alreadyAttacked) {
                // Attack code here
                // agent.SetDestination(transform.position);
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                animator.SetTrigger("isAttacking");
                alreadyAttacked = true;
                if (eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                    animator.SetFloat("AttackMultiplier", 1 / eWeaponStats.shootCooldown);
                    Invoke(nameof(ResetAttack), eWeaponStats.shootCooldown + timeBetweenAttacks);
                }else {
                    animator.SetFloat("AttackMultiplier", 1 / eWeaponStats.attackCooldown);
                    Invoke(nameof(ResetAttack), eWeaponStats.attackCooldown + timeBetweenAttacks);
                }
            }
        }

        private void ResetAttack() {
            alreadyAttacked = false;
        }

        public void SpherecastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound) {
            //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            //new Vector3(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z)
            //Debug.DrawRay(ToolMethods.OffsetPosition(transform.position, 0, height-0.5f, 0), ToolMethods.SettingPosition(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z), Color.green, 3);
            attackSound.Play();
            isRotating = false;
            RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(transform.position, 0, height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z), range, enemyLayers);
            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) {
                    if (hit.collider.tag == "Player") {
                        ToolMethods.AlertRadius(alertRadius, thePlayer.transform.position, pMovement.enemyMask);
                        hurtSound.Play();
                        Vector3 direction = thePlayer.transform.position - transform.position;
                        direction.y = (float)(Math.Sin(-transform.rotation.x * Math.PI/180) * knockback);
                        pMovement.rb.AddForce(direction.normalized * knockback, ForceMode.Impulse);
                        if (pMovement.isBlocking) {
                            var forward = transform.TransformDirection(Vector3.forward);
                            var playerForward = thePlayer.transform.TransformDirection(Vector3.forward);
                            var dotProduct = Vector3.Dot(forward, playerForward);

                            if (dotProduct < -0.9) {
                                if (pMovement.myWeaponStats.weaponHealth <= 0 && pMovement.statusEffects.Contains("isInjured")) {
                                    pMovement.TakeDamage(damage);
                                }else {
                                    CameraShaker.Instance.ShakeOnce(damage/2, damage, 0.1f, 0.5f);
                                    pMovement.UsingStamina(damage * 10f);
                                    if (pMovement.isParrying) {
                                        soundController.PlayOneShot("Parry");
                                        pMovement.StartParrying();
                                        TakeDamage(1f);
                                        if (Hand.transform.childCount > 1) {
                                            eWeapon.GetComponent<Holdable>().DroppingWeapon(transform);
                                            selectedWeapon = 1;
                                            SwitchWeapon(selectedWeapon);
                                        }
                                    }else {
                                        pMovement.BlockingDamage(damage);
                                    }
                                }
                            }else {
                                pMovement.TakeDamage(damage);
                            }
                        }else {
                            pMovement.TakeDamage(damage);
                        }
                    }else {
                        Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                        if (destructable != null) {
                        destructable.Interact(); 
                        }
                        ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                        ground.Play();
                        Destroy(ground.gameObject, 0.5f);
                    }
                }
            }
        }

        public void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound) {
            RaycastHit hit;
            //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            //new Vector3(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z)
            //Debug.DrawRay(ToolMethods.OffsetPosition(transform.position, 0, height-0.5f, 0), ToolMethods.SettingPosition(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z), Color.green, 3);
            isRotating = false;
            attackSound.Play();
            if (Physics.Raycast(ToolMethods.OffsetPosition(transform.position, 0, height - 0.5f, 0), ToolMethods.SettingVector(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z), out hit, range, enemyLayers)) {
                if (hit.collider.tag == "Player") {
                    ToolMethods.AlertRadius(alertRadius, thePlayer.transform.position, pMovement.enemyMask);
                    hurtSound.Play();
                    Vector3 direction = thePlayer.transform.position - transform.position;
                    direction.y = (float)(Math.Sin(-transform.rotation.x * Math.PI/180) * knockback);
                    pMovement.rb.AddForce(direction.normalized * knockback, ForceMode.Impulse);
                    if (pMovement.isBlocking) {
                        var forward = transform.TransformDirection(Vector3.forward);
                        var playerForward = thePlayer.transform.TransformDirection(Vector3.forward);
                        var dotProduct = Vector3.Dot(forward, playerForward);

                        if (dotProduct < -0.9) {
                            if (pMovement.myWeaponStats.weaponHealth <= 0 && pMovement.statusEffects.Contains("isInjured")) {
                                pMovement.TakeDamage(damage);
                            }else {
                                CameraShaker.Instance.ShakeOnce(damage/2, damage, 0.1f, 0.5f);
                                pMovement.UsingStamina(damage * 10f);
                                if (pMovement.isParrying) {
                                    soundController.PlayOneShot("Parry");
                                    pMovement.StartParrying();
                                    TakeDamage(1f);
                                    if (Hand.transform.childCount > 1) {
                                        eWeapon.GetComponent<Holdable>().DroppingWeapon(transform);
                                        selectedWeapon = 1;
                                        SwitchWeapon(selectedWeapon);
                                    }
                                }else {
                                    pMovement.BlockingDamage(damage);
                                }
                            }
                        }else {
                            pMovement.TakeDamage(damage);
                        }
                    }else {
                        pMovement.TakeDamage(damage);
                    }
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                       destructable.Interact(); 
                    }
                    ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    ground.Play();
                    Destroy(ground.gameObject, 0.5f);
                }
            }
        }

        private void CheckGround() {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
            if (isGrounded && isKnockedBack) {
                animator.ResetTrigger("isDamaged");
                rb.isKinematic = true;
                if (!isDying) {
                    agent.enabled = true;
                    isKnockedBack = false;
                    agent.SetDestination(thePlayer.transform.position);
                }
            }
        }

        private IEnumerator GroundCheckDelay() {
            agent.enabled = false;
            yield return new WaitForSeconds(0.2f);
            CheckGround();
        }

        public void TakeDamage(float amount) {
            enemyHealth -= amount;
            alreadyAttacked = false;
            if (!targetLocked) {
                isInitialRotation = true;
            }
            isRotating = true;
            animator.ResetTrigger("isAttacking");
            animator.SetTrigger("isDamaged");
            if (enemyHealth <= 0) {
                if (Hand.transform.childCount > 1) {
                    eWeapon.GetComponent<Holdable>().DroppingWeapon(transform);
                }
                animator.SetTrigger("isDying");
                isDying = true;
                StartCoroutine(GroundCheckDelay());
            }
            rb.isKinematic = false;
            isKnockedBack = true;
            print(gameObject.name + " took some damage. Current Health: " + enemyHealth);
        }

        private void SwitchWeapon(int selectedWeapon) {
            if (selectedWeapon > Hand.transform.childCount - 1) {
                selectedWeapon = 0;
            }else if (selectedWeapon < 0) {
                selectedWeapon = Hand.transform.childCount - 1;
            }
            
            int i = 0;
            foreach (Transform weapon in Hand.transform) {
                if (i == selectedWeapon) {
                    weapon.gameObject.SetActive(true);
                    eWeapon = weapon.gameObject;
                }else {
                    weapon.gameObject.SetActive(false);
                }
                i++; 
            }
            eWeaponStats = eWeapon.GetComponent<Weapons>();

            if (eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                animatorOverrideController["Punch"] = eWeaponStats.enemyShootAnimation;
            }else {
                animatorOverrideController["Punch"] = eWeaponStats.attackAnimation;
            }
        }

        /*public void AlertOthers() {
            Movement[] list = SceneController.Instance.listOfEnemies;
            for (int i = 0; i < list.Length; i++) {
                Debug.Log("listOfEnemies[i] : " + SceneController.Instance.listOfEnemies[i]);
                Debug.Log("This: " + this);
                if (list[i] != this && list[i] != null && list[i].agent.enabled) {
                    list[i].agent.SetDestination(thePlayer.transform.position);
                }
            }
        }*/

        /*void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            //Physics.SphereCast(transform.position + new Vector3(0, height - 0.5f, 0), 1f, transform.TransformDirection(Vector3.forward), out hit, eWeaponStats.attackRange)) {
            Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, height - 0.5f, 0), 0.3f);
        }*/
    }
}


