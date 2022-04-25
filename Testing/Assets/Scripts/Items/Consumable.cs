using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Consumable : Interactable {
    
    public override void Interact() {
        base.Interact();
        Destroy(gameObject);
    }
}