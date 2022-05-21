using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

    [SerializeField] string interactSound;
    [SerializeField] float soundPitch;
    [SerializeField] float soundVolume;
    [SerializeField] ParticleSystem interactParticle;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public Collider thisCollider;
    [SerializeField] public Rigidbody thisRb;
    [HideInInspector] public GameObject thrower;

    public virtual void Start() {
        // Debug.Log("Ignored Collision with " + thrower);
        if (thrower != null) {
            Collider throwerCollider = thrower.GetComponent<Collider>();
            if (throwerCollider != null) {
                Physics.IgnoreCollision(thisCollider, throwerCollider, true);
            }
        }
    }
    
    public virtual void Interact(){
        if (interactSound != null) {
            SceneController.Instance.soundController.PlayClipAtPoint(interactSound, transform.position, soundPitch, soundVolume);
        }
        if (interactParticle != null) {
            ParticleSystem particle =  Instantiate(interactParticle, transform.position, ToolMethods.SettingQuaternion(0, 0, 0, 0));
            particle.Play();
        }
    }

}
