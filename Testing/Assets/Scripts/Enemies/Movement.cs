using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;

namespace Enemies {
    public class Movement : MonoBehaviour {
        
        [SerializeField] Transform groundCheck;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] public Rigidbody rb;
        [SerializeField] LayerMask whatIsGround, whatIsPlayer;
        [SerializeField] float height;
        [HideInInspector] public GameObject thePlayer;
        LayerMask enemyLayers;
        float groundDistance = 0.2f;
        bool isGrounded;
        AnimatorOverrideController animatorOverrideController;
        Animator animator;
        Player.Health pHealth;
        Player.PlayerMovement pMovement;

        [Header("Patroling")]
        [Range(0,360)] public float angle;
        [SerializeField] float runningSpeed;
        [SerializeField] float walkingSpeed;
        [SerializeField] Vector3[] destinations;
        int destPoint = 0;
        bool targetLocked;
        float distanceToTarget;
        public bool canSeePlayer;

        [Header("Attacking")]
        [SerializeField] GameObject Hand;
        [SerializeField] float timeBetweenAttacks;
        int selectedWeapon;
        GameObject eWeapon;
        Weapons eWeaponStats;
        bool alreadyAttacked;
        bool isRotating;
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

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
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
                    if (distanceToTarget <= eWeaponStats.attackRange - 0.5f) { 
                        AttackPlayer();
                    }
                }else {
                    Walking();
                    if (!agent.pathPending && agent.remainingDistance < 0.5f) {
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
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2) {
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

        public void GoToPlayer() {
            agent.SetDestination(thePlayer.transform.position);
        }

        private void Running() {
            animator.SetBool("isRunning", true);
            agent.speed = runningSpeed;
        }

        private void Walking() {
            animator.SetBool("isRunning", false);
            agent.speed = walkingSpeed;
        }
        

// Combat Methods
        private void AttackPlayer() {
            if (isRotating) {
                Vector3 direction = (thePlayer.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

                /*playerPos = new Vector3(thePlayer.transform.position.x, transform.position.y, thePlayer.transform.position.z);
                var playerRotation = Quaternion.LookRotation(playerPos - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, 10 * Time.deltaTime);
                */
            }
            if (!alreadyAttacked) {
                // Attack code here
                agent.SetDestination(transform.position);
                animator.SetFloat("AttackMultiplier", 1 / eWeaponStats.attackCooldown);
                animator.SetTrigger("isAttacking");
                isRotating = false;
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), eWeaponStats.attackCooldown + timeBetweenAttacks);
            }
        }

        private void ResetAttack() {
            alreadyAttacked = false;
        }

        public void StartRotation() {
            isRotating = true;
        }

        public void PlayerDamage() {
            RaycastHit hit;
            Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            //if (Physics.Raycast(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward), out hit, eWeaponStats.attackRange)) {
            if (Physics.SphereCast(transform.position + new Vector3(0, height - 0.5f, 0), 0.3f, transform.TransformDirection(Vector3.forward), out hit, eWeaponStats.attackRange, enemyLayers)) {
                if (hit.collider.tag == "Player") {
                    if (pMovement.isBlocking) {
                        var forward = transform.TransformDirection(Vector3.forward);
                        var playerForward = thePlayer.transform.TransformDirection(Vector3.forward);
                        var dotProduct = Vector3.Dot(forward, playerForward);

                        if (dotProduct < -0.9) {
                            if (pMovement.myWeaponStats.weaponHealth <= 0 && pMovement.isInjured) {
                                pHealth.TakeDamage(eWeaponStats.attackDamage);
                            }else {
                                print("Blocked!");
                                CameraShaker.Instance.ShakeOnce(eWeaponStats.attackDamage/2, eWeaponStats.attackDamage, 0.1f, 0.5f);
                                if (pMovement.isParrying) {
                                    pMovement.StartParrying();
                                    TakeDamage(1f);
                                    if (Hand.transform.childCount > 1) {
                                        eWeapon.GetComponent<Holdable>().DroppingWeapon(transform);
                                        selectedWeapon = 1;
                                        SwitchWeapon(selectedWeapon);
                                    }
                                }else {
                                    pMovement.BlockingDamage(eWeaponStats.attackDamage);
                                }
                            }
                        }else {
                            print("Failed Block!");
                            pHealth.TakeDamage(eWeaponStats.attackDamage);
                        }
                    }else {
                        pHealth.TakeDamage(eWeaponStats.attackDamage);
                    }
                }
            }else {
                print("Missed Attack.");
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

        public void DestroyEnemy() {
            Destroy(gameObject);
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
            animatorOverrideController["Punch"] = eWeaponStats.attackAnimation;
        }

        /*void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            //Physics.SphereCast(transform.position + new Vector3(0, height - 0.5f, 0), 1f, transform.TransformDirection(Vector3.forward), out hit, eWeaponStats.attackRange)) {
            Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, height - 0.5f, 0), 0.3f);
        }*/
    }
}


