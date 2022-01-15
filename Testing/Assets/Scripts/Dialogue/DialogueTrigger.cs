using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public Dialogue dialogue;
    
    [Header("Trigger methods")]
    public bool whenEnabled;

    void OnEnable() {
        if (whenEnabled) {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue() {
        FindObjectOfType<DialogueController>().StartDialogue(dialogue);
    }
}
