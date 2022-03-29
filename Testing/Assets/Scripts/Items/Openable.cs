using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class Openable : Interactable {

    [SerializeField] LayerMask enemyMask;
    [SerializeField] Animator animator;
    bool canUse;
    bool isOpen;

    public void Start() {
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
                isOpen = true;
                animator.SetBool("isOpen", true);  
                ToolMethods.AlertRadius(5f, user.position, enemyMask);
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