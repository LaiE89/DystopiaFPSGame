using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EZCameraShake;

namespace Player {
    public class PlayerMovement : MonoBehaviour {

        [Header("Keybinds")]
        [SerializeField] KeyCode jumpKey = KeyCode.Space;
        [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] KeyCode forwardKey = KeyCode.W;
        [SerializeField] KeyCode leftKey = KeyCode.A;
        [SerializeField] KeyCode backwardKey = KeyCode.S;
        [SerializeField] KeyCode rightKey = KeyCode.D;
        [SerializeField] KeyCode consumeKey = KeyCode.E;
        [SerializeField] KeyCode pickUpKey = KeyCode.Q;

        [Header("Mouse Movement")]
        [SerializeField] float startingYRotation;
        [SerializeField] Transform mouseCam;
        [SerializeField] Transform orientation;
        private static float sensX;
        private static float sensY;
        float mouseX;
        float mouseY;
        float xRotation;
        float yRotation; 
        public static float newSens;

        [Header("Ground Detection")]
        [SerializeField] LayerMask groundMask;
        [SerializeField] LayerMask enemyMask;
        [SerializeField] Transform groundCheck;
        [HideInInspector] public bool isGrounded;
        float groundDistance = 0.2f;
        RaycastHit slopeHit;
        //Vector3 slopeMoveDirection;

        [Header("Movement")]
        [SerializeField] float moveSpeed;
        [SerializeField] public float walkSpeed;
        [SerializeField] public float sprintSpeed;
        float acceleration = 10f;
        float horizontalMovement;
        float verticalMovement;
        Vector3 moveDirection;
        bool isMovingLR;
        bool isMovingForward;
        bool isMovingBackward;
        bool isRunning;

        [Header("Stamina")]
        [SerializeField] Slider staminaSlider;
        [SerializeField] float maxPlayerStamina;
        [SerializeField] float timeBeforeStamina;
        float playerStamina;
        Coroutine isRegenStamina;

        [Header("Jumping")]
        [SerializeField] public float jumpForce;

        [Header("Attacking")]
        [SerializeField] public TextMeshProUGUI bulletsTextBox;
        [SerializeField] public Camera attackCam;
        [SerializeField] GameObject hand;
        [SerializeField] GameObject firstPersonView;
        [SerializeField] Material injuredArmMaterial;
        [SerializeField] Material healedArmMaterial;
        [SerializeField] GameObject boneWeapon;
        [SerializeField] public GameObject myWeapon;
        [HideInInspector] public bool isBlocking;
        [HideInInspector] public bool isParrying;
        [HideInInspector] public bool isInjured;
        [HideInInspector] public Weapons myWeaponStats;
        LayerMask playerLayers;
        SkinnedMeshRenderer armMeshRenderer;
        Weapons defaultWeaponStats;
        Animator weaponAnimator;
        public AnimatorOverrideController weaponOverrideController;
        float attackTimer;
        bool isAttacking;
        GameObject savedWeapon;

        [Header("Consume")]
        [SerializeField] public int playerDrugs;
        [SerializeField] public TextMeshProUGUI drugsTextBox;
        bool isConsuming;

        [Header("Pick Up")]
        [SerializeField] public int pickUpRange;
        [SerializeField] public TextMeshProUGUI interactTextBox;
        int selectedWeapon;

        [Header("Others")]
        [HideInInspector] public SoundController soundController;
        [HideInInspector] public int sceneIndex;
        [HideInInspector] public Rigidbody rb;
        Health health;
        GameObject enemy;
        Enemies.Movement eMovement;
        Rigidbody eRb;

        private void Awake() {
            newSens = OptionsMenu.sens;
            sensX = newSens;
            sensY = newSens;
            
            weaponAnimator = firstPersonView.GetComponent<Animator>();
            armMeshRenderer = firstPersonView.GetComponentInChildren<SkinnedMeshRenderer>();

            weaponOverrideController = new AnimatorOverrideController(weaponAnimator.runtimeAnimatorController);
            weaponAnimator.runtimeAnimatorController = weaponOverrideController;

            if (MainMenu.loading) {
                LoadPlayer();
                MainMenu.loading = false;
            }
        }

        private void Start() {
            yRotation = startingYRotation;
            soundController = SceneController.Instance.soundController;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            sceneIndex = SceneController.sceneIndex + 1;
            
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            defaultWeaponStats = hand.transform.GetChild(0).gameObject.GetComponent<Weapons>();
            defaultWeaponStats.weaponHealth = defaultWeaponStats.maxWeaponHealth; 
            selectedWeapon = 0;
            SwitchWeapon(selectedWeapon);

            health = GetComponent<Player.Health>();

            staminaSlider.maxValue = maxPlayerStamina;
            playerStamina = maxPlayerStamina;
            staminaSlider.value = playerStamina;
            drugsTextBox.text = ("DRUGS x " + playerDrugs);

            playerLayers = groundMask | enemyMask;

            if (savedWeapon != null) {
                savedWeapon.GetComponent<Interactable>().Interact(hand.transform);
                SwitchWeapon(savedWeapon.transform.GetSiblingIndex());
            }
        }

        private void Update() {
            if (!ingameMenus.pausedGame) {
                // Checking ground
                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                
                // Essentials
                ControlDrag();
                MyInput();
                MouseInput();

                // Key Inputs
                Movement();
                Jump();
                Attack();
                Block();
                Consume();
                PickUp();

                // Faster Falling 
                if (rb.velocity.y < 0) {
                    rb.velocity += Vector3.up * Physics.gravity.y * 1.5f * Time.deltaTime;
                } 

                // Slope Movement
                // slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
            }
        }

        private void FixedUpdate() {
            MovePlayer();  
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        public static void SettingChanges() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            newSens = OptionsMenu.sens;
            sensX = newSens;
            sensY = newSens;
        }

        public void UsingStamina(float amount) {
            playerStamina -= amount;
            if (playerStamina < 0) {
                playerStamina = 0;
            }
            staminaSlider.value = playerStamina;
            if (isRegenStamina != null) {
                StopCoroutine(isRegenStamina);
            }
            isRegenStamina = StartCoroutine(RegenStamina());
        }

        private IEnumerator RegenStamina() {
            yield return new WaitForSeconds(timeBeforeStamina);
            while (playerStamina < maxPlayerStamina) {
                playerStamina += maxPlayerStamina / 100;
                staminaSlider.value = playerStamina;
                yield return new WaitForSeconds(0.1f);
            }
            isRegenStamina = null;
        }

        private void MyInput() {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            verticalMovement = Input.GetAxisRaw("Vertical");

            moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        }

        void MouseInput() {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");
            yRotation += mouseX * sensX * 0.01f;
            xRotation -= mouseY * sensY * 0.01f;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            mouseCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        private void Jump() {
            if (Input.GetKeyDown(jumpKey) && isGrounded && !isConsuming) {
                if (playerStamina >= 20f) {
                    soundController.Stop("PlayerRun", 0.25f);
                    soundController.Play("PlayerJump");
                    UsingStamina(20f);
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    StartCoroutine(JumpDelay());
                }
            }
        }

        private IEnumerator JumpDelay() {
            yield return new WaitForSeconds(0.1f);
            isRunning = false;
        }
        
        private void Movement() {
            if (isGrounded && !isConsuming) {
                if (Input.GetKey(sprintKey) && Input.GetKey(forwardKey)) {
                    Sprint();
                }else {
                    Walk();
                }
            }
        }

        private void Walk() {
            if (isRunning) {
                soundController.Stop("PlayerRun", 0.25f);
                isRunning = false;
            }
            // moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            moveSpeed = walkSpeed;
        }

        private void Sprint() {
            if (playerStamina > 20f) {
                if (!isRunning) {
                    soundController.Play("PlayerRun");
                    isRunning = true;
                }
                UsingStamina(Time.deltaTime * 20f); // 0.2f
                moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
            }else {
                Walk();
            }
        }

        private void Attack() {
            attackTimer += Time.deltaTime;
            if (Input.GetMouseButton(0) && !isConsuming) {
                if (myWeaponStats.isGun && myWeaponStats.bullets > 0) {
                    if (attackTimer >= myWeaponStats.shootCooldown) {
                        isAttacking = true;
                        attackTimer = 0f;
                        weaponAnimator.SetFloat("AttackMultiplier", 1 / myWeaponStats.shootCooldown);
                        weaponAnimator.SetTrigger("isAttacking");
                    }
                }else {
                    if (attackTimer >= myWeaponStats.attackCooldown) {
                        if (playerStamina >= myWeaponStats.staminaCost) {
                            UsingStamina(myWeaponStats.staminaCost);
                            isAttacking = true;
                            attackTimer = 0f;
                            weaponAnimator.SetFloat("AttackMultiplier", 1 / myWeaponStats.attackCooldown);
                            weaponAnimator.SetTrigger("isAttacking");
                        }
                    }
                }
            }
            if (attackTimer >= myWeaponStats.attackCooldown) {
                isAttacking = false;
            }
        }

        private void Block() {
            if (Input.GetMouseButton(1) && !Input.GetMouseButton(0) && !isAttacking && !isConsuming) {
                if (!isBlocking) {
                    isParrying = true;
                }
                weaponAnimator.SetBool("isBlocking", true);
                isBlocking = true;
                moveSpeed = Mathf.Lerp(moveSpeed, moveSpeed / 2, acceleration * Time.deltaTime);
            }else {
                StopBlocking();
            }
        }

        private void StopBlocking() {
            weaponAnimator.SetBool("isBlocking", false);
            // animator.SetBool("isBlocking", false);
            isBlocking = false;
            isParrying = false;
        }

        public void BlockingDamage(float amount) {
            myWeaponStats.blockSound.Play();
            myWeaponStats.weaponHealth -= amount;
            if (!myWeaponStats.isDefaultItem) {
                if (myWeaponStats.weaponHealth <= 0) {
                    myWeaponStats.breakSound.Play();
                    Destroy(myWeapon);
                    selectedWeapon = 0;
                    SwitchWeapon(selectedWeapon);
                }
            }else {
                if (myWeaponStats.weaponHealth <= 0) {
                    CameraShaker.Instance.ShakeOnce(amount*4, amount, 0.1f, 0.5f);
                    myWeaponStats.breakSound.Play();
                    GameObject droppedBone =  Instantiate(boneWeapon, hand.transform.position, transform.rotation) as GameObject;
                    droppedBone.GetComponent<Holdable>().CreatingWeapon(transform);
                    armMeshRenderer.material = injuredArmMaterial;
                    isInjured = true;
                }
            }
        }

        public void StartParrying() {
            weaponAnimator.SetBool("isBlocking", false);
            weaponAnimator.SetTrigger("isParrying");
        }

        public void StopParrying() {
            isParrying = false;
        }

        private void Consume() {
            if (Input.GetKeyDown(consumeKey) && !isAttacking && !isConsuming && !isRunning && playerDrugs > 0) {
                StopBlocking();
                isConsuming = true; 
                weaponAnimator.SetTrigger("isConsuming");
            }
        }

        public void Consuming() {
            isConsuming = false;
            playerDrugs -= 1;
            drugsTextBox.text = ("DRUGS x " + playerDrugs);
            if (health.playerHealth < health.maxPlayerHealth) {
                health.playerHealth += 1;
            }
            if (isInjured) {
                isInjured = false;
                armMeshRenderer.material = healedArmMaterial;
                defaultWeaponStats.weaponHealth = defaultWeaponStats.maxWeaponHealth;
            }
        }

        private void PickUp() {
            if (Input.GetKeyDown(pickUpKey) && !isAttacking && !isBlocking) {
                Ray ray = attackCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, pickUpRange)) {
                    switch (hit.collider.tag) {
                        case "Item":
                            if (hit.collider.transform.parent == null) {
                                GameObject targetWeapon = hit.collider.gameObject;
                                targetWeapon.GetComponent<Interactable>().Interact(hand.transform);
                                SwitchWeapon(targetWeapon.transform.GetSiblingIndex());
                                if (hand.transform.childCount > 2) {
                                    hand.transform.GetChild(targetWeapon.transform.GetSiblingIndex()-1).gameObject.GetComponent<Holdable>().DroppingWeapon(transform);
                                } 
                            }
                            break;
                        case "Appliance":
                            GameObject targetAppliance = hit.collider.gameObject;
                            targetAppliance.GetComponent<Interactable>().Interact(transform);
                            break;
                        case "Food":
                            GameObject targetFood = hit.collider.gameObject;
                            targetFood.GetComponent<Interactable>().Interact();
                            break;
                    }
                }
            }
        }

        public bool interactableInRange(GameObject interactableObj) {
            Ray ray = attackCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange)) {
                if (hit.transform.gameObject == interactableObj) {
                    return true;
                }
            }
            return false;
        }

        public void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound) {
            Ray ray = attackCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            attackSound.Play();
            if (Physics.Raycast(ray, out hit, range, playerLayers)) {
                if (hit.collider.tag == "Enemy") {
                    hurtSound.Play();
                    enemy = hit.collider.gameObject;
                    eMovement = enemy.GetComponent<Enemies.Movement>();
                    eRb = enemy.GetComponent<Rigidbody>();
                    ParticleSystem blood = Instantiate(SceneController.Instance.bloodParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    blood.Play();
                    Destroy(blood.gameObject, 0.5f);
                    eMovement.TakeDamage(damage);
                    Vector3 eDirection = enemy.transform.position - transform.position;
                    eDirection.y = (float)(Math.Sin(-xRotation * Math.PI/180) * knockback);
                    eRb.AddForce(eDirection.normalized * knockback, ForceMode.Impulse);
                }else {
                    ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    ground.Play();
                    Destroy(ground.gameObject, 0.5f);
                }
            }
        }

        private void ControlDrag() {
            if (isGrounded) {
                rb.drag = 6;
            }else {
                rb.drag = 1;
            }
        }

        private bool OnSlope() {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, out slopeHit, 0.75f)) {
                if (slopeHit.normal != Vector3.up) {
                    return true;
                }else {
                    return false;
                }
            }
            return false;
        }

        private void MovePlayer() {
            if (isGrounded) {
                if (OnSlope()) {
                    rb.useGravity = false;
                }else if (!OnSlope() && !rb.useGravity){
                    rb.useGravity = true;
                }
                rb.AddForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Acceleration);
            }else if (!isGrounded) {
                rb.AddForce(moveDirection.normalized * moveSpeed * 0.4f, ForceMode.Acceleration);
                if (!rb.useGravity) {
                    rb.useGravity = true;
                }
            }
        }

        public void SwitchWeapon(int selectedWeapon) {
            if (selectedWeapon > hand.transform.childCount - 1) {
                selectedWeapon = 0;
            }else if (selectedWeapon < 0) {
                selectedWeapon = hand.transform.childCount - 1;
            }
            
            int i = 0;
            foreach (Transform weapon in hand.transform) {
                if (i == selectedWeapon) {
                    weapon.gameObject.SetActive(true);
                    myWeapon = weapon.gameObject;
                }else {
                    weapon.gameObject.SetActive(false);
                }
                i++; 
            }
            myWeaponStats = myWeapon.GetComponent<Weapons>();

            if (myWeaponStats.isGun) {
                bulletsTextBox.text = ("BULLETS x" + myWeaponStats.bullets);
                if (myWeaponStats.bullets > 0) {
                    weaponOverrideController["Attack"] = myWeaponStats.fpShootAnimation;
                }else {
                    weaponOverrideController["Attack"] = myWeaponStats.fpAttackAnimation; 
                }           
            }else {
                bulletsTextBox.text = ("");
                weaponOverrideController["Attack"] = myWeaponStats.fpAttackAnimation;
            }
            weaponOverrideController["Block"] = myWeaponStats.fpBlockAnimation;
        }

        public void AlertEveryone() {
            Enemies.Movement[] list = SceneController.Instance.listOfEnemies;
            for (int i = 0; i < list.Length; i++) {
                if (list[i] != null && list[i].agent.enabled) {
                    list[i].agent.SetDestination(transform.position);
                }
            }
        }

        public void AlertRadius(float radius) {
            Collider[] list = Physics.OverlapSphere(transform.position, radius, enemyMask);

            for (int i = 0; i < list.Length; i++) {
                Enemies.Movement enemyScript = list[i].GetComponent<Enemies.Movement>();
                if (enemyScript != null && enemyScript.agent.enabled && !enemyScript.alreadyAttacked) {
                    enemyScript.agent.SetDestination(transform.position);
                }
            } 
        }

        public void SavePlayer() {
            SaveSystem.SavePlayer(this);
        }

        public void LoadPlayer() {
            PlayerData data = SaveSystem.LoadPlayer();
            walkSpeed = data.walkSpeed;
            sprintSpeed = data.sprintSpeed;
            jumpForce = data.jumpForce;
            playerDrugs = data.playerDrugs;
            pickUpRange = data.pickUpRange;
            GameObject cloneWeapon = (GameObject)Resources.Load(data.myWeapon, typeof(GameObject));
            if (!cloneWeapon.GetComponent<Weapons>().isDefaultItem) {
                savedWeapon = Instantiate(cloneWeapon, this.transform.position, transform.rotation) as GameObject; 
            }
        }

        /*void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, 10f);
            Gizmos.DrawRay(attackCam.ScreenPointToRay(Input.mousePosition));
        }*/
    }
}