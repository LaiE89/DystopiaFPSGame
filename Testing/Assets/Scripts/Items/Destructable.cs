using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

    [SerializeField] string interactSound;
    [SerializeField] ParticleSystem interactParticle;
    [SerializeField] public LayerMask groundMask;

    public virtual void Interact(){
        if (interactSound != null) {
            SceneController.Instance.soundController.PlayClipAtPoint(interactSound, transform.position);
        }
        if (interactParticle != null) {
            ParticleSystem particle =  Instantiate(interactParticle, transform.position, ToolMethods.SettingQuaternion(0, 0, 0, 0));
            particle.Play();
        }
    }

}
