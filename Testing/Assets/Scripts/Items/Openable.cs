using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class Openable : Interactable {

    [SerializeField] LayerMask enemyMask;
    [SerializeField] Animator animator;
    [SerializeField] Consumable key;
    [SerializeField] NavMeshObstacle obstacle;
    bool canUse;
    bool isOpen;
    string originalItemName;

    public void Start() {
        originalItemName = this.itemName;
        canUse = true;
        isOpen = false;
    }
    
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == ToolMethods.LayerMaskToLayer(enemyMask)) {
            Interact();
        }
    }

    public override void Interact(Transform user) {
        if (canUse) {
            base.Interact();
            if (isOpen) {
                isOpen = false;  
                animator.SetBool("isOpen", false);  
                ToolMethods.AlertRadius(5f, user.position, enemyMask);
            }else {
                if (key == null) {
                    if (obstacle.isActiveAndEnabled) {
                        this.itemName = originalItemName;
                        obstacle.enabled = false;
                    }
                    isOpen = true;
                    animator.SetBool("isOpen", true);  
                    ToolMethods.AlertRadius(5f, user.position, enemyMask);
                }else {
                    this.itemName = "Requires " + key.itemName;
                }
            }
            this.canUse = false;
            StartCoroutine(useDelay());
        }
    }

    public override void Interact() {
        if (!isOpen) {
            base.Interact();
            isOpen = true;
            animator.SetBool("isOpen", true);  
        }
    }

    private IEnumerator useDelay() {
        yield return new WaitForSeconds(0.5f);
        this.canUse = true;
    }
}