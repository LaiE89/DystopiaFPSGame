using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AmmoConsumable : Consumable {
    
    public override void Interact() {
        base.Interact();
        Player.PlayerMovement playerInstance = SceneController.Instance.player;
        playerInstance.playerAmmo += 1;
        playerInstance.ammoTextBox.text = ("AMMO x " + playerInstance.playerAmmo);
        playerInstance.interactTextBox.text = ("");
    }
}