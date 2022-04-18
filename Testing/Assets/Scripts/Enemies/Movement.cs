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
        [SerializeField] public LayerMask groundMask, playerMask;
        [SerializeField] public float height;
        [SerializeField] GameObject Hand;
        [SerializeField] public AudioSource walkSound;
        [SerializeField] public AudioSource runSound;
        [HideInInspector] public GameObject thePlayer;
        [HideInInspector] public SoundController soundController;
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public LayerMask enemyLayers;
        bool isGrounded;
        float groundDistance = 0.1f; // original groundDistance = 0.2, original groundposition - 0.15
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
        [HideInInspector] public bool isKnockedBack;
        [HideInInspector] public bool canSeePlayer;
        [HideInInspector] public bool targetLocked;
        [HideInInspector] public float angleToPlayerHorz;
        [HideInInspector] public Vector3 directionToTarget;
        bool isRunning;
        bool isWalking;
        int destPoint;
        float angleToPlayer;
        float distanceToTarget;
        
        [Header("Attacking Variables")]
        [SerializeField] float timeBetweenAttacks;
        [SerializeField] float shootingAccuracyAngle;
        [SerializeField] float meleeAccuracyAngle = 180;
        [SerializeField] float rotationDegPerSec;
        [SerializeField] public float alertRadius;
        [SerializeField] public float enemyHealth;
        [SerializeField] public SkillsObject[] skills;
        [HideInInspector] public Weapons eWeaponStats;
        [HideInInspector] public bool isRotating;
        [HideInInspector] public bool alreadyAttacked;
        [HideInInspector] public bool isDying;
        [HideInInspector ]public Coroutine skillLagRoutine;
        int selectedWeapon;
        GameObject eWeapon;
        bool isInitialRotation;
        float timeNotSeeing;

        [Header("States Variables")]
        [SerializeField] bool isIdle;
        [SerializeField] public bool isAlertable;
        [SerializeField] public bool isPassive;
        [SerializeField] AnimationClip idleAnimation;
        [HideInInspector] public float damageMultiplier;
        [HideInInspector] public float attackSpeedMultiplier;
        [HideInInspector] public float speedMultiplier;
        [HideInInspector] public bool isChoking;
        Quaternion startingRotation;
        bool isGoingBack;

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

            for (int i = 0; i < skills.Length; i++) {
                this.skills[i] = skills[i].CreateInstance(1);
            }

            UpdatingStatus();
            
            // Initializing Variables
            startingRotation = gameObject.transform.rotation;
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
            enemyLayers = groundMask | playerMask;
            pMovement = thePlayer.GetComponent<Player.PlayerMovement>();
            selectedWeapon = 0;
            SwitchWeapon(selectedWeapon);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Item"), LayerMask.NameToLayer("Enemy"), true);
            if (isIdle) {
                animatorOverrideController["Idle"] = idleAnimation;
            }else {
                GoNextPoint();
            }
            StartCoroutine(FOVRoutine());
            if (skills.Length > 0) {
                StartCoroutine(SkillCheckRoutine());
            }
        }
        
        private void Update() {
            if (isKnockedBack) {
                StartCoroutine(GroundCheckDelay());
            }else {
                if (targetLocked) {
                    Running();
                    if (!alreadyAttacked) {
                        if (agent.isActiveAndEnabled) {
                            if (agent.pathStatus == NavMeshPathStatus.PathPartial) {
                                NavMeshHit hit;
                                if (NavMesh.SamplePosition(thePlayer.transform.position, out hit, 3.0f, groundMask)) {
                                    agent.SetDestination(hit.position);
                                }
                                if (agent.remainingDistance < 0.5f) {
                                    targetLocked = false;
                                    GoNextPoint();
                                }
                            }else {
                                if (canSeePlayer) {
                                    if (timeNotSeeing > 0) {
                                        timeNotSeeing = 0;
                                    }
                                    agent.SetDestination(thePlayer.transform.position);
                                }else { 
                                    agent.SetDestination(thePlayer.transform.position);
                                    timeNotSeeing += Time.deltaTime;
                                    if (timeNotSeeing > 2) {
                                        targetLocked = false;
                                        if (isIdle && Vector3.Distance(gameObject.transform.position, destinations[0]) > 0.5f) {
                                            // GoNextPoint();
                                            isGoingBack = true;
                                        }
                                        Debug.Log(this.name + " stopped following at " + Time.time);
                                        timeNotSeeing = 0;
                                    }
                                }
                            }
                            // agent.SetDestination(thePlayer.transform.position);
                            if (agent.isStopped) {
                                agent.isStopped = false;
                            }
                        }
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
                        if (eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                            Vector3 direction = (thePlayer.transform.position - transform.position).normalized;
                            Quaternion lookRotation = Quaternion.LookRotation(ToolMethods.SettingVector(direction.x, 0, direction.z));
                            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDegPerSec);
                        }else {
                            if (Mathf.Abs(gameObject.transform.position.y - thePlayer.transform.position.y) < 1f) {
                                Vector3 direction = (thePlayer.transform.position - transform.position).normalized;
                                Quaternion lookRotation = Quaternion.LookRotation(ToolMethods.SettingVector(direction.x, 0, direction.z));
                                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDegPerSec);
                            }
                        }
                    }
                }else {
                    if (!isIdle) {
                        Walking();
                        if (agent.isActiveAndEnabled && !agent.pathPending && agent.remainingDistance <= 1f) {
                            GoNextPoint();
                        }
                    }else {
                        if (agent.isActiveAndEnabled && !agent.pathPending && agent.remainingDistance <= 0.05f) {
                            if (Vector3.Distance(gameObject.transform.position, destinations[0]) > 0.05f) {
                                GoNextPoint();
                            }else {
                                animator.SetBool("isWalking", false);
                                animator.SetBool("isRunning", false);
                                if (isGoingBack) {
                                    gameObject.transform.rotation = startingRotation;
                                    isGoingBack = false;
                                }
                            }
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
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRange, playerMask);
            if (rangeChecks.Length != 0) {
                Transform target = rangeChecks[0].transform;
                directionToTarget = (target.position - transform.position).normalized;
                angleToPlayer = Vector3.Angle(transform.forward, directionToTarget);
                angleToPlayerHorz = Vector3.Angle(transform.forward, ToolMethods.SettingVector(directionToTarget.x, 0, directionToTarget.z));
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (angleToPlayer < viewAngle / 2) {
                    // Height used to be 0.75f
                    if (!Physics.Raycast(ToolMethods.OffsetPosition(transform.position, 0, height - 0.5f, 0), directionToTarget, distanceToTarget, groundMask)){
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
            if (!targetLocked && canSeePlayer && !isPassive) {
                targetLocked = true;
                if (agent.isActiveAndEnabled) {
                    timeNotSeeing = 0;
                    agent.SetDestination(thePlayer.transform.position);
                }
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
                agent.speed = runningSpeed * speedMultiplier;
                //agent.acceleration = runningSpeed + 5;
                isWalking = false;
                isRunning = true;
            }
        }

        private void Walking() {
            if (!isWalking) {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", true);
                agent.speed = walkingSpeed * speedMultiplier;
                //agent.acceleration = walkingSpeed + 5;
                isRunning = false;
                isWalking = true;
            }
        }
        

// Combat Methods
        public void UpdatingStatus() {
            attackSpeedMultiplier = 1;
            damageMultiplier = 1;
            speedMultiplier = 1;
        }

        private IEnumerator SkillCheckRoutine(){
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true) {
                yield return wait;
                for (int i = 0; i < skills.Length; i++) {
                    if (skills[i].CanUseSkill(gameObject)) {
                        skills[i].UseSkill(gameObject, thePlayer);
                    }
                    //float sum = (skills[i].useTime + skills[i].cooldown);
                    //Debug.Log("Using skill: " + skills[i] + ", UseCD Time: " + sum + ", Current Time: " + Time.time);
                }
            }
        }

        private void AttackPlayer() {
            if (!alreadyAttacked) {
                // Attack code here
                // agent.SetDestination(transform.position);
                agent.velocity = Vector3.zero;
                if (agent.isActiveAndEnabled) {
                    agent.isStopped = true;
                }
                animator.SetTrigger("isAttacking");
                alreadyAttacked = true;
                if (eWeaponStats.isGun && eWeaponStats.bullets > 0) {
                    animator.SetFloat("AttackMultiplier", 1 / (eWeaponStats.shootCooldown * attackSpeedMultiplier));
                    Invoke(nameof(ResetAttack), eWeaponStats.shootCooldown * attackSpeedMultiplier + timeBetweenAttacks);
                }else {
                    animator.SetFloat("AttackMultiplier", 1 / (eWeaponStats.attackCooldown * attackSpeedMultiplier));
                    Invoke(nameof(ResetAttack), eWeaponStats.attackCooldown * attackSpeedMultiplier + timeBetweenAttacks);
                }
            }
        }

        private void ResetAttack() {
            alreadyAttacked = false;
        }

        public void SpherecastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound) {
            attackSound.Play();
            isRotating = false;
            RaycastHit[] hits = Physics.SphereCastAll(ToolMethods.OffsetPosition(transform.position, 0, height - 0.5f, 0), 0.3f, ToolMethods.SettingVector(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z), range, enemyLayers);
            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) {
                    if (hit.collider.tag == "Player") {
                        CombatCalculation(damage, knockback, hurtSound);
                    }else {
                        Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                        if (destructable != null) {
                        destructable.Interact(); 
                        }
                        ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                        ground.Play();
                    }
                }
            }
        }

        public void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound) {
            RaycastHit hit;
            //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            //new Vector3(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z)
            Debug.DrawRay(ToolMethods.OffsetPosition(transform.position, 0, height-0.5f, 0), ToolMethods.SettingVector(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z) * 20, Color.green, 3);
            isRotating = false;
            attackSound.Play();
            if (Physics.Raycast(ToolMethods.OffsetPosition(transform.position, 0, height - 0.5f, 0), ToolMethods.SettingVector(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z), out hit, range, enemyLayers)) {
                if (hit.collider.tag == "Player") {
                    CombatCalculation(damage, knockback, hurtSound);
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                       destructable.Interact(); 
                    }
                    ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    ground.Play();
                }
            }
        }

        private void CheckGround() {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
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

        public IEnumerator GroundCheckDelay() {
            agent.enabled = false;
            yield return new WaitForSeconds(0.2f);
            CheckGround();
        }

        public IEnumerator LoopingGroundCheckDelay() {
            agent.enabled = false;
            isGrounded = false;
            yield return new WaitForSeconds(0.2f);
            while (!isGrounded) {
                yield return new WaitForEndOfFrame();
                CheckGround();
            }
        }

        public void TakeDamage(float amount) {
            enemyHealth -= amount;
            if (isPassive) {
                isPassive = false;
            }
            animator.ResetTrigger("isAttacking");
            animator.SetTrigger("isDamaged");
            ResetSpeed();
            turnNonKinematic();
            if (skillLagRoutine != null) {
                StopCoroutine(skillLagRoutine);
            }
            if (enemyHealth <= 0) {
                if (Hand.transform.childCount > 1) {
                    eWeapon.GetComponent<Holdable>().DroppingWeapon(transform);
                }
                animator.SetTrigger("isDying");
                isDying = true;
                StartCoroutine(LoopingGroundCheckDelay());
            }
            Debug.Log(gameObject.name + " took some damage. Current Health: " + enemyHealth);
        }

        public IEnumerator TakeFireDamage(int numberOfTicks) {
            for (int i = 0; i < numberOfTicks; i++) {
                TakeDamage(1f);
                ParticleSystem fire = Instantiate(SceneController.Instance.burningParticles, transform.position, transform.rotation) as ParticleSystem;
                fire.Play();
                yield return new WaitForSeconds(1f);
            }
        }

        public void CombatCalculation(float damage, float knockback, AudioSource hurtSound) {
            ToolMethods.AlertRadius(alertRadius, thePlayer.transform.position, pMovement.enemyMask);
            if (hurtSound != null) {
                hurtSound.Play();
            }
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
        }

        public void turnNonKinematic() {
            alreadyAttacked = false;
            if (!targetLocked) {
                isInitialRotation = true;
            }
            isRotating = true;
            rb.isKinematic = false;
            isKnockedBack = true;
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

        public IEnumerator SkillEndingLag(float time, float speed) {
            var instruction = new WaitForEndOfFrame();
            // This needs fixing so it works for dynamic speed multiplier
            float curSpeed = 1;
            this.isRunning = false;
            this.speedMultiplier *= speed;
            while (time > 0) {
                time -= Time.deltaTime;
                yield return instruction;
            }
            this.isRunning = false;
            this.speedMultiplier = curSpeed;
        }

        public void ResetSpeed() {
            this.isRunning = false;
            // Make this dynamic
            this.speedMultiplier = 1;
        }

        /*void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            //Physics.SphereCast(transform.position + new Vector3(0, height - 0.5f, 0), 1f, transform.TransformDirection(Vector3.forward), out hit, eWeaponStats.attackRange)) {
            Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, height - 0.5f, 0), 0.3f);
        }*/
    }
}


