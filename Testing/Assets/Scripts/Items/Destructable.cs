using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

    [SerializeField] string interactSound;
    [SerializeField] ParticleSystem interactParticle;

    public virtual void Interact(){
        if (interactSound != null) {
            SceneController.Instance.soundController.PlayClipAtPoint(interactSound, transform.position);
        }
        if (interactParticle != null) {
            ParticleSystem particle =  Instantiate(interactParticle, transform.position, transform.rotation);
            particle.Play();
            Destroy(particle, 3f);
        }
    }

}
