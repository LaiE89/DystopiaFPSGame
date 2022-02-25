using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AmmoConsumable : Interactable {
    
    public override void Interact() {
        Player.PlayerMovement playerInstance = SceneController.Instance.player;
        base.Interact();
        playerInstance.playerAmmo += 1;
        playerInstance.ammoTextBox.text = ("AMMO x " + playerInstance.playerAmmo);
        playerInstance.interactTextBox.text = ("");
        Destroy(gameObject);
    }
}