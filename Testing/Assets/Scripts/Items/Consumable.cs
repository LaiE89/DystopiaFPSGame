using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Consumable : Interactable {
    
    public override void Interact() {
        base.Interact();
        Player.PlayerMovement playerInstance = SceneController.Instance.player;
        playerInstance.interactTextBox.text = ("");
        Destroy(gameObject);
    }
}