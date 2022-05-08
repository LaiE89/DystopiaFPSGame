using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DrugConsumable : Consumable {
    
    public override void Interact() {
        base.Interact();
        Player.PlayerMovement playerInstance = SceneController.Instance.player;
        playerInstance.playerDrugs += 1;
        playerInstance.drugsTextBox.text = ("DRUGS x " + playerInstance.playerDrugs);
        // playerInstance.interactTextBox.text = ("");
        // Destroy(gameObject);
    }
}