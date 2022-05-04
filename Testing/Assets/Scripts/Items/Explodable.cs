using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : Destructable {
    [SerializeField] float radius;
    [SerializeField] float horizontalForce;
    [SerializeField] float verticalForce;
    [SerializeField] float damage;
    [SerializeField] int numberOfTicks;
    [SerializeField] float alertRadius;
    bool isExploded;

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
                    if (rb != null && rb != this.GetComponent<Rigidbody>()) {
                        Enemies.Movement eMovement = nearbyObject.GetComponent<Enemies.Movement>();
                        Player.PlayerMovement pMovement = nearbyObject.GetComponent<Player.PlayerMovement>();
                        if (eMovement != null) {
                            eMovement.TakeDamage(damage);
                            eMovement.StartCoroutine(eMovement.TakeFireDamage(numberOfTicks));
                        }else if (pMovement != null) {
                            pMovement.TakeDamage(damage);
                            pMovement.StartCoroutine(pMovement.TakeFireDamage(numberOfTicks));
                        }
                        rb.AddExplosionForce(horizontalForce, transform.position, radius, verticalForce, ForceMode.Impulse);
                    }
                    Destructable destructable = nearbyObject.GetComponent<Destructable>();
                    if (destructable != null && destructable != this) {
                        destructable.Interact();
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
