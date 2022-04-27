using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour {

    [SerializeField] public string itemName;
    [SerializeField] public string interactSound;

    public virtual void Interact(){
        if (interactSound != null) {
            // SceneController.Instance.soundController.PlayOneShot(interactSound);
            AudioSource source = SceneController.Instance.soundController.GetSound(interactSound);
            SceneController.Instance.soundController.PlayClipAtPoint(interactSound, gameObject.transform.position, source.pitch, source.volume);
        }
    }
    
    public virtual void Interact(Transform i){
        if (interactSound != null) {
            // SceneController.Instance.soundController.PlayOneShot(interactSound);
            AudioSource source = SceneController.Instance.soundController.GetSound(interactSound);
            SceneController.Instance.soundController.PlayClipAtPoint(interactSound, gameObject.transform.position, source.pitch, source.volume);
        }
    }

    public virtual void OnMouseOver() {
        Player.PlayerMovement player = SceneController.Instance.player;
        if ((gameObject.tag == "Appliance" || gameObject.tag == "Food" || transform.parent == null) && !ingameMenus.pausedGame && player.interactableInRange(this.gameObject)){
            player.interactTextBox.text = (itemName.ToUpper());
        }else {
            player.interactTextBox.text = ("");
        }
    }

    public virtual void OnMouseExit() {
        Player.PlayerMovement player = SceneController.Instance.player;
        if ((gameObject.tag == "Appliance" || gameObject.tag == "Food" || transform.parent == null) && !ingameMenus.pausedGame) {
            player.interactTextBox.text = ("");
        }
    }
}