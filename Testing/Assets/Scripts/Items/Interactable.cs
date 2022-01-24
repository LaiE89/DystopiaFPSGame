using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour {

    [SerializeField] string itemName;

    public virtual void Interact(){

    }
    
    public virtual void Interact(Transform i){

    }

    public virtual void OnMouseOver() {
        Player.PlayerMovement player = SceneController.Instance.player;
        if (transform.parent == null && !ingameMenus.pausedGame && player.interactableInRange(this.gameObject)){
            player.interactTextBox.text = (itemName.ToUpper());
        }else {
            player.interactTextBox.text = ("");
        }
    }

    public virtual void OnMouseExit() {
        Player.PlayerMovement player = SceneController.Instance.player;
        if (transform.parent == null && !ingameMenus.pausedGame) {
            player.interactTextBox.text = ("");
        }
    }
}