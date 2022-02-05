using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Climbable : Interactable {
    
    [SerializeField] Vector3 destination;
    
    public override void Interact(Transform user) {
        base.Interact();
        user.position =  destination;      
    }
}