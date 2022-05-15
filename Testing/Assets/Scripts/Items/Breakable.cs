using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : Destructable {
    [SerializeField] float radius;
    [SerializeField] public float damage;
    [SerializeField] float horizontalForce;
    [SerializeField] float verticalForce;
    [SerializeField] float alertRadius;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask enemyMask;
    bool isBreaking;
    bool isBroken;

    public void Awake() {
        StartCoroutine(breakDelay());
    }

    IEnumerator breakDelay() {
        isBreaking = false;
        yield return new WaitForEndOfFrame();
        isBreaking = true;

    }

    public void OnCollisionEnter() {
        if (isBreaking) {
            Interact();
            Destroy(gameObject);
        }
    }

    public override void Interact() {
        if (!isBroken) {
            isBroken = true;
            if (alertRadius > 0) {
                ToolMethods.AlertRadius(alertRadius, gameObject.transform.position, gameObject.transform.position, enemyMask);
            }
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
                            eMovement.TakeDamage(damage);
                        }else if (pMovement != null) {
                            pMovement.TakeDamage(damage);
                        }
                        rb.AddExplosionForce(horizontalForce, transform.position, radius, verticalForce, ForceMode.Impulse);
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

