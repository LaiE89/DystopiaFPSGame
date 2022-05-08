using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class Activatable : Interactable {
    [SerializeField] LayerMask enemyMask;
    [SerializeField] Animator gateAnimator;
    [SerializeField] AudioSource gateOpen;
    [SerializeField] AudioSource gateClose;
    [SerializeField] ParticleSystem particles;
    bool canUse;
    bool isOpen;

    public void Start() {
        canUse = true;
    }

    public override void Interact(Transform user) {
        if (canUse) {
            base.Interact();
            if (isOpen) {
                isOpen = false;
                gateAnimator.SetBool("isOpen", false);
                gateClose.Play();
                particles.Play();
                ToolMethods.AlertRadius(5f, user.position, enemyMask);
            }else {
                isOpen = true;
                gateAnimator.SetBool("isOpen", true);  
                gateOpen.Play();
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
            gateAnimator.SetBool("isOpen", true);  
        }
    }

    private IEnumerator useDelay() {
        yield return new WaitForSeconds(2f);
        this.canUse = true;
    }
}