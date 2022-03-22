using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class Climbable : Interactable {
    
    [SerializeField] Vector3 topDestination;
    [SerializeField] Vector3 baseDestination;
    [SerializeField] LayerMask enemyMask;
    bool canUse;

    public void Start() {
        canUse = true;
    }
    
    public override void Interact(Transform user) {
        if (canUse) {
            base.Interact();
            if (Vector3.Distance(user.position, topDestination) > Vector3.Distance(user.position, baseDestination)) {
                user.position = topDestination;  
                ToolMethods.AlertRadius(5f, user.position, enemyMask);
            }else {
                user.position = baseDestination;
                ToolMethods.AlertRadius(5f, user.position, enemyMask);
            }
            this.canUse = false;
            StartCoroutine(useDelay());
        }
    }

    private IEnumerator useDelay() {
        yield return new WaitForSeconds(1f);
        this.canUse = true;
    }
}