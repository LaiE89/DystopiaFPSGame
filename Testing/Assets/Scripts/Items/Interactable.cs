using UnityEngine;
using System.Collections;

public abstract class Interactable : MonoBehaviour {

    [SerializeField] string itemName;

    public virtual void Interact(){

    }
    
    public virtual void Interact(Transform i){

    }

    void OnMouseOver() {
        if (transform.parent == null && !ingameMenus.pausedGame) {
            //Player.PlayerMovement.interactBox.text = (itemName.ToUpper());
            SceneController.Instance.player.interactTextBox.text = (itemName.ToUpper());
        }
    }

    void OnMouseExit() {
       if (transform.parent == null && !ingameMenus.pausedGame) {
            SceneController.Instance.player.interactTextBox.text = ("");
            //Player.PlayerMovement.interactBox.text = ("");
        }
    }
}