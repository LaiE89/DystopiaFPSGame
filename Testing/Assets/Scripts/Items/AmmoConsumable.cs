using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class AmmoConsumable : Interactable {
    
    public override void Interact() {
        Player.PlayerMovement playerInstance = SceneController.Instance.player;
        if (playerInstance.myWeaponStats.isGun) {
            base.Interact();
            playerInstance.myWeaponStats.bullets = playerInstance.myWeaponStats.maxBullets;
            playerInstance.weaponOverrideController["Attack"] = playerInstance.myWeaponStats.fpShootAnimation;
            playerInstance.bulletsTextBox.text = ("BULLETS x" + playerInstance.myWeaponStats.bullets);
            playerInstance.interactTextBox.text = ("");
            Destroy(gameObject);
        }
    }
}