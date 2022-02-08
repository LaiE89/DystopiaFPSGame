using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : Destructable {
    [SerializeField] float radius;
    [SerializeField] float damage;
    [SerializeField] float alertRadius;
    [SerializeField] float igniteTime;
    [SerializeField] int numberOfTicks;
    [SerializeField] Rigidbody rb;
    bool isIgnited;
    bool isExploded;

    public void Awake() {
        StartCoroutine(igniteDelay());
    }

    IEnumerator igniteDelay() {
        isIgnited = false;
        yield return new WaitForSeconds(igniteTime);
        isIgnited = true;

    }

    public void OnCollisionEnter() {
        if (isIgnited) {
            Interact();
            Destroy(gameObject);
        }
    }

    public override void Interact() {
        if (!isExploded) {
            isExploded = true;
            ToolMethods.AlertRadius(alertRadius, transform.position, LayerMask.NameToLayer("Enemy"));
            base.Interact();
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider nearbyObject in colliders) {
                Vector3 directionToTarget = (ToolMethods.OffsetPosition(nearbyObject.gameObject.transform.position, 0, 0.2f, 0) - ToolMethods.OffsetPosition(transform.position, 0, 0.2f, 0)).normalized;
                float distanceToTarget = Vector3.Distance(ToolMethods.OffsetPosition(transform.position, 0, 0.2f, 0), ToolMethods.OffsetPosition(nearbyObject.gameObject.transform.position, 0, 0.2f, 0));
                if (!Physics.Raycast(ToolMethods.OffsetPosition(transform.position, 0, 0.2f, 0), directionToTarget, distanceToTarget, groundMask)) {
                    Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                    if (rb != null) {
                        Enemies.Movement eMovement = nearbyObject.GetComponent<Enemies.Movement>();
                        Player.PlayerMovement pMovement = nearbyObject.GetComponent<Player.PlayerMovement>();
                        if (eMovement != null) {
                            eMovement.StartCoroutine(eMovement.TakeFireDamage(numberOfTicks));
                        }else if (pMovement != null) {
                            pMovement.StartCoroutine(pMovement.TakeFireDamage(numberOfTicks));
                        }
                    }
                    Destructable destructable = nearbyObject.GetComponent<Destructable>();
                    if (destructable != null && destructable != this) {
                        destructable.Interact();
                    }
                }
            }
        }
    }
}

