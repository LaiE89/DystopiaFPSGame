using UnityEngine;

public abstract class Trigger : MonoBehaviour {
    [SerializeField] bool onStart;

    void OnTriggerEnter(Collider other) {
        if (!onStart && other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            result();
        }
    }

    void Start() {
        if (onStart) {
            result();
        }
    }
    
    public abstract void result();
}