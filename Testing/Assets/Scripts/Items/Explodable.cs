using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : Destructable {
    [SerializeField] float radius;
    [SerializeField] float horizontalForce;
    [SerializeField] float verticalForce;
    [SerializeField] float damage;
    [SerializeField] float alertRadius;
    bool isExploded;

    public override void Interact() {
        if (!isExploded) {
            isExploded = true;
            ToolMethods.AlertRadius(alertRadius, transform.position, LayerMask.NameToLayer("Enemy"));
            base.Interact();
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider nearbyObject in colliders) {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null && rb != this.GetComponent<Rigidbody>()) {
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
                if (destructable != null) {
                    destructable.Interact();
                }
            }
            Destroy(gameObject);
        }
    }
}
