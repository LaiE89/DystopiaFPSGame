using UnityEngine;

public abstract class Trigger : MonoBehaviour {

    void OnTriggerEnter() {
        result();
    }

    public abstract void result();
}