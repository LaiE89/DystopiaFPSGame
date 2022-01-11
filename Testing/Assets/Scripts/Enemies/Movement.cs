using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;

namespace Enemies {
    public class Movement : MonoBehaviour {
        
        [SerializeField] Transform groundCheck;
        [SerializeField] public NavMeshAgent agent;
        [SerializeField] public Rigidbody rb;
        [SerializeField] LayerMask whatIsGround, whatIsPlayer;
        [SerializeField] float height;
        [HideInInspector] public GameObject thePlayer;
        [HideInInspector] public SoundController soundController;
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;
        LayerMask enemyLayers;
        float groundDistance = 0.2f;
        bool isGrounded;
        Animator animator;
        Player.Health pHealth;
        Player.PlayerMovement pMovement;

        [Header("Patroling")]
        [Range(0,360)] public float angle;
        [SerializeField] float runningSpeed;
        [SerializeField] float walkingSpeed;
        [SerializeField] Vector3[] destinations;
        bool isRunning;
        bool isWalking;
        int destPoint = 0;
        bool targetLocked;
        float angleToPlayer;
        float distanceToTarget;
        public bool canSeePlayer;

        [Header("Attacking")]
        [SerializeField] GameObject Hand;
        [SerializeField] float timeBetweenAttacks;
        [SerializeField] float shootingAccuracyAngle;
        [SerializeField] float meleeAccuracyAngle = 180; // or else the enemy might circle the player
        [SerializeField] float rotationDegPerSec;
        [SerializeField] float alertRadius;
        [HideInInspector] public Weapons eWeaponStats;
        [HideInInspector] public bool isRotating;
        [HideInInspector] public bool alreadyAttacked;
        int selectedWeapon;
        GameObject eWeapon;
        Vector3 playerPos;

        [Header("States")]
        [HideInInspector] bool playerInSightRange;
        [HideInInspector] bool playerInAttackRange;
        [HideInInspector] bool isKnockedDown;
        [HideInInspector] public bool isKnockedBack;
        [SerializeField] public float sightRange;

        [Header("Health")]
        public float enemyHealth;
        bool isDying = false;

        [Header("Sounds")]
        [SerializeField] public AudioSource walkSound;
        [SerializeField] public AudioSource runSound;

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            isWalking = false;
            soundController = SceneController.Instance.soundController;
            thePlayer = SceneController.Instance.playerObject;
            animator = GetComponent<Animator>();
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
            rb.freezeRotation = true;
            enemyLayers = whatIsGround | whatIsPlayer;
            pHealth = thePlayer.GetComponent<Player.Health>();
            pMovement = thePlayer.GetComponent<Player.PlayerMovement>();
            selectedWeapon = 0;
            SwitchWeapon(selectedWeapon);
            StartCoroutine(FOVRoutine());
        }
        
        private void Update() {
            if (isKnockedBack) {
                StartCoroutine(GroundCheckDelay());
            }else {
                if (targetLocked) {
                    Running();
                    if (!alreadyAttacked) {
                        agent.SetDestination(thePlayer.transform.position);
                    }
                    if (eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                        if (canSeePlayer && distanceToTarget <= eWeaponStats.shootRange - 1f && angleToPlayer < shootingAccuracyAngle) { 
                            AttackPlayer();
                        }
                    }else {
                        if (canSeePlayer && distanceToTarget <= eWeaponStats.attackRange - 0.5f && angleToPlayer < meleeAccuracyAngle) { 
                            AttackPlayer();
                        }
                    }
                }else {
                    Walking();
                    if (!agent.pathPending && agent.remainingDistance <= 1f) {
                        GoNextPoint();
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
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                angleToPlayer = Vector3.Angle(transform.forward, directionToTarget);
                if (angleToPlayer < angle / 2) {
                    distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z), directionToTarget, distanceToTarget, whatIsGround)){
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
            if (canSeePlayer) {
                targetLocked = true;
            }else if (!canSeePlayer && rangeChecks.Length == 0){
                targetLocked = false;
            }
        }


// Movement Methods
        private void GoNextPoint(){
            if(destinations.Length == 0){
                return;
            }
            agent.destination = destinations[destPoint];
            destPoint = (destPoint + 1) % destinations.Length;
        }

        private void Running() {
            if (!isRunning) {
                animator.SetBool("isRunning", true);
                agent.speed = runningSpeed;
                agent.acceleration = runningSpeed + 5;
                isWalking = false;
                isRunning = true;
            }
        }

        private void Walking() {
            if (!isWalking) {
                animator.SetBool("isRunning", false);
                agent.speed = walkingSpeed;
                agent.acceleration = walkingSpeed + 5;
                isRunning = false;
                isWalking = true;
            }
        }
        

// Combat Methods

        private void AttackPlayer() {
            if (isRotating) {
                Vector3 direction = (thePlayer.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDegPerSec);
            }

            if (!alreadyAttacked) {
                // Attack code here
                agent.SetDestination(transform.position);
                animator.SetTrigger("isAttacking");
                isRotating = false;
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

        public void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound) {
            RaycastHit hit;
            //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            //if (Physics.Raycast(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward), out hit, eWeaponStats.attackRange)) {
            attackSound.Play();
            if (Physics.SphereCast(transform.position + new Vector3(0, height - 0.5f, 0), 0.3f, transform.TransformDirection(Vector3.forward), out hit, range, enemyLayers)) {
                if (hit.collider.tag == "Player") {
                    SceneController.Instance.player.AlertRadius(alertRadius);
                    hurtSound.Play();
                    if (pMovement.isBlocking) {
                        var forward = transform.TransformDirection(Vector3.forward);
                        var playerForward = thePlayer.transform.TransformDirection(Vector3.forward);
                        var dotProduct = Vector3.Dot(forward, playerForward);

                        if (dotProduct < -0.9) {
                            if (pMovement.myWeaponStats.weaponHealth <= 0 && pMovement.isInjured) {
                                pHealth.TakeDamage(damage);
                            }else {
                                CameraShaker.Instance.ShakeOnce(damage/2, damage, 0.1f, 0.5f);
                                if (pMovement.isParrying) {
                                    soundController.Play("Parry");
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
                            pHealth.TakeDamage(damage);
                        }
                    }else {
                        pHealth.TakeDamage(damage);
                    }
                }else {
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


