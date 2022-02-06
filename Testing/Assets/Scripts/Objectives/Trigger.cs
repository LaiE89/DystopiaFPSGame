using UnityEngine;

public abstract class Trigger : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            result();
        }
    }

    public abstract void result();
}