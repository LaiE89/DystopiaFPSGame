using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] KeyCode skill1Key = KeyCode.Alpha1;
        [SerializeField] KeyCode skill2Key = KeyCode.Alpha2;
        [SerializeField] KeyCode skill3Key = KeyCode.Alpha3;

        [Header("Mouse Movement")]
        [SerializeField] float startingYRotation;
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
        [SerializeField] public LayerMask enemyMask;
        [SerializeField] Transform groundCheck;
        [HideInInspector] public bool isGrounded;
        float groundDistance = 0.2f;
        RaycastHit slopeHit;

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

        [Header("Health")]
        [SerializeField] public float playerHealth;
        [SerializeField] public float maxPlayerHealth;

        [Header("Stamina")]
        [SerializeField] float maxPlayerStamina = 100;
        [SerializeField] float timeBeforeStamina;
        [HideInInspector] public float playerStamina;
        Coroutine isRegenStamina;
        
        [Header("Hunger")]
        [SerializeField] public float maxPlayerHunger = 100;
        [SerializeField] public float playerHunger;

        [Header("Jumping")]
        [SerializeField] public float jumpForce;

        [Header("Attacking")]
        [SerializeField] Material injuredArmMaterial;
        [SerializeField] Material healedArmMaterial;
        [SerializeField] GameObject boneWeapon;
        [SerializeField] public GameObject myWeapon;
        [SerializeField] public SkillsObject[] skills;
        [HideInInspector] public AnimatorOverrideController weaponOverrideController;
        [HideInInspector] public bool isBlocking;
        [HideInInspector] public bool isParrying;
        [HideInInspector] public Weapons myWeaponStats;
        [HideInInspector] public Animator weaponAnimator;
        LayerMask playerLayers;
        SkinnedMeshRenderer armMeshRenderer;
        Weapons defaultWeaponStats;
        float attackTimer;
        bool isAttacking;
        GameObject savedWeapon;

        [Header("Consume")]
        [SerializeField] public int playerDrugs;
        [HideInInspector] public bool isConsuming;

        [Header("Interaction")]
        [SerializeField] public int pickUpRange;
        [HideInInspector] public int selectedWeapon;

        [Header("Status")]
        [SerializeField] public List<string> statusEffects;
        [HideInInspector] public float damageMultiplier;
        [HideInInspector] public float attackSpeedMultiplier;
        [HideInInspector] public float staminaUsageMultiplier;

        [Header("Gameobject Initialization")]
        [SerializeField] public TextMeshProUGUI interactTextBox;
        [SerializeField] public TextMeshProUGUI drugsTextBox;
        [SerializeField] public TextMeshProUGUI bulletsTextBox;
        [SerializeField] public Slider holdInteractSlider;
        [SerializeField] public Slider hungerSlider;
        [SerializeField] public Slider staminaSlider;
        [SerializeField] public Slider healthSlider;
        [SerializeField] public Image hurtScreen;
        public struct SkillIcon {
            public Image skillImage;
            public Image cooldownImage;
            public KeyCode skillKey;
        }
        [SerializeField] public ingameMenus canvas;
        List<SkillIcon> listOfSkillIcons;

        [SerializeField] Transform mouseCam;
        [SerializeField] public Camera attackCam;
        [SerializeField] public GameObject hand;
        [SerializeField] GameObject firstPersonView;

        [HideInInspector] public SoundController soundController;
        [HideInInspector] public int sceneIndex;
        [HideInInspector] public Rigidbody rb;
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
            
            // Initializing Gameobjects
            listOfSkillIcons = new List<SkillIcon>();
            canvas = SceneController.Instance.canvas.GetComponent<ingameMenus>();
            foreach (Transform child in canvas.inGameUI.transform) {
                switch (child.name) {
                    case "Hurt Image":
                        hurtScreen = child.GetComponent<Image>();
                        break;
                    case "Health Bar":
                        healthSlider = child.GetComponent<Slider>();
                        break;
                    case "Stamina Bar":
                        staminaSlider = child.GetComponent<Slider>();
                        break;
                    case "Hunger Bar":
                        hungerSlider = child.GetComponent<Slider>();
                        break;
                    case "Holding Bar":
                        holdInteractSlider = child.GetComponent<Slider>();
                        break;
                    case "Drugs Text":
                        drugsTextBox = child.GetComponent<TextMeshProUGUI>();
                        break;
                    case "Bullets Text":
                        bulletsTextBox = child.GetComponent<TextMeshProUGUI>();
                        break;
                    case "Interact Text":
                        interactTextBox = child.GetComponent<TextMeshProUGUI>();
                        break;
                    case "Skill 1":
                        SkillIcon skill1 = new SkillIcon();
                        skill1.skillImage = child.GetComponent<Image>();
                        skill1.cooldownImage = child.GetChild(0).GetComponent<Image>();
                        //skill1.skillImage.GetComponentInChildren<Image>();
                        skill1.skillKey = skill1Key;
                        listOfSkillIcons.Add(skill1);
                        break;
                    default:
                        break;
                }
            }

            for (int i = 0; i < skills.Length; i++) {
                if (i == 0) {
                    this.skills[i] = skills[i].CreateInstance(0.5f);
                    this.skills[i].skillNumber = 1;
                    Debug.Log("Skill Number: " + this.skills[i].skillNumber);
                    listOfSkillIcons[0].skillImage.gameObject.SetActive(true);
                    listOfSkillIcons[0].cooldownImage.gameObject.SetActive(true);
                    listOfSkillIcons[0].cooldownImage.fillAmount = 1;
                }
            }
            if (skills.Length > 0) {
                StartCoroutine(SkillCheckRoutine());
            }
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

            healthSlider.maxValue = maxPlayerHealth;
            playerHealth = maxPlayerHealth;
            healthSlider.value = playerHealth;

            staminaSlider.maxValue = maxPlayerStamina;
            playerStamina = maxPlayerStamina;
            staminaSlider.value = playerStamina;

            hungerSlider.maxValue = maxPlayerHunger;
            hungerSlider.value = playerHunger;

            drugsTextBox.text = ("DRUGS x " + playerDrugs);

            playerLayers = groundMask | enemyMask;

            UpdatingStatus(this.statusEffects);

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

///// HEALTH AND STATUSES /////
        public void TakeDamage(float amount) {
            playerHealth -= amount;
            healthSlider.value = playerHealth;
            CameraShaker.Instance.ShakeOnce(amount*2, amount, 0.1f, 0.5f);
            if (playerHealth <= 0) {
                StartCoroutine(DeathDelay());
            }
            StartCoroutine(FadeBlood(true));
        }

        IEnumerator DeathDelay() {
            yield return new WaitForSeconds(0.1f);
            SceneController.Instance.soundController.StopAll();
            canvas.ToggleDeathScreen();
        }

        IEnumerator FadeBlood(bool fadeAway) {
            if (fadeAway) {
                for (float i = 1; i >= 0; i -= Time.deltaTime) {
                    hurtScreen.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }else {
                for (float i = 0; i <= 1; i += Time.deltaTime) {
                    hurtScreen.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
        }

        public void UpdatingStatus(List<string> statusList) {
            damageMultiplier = 1;
            attackSpeedMultiplier = 1;
            staminaUsageMultiplier = 1;
            if (statusList.Count > 0) {
                foreach (string status in statusList) {
                    switch (status) {
                        case "isInjured":
                            damageMultiplier *= 0.5f;
                            attackSpeedMultiplier *= 1.5f;
                            armMeshRenderer.material = injuredArmMaterial;
                            if(defaultWeaponStats.weaponHealth > 0) {
                                defaultWeaponStats.weaponHealth = 0;
                            }
                            break;
                        case "isHungry":
                            attackSpeedMultiplier *= 1.5f;
                            staminaUsageMultiplier *= 1.5f;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void UsingStamina(float amount) {
            playerStamina -= amount * staminaUsageMultiplier;
            playerHunger -= amount * staminaUsageMultiplier * 0.15f;
            if (playerStamina < 0) {
                playerStamina = 0;
            }
            if (playerHunger <= 0) {
                if (playerHunger < 0) {
                    playerHunger = 0;
                }
                if (!statusEffects.Contains("isHungry")){
                    statusEffects.Add("isHungry");
                    UpdatingStatus(statusEffects);
                }
            }
            hungerSlider.value = playerHunger;
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

///// COMBAT /////
        private IEnumerator SkillCheckRoutine(){
            //WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true) {
                yield return null;
                for (int i = 0; i < skills.Length; i++) {
                    listOfSkillIcons[skills[i].skillNumber - 1].cooldownImage.fillAmount = 1 - (Time.time - skills[i].useTime) / skills[i].cooldown;
                    if (Input.GetKeyDown(listOfSkillIcons[skills[i].skillNumber - 1].skillKey) && skills[i].CanUseSkill(gameObject)) {
                        skills[i].UseSkill(gameObject);
                        listOfSkillIcons[skills[i].skillNumber - 1].cooldownImage.fillAmount = 1;
                    }
                }
            }
        }

        private void Attack() {
            attackTimer += Time.deltaTime;
            if (Input.GetMouseButton(0) && !isConsuming) {
                if (myWeaponStats.isGun && myWeaponStats.bullets > 0) {
                    if (attackTimer >= myWeaponStats.shootCooldown * attackSpeedMultiplier) {
                        isAttacking = true;
                        attackTimer = 0f;
                        weaponAnimator.SetFloat("AttackMultiplier", 1 / (myWeaponStats.shootCooldown * attackSpeedMultiplier));
                        weaponAnimator.SetTrigger("isAttacking");
                    }
                }else {
                    if (attackTimer >= myWeaponStats.attackCooldown  * attackSpeedMultiplier) {
                        if (playerStamina >= myWeaponStats.staminaCost) {
                            UsingStamina(myWeaponStats.staminaCost);
                            isAttacking = true;
                            attackTimer = 0f;
                            weaponAnimator.SetFloat("AttackMultiplier", 1 / (myWeaponStats.attackCooldown * attackSpeedMultiplier));
                            weaponAnimator.SetTrigger("isAttacking");
                        }
                    }
                }
            }
            if (attackTimer >= myWeaponStats.attackCooldown) {
                isAttacking = false;
            }
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
                    ParticleSystem blood = Instantiate(SceneController.Instance.bloodParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    blood.Play();
                    Destroy(blood.gameObject, 0.5f);
                    DealDamage(eMovement, enemy, damage, knockback);
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

        public void DealDamage(Enemies.Movement enemyScript, GameObject enemyObject, float damage, float knockback) {
            enemyScript.TakeDamage(damage);
            Vector3 eDirection = enemyObject.transform.position - transform.position;
            eDirection.y = (float)(Math.Sin(-xRotation * Math.PI/180) * knockback);
            enemyScript.rb.AddForce(eDirection.normalized * knockback, ForceMode.Impulse);
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
                    soundController.PlayOneShot("Item Break");
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
                    //isInjured = true;
                    statusEffects.Add("isInjured");
                    UpdatingStatus(this.statusEffects);
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

///// ITEMS /////
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
            if (playerHealth < maxPlayerHealth) {
                playerHealth += 1;
                healthSlider.value = playerHealth;
            }
            if (statusEffects.Contains("isInjured")) {
                statusEffects.Remove("isInjured");
                UpdatingStatus(this.statusEffects);
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
                        case "Consumable" :
                            GameObject targetConsumable = hit.collider.gameObject;
                            targetConsumable.GetComponent<Interactable>().Interact();
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

///// MOVEMENT /////
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
            //mouseCam.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            mouseCam.rotation = Quaternion.Lerp(mouseCam.rotation, Quaternion.Euler(xRotation, yRotation, 0), Time.deltaTime * 50);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        private void Jump() {
            if (Input.GetKeyDown(jumpKey) && isGrounded && !isConsuming) {
                if (playerStamina >= 10f) {
                    soundController.Stop("PlayerRun", 0.25f);
                    soundController.PlayOneShot("PlayerJump");
                    UsingStamina(10f);
                    rb.velocity = ToolMethods.SettingVector(rb.velocity.x, 0, rb.velocity.z);
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

        private void ControlDrag() {
            if (isGrounded) {
                rb.drag = 6;
            }else {
                rb.drag = 1;
            }
        }
    

        private bool OnSlope() {
            //if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, out slopeHit, 0.75f)) {
            if (Physics.Raycast(ToolMethods.OffsetPosition(transform.position, 0, 0.5f, 0), Vector3.down, out slopeHit, 0.75f)) {
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

///// SETTINGS /////
        public static void SettingChanges() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            newSens = OptionsMenu.sens;
            sensX = newSens;
            sensY = newSens;
        }

        public void SavePlayer() {
            SaveSystem.SavePlayer(this);
        }

        public void LoadPlayer() {
            PlayerData data = SaveSystem.LoadPlayer();
            playerHunger = data.playerHunger;
            walkSpeed = data.walkSpeed;
            sprintSpeed = data.sprintSpeed;
            jumpForce = data.jumpForce;
            playerDrugs = data.playerDrugs;
            pickUpRange = data.pickUpRange;
            statusEffects = data.statusEffects;
            GameObject cloneWeapon = (GameObject)Resources.Load(data.myWeapon, typeof(GameObject));
            if (!cloneWeapon.GetComponent<Weapons>().isDefaultItem) {
                savedWeapon = Instantiate(cloneWeapon, this.transform.position, transform.rotation) as GameObject; 
            }
        }
    }
}