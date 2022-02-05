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
        if (dialogue.isObjective) {
            SceneController.Instance.objectivesController.StartDialogue(dialogue);
        }else {
            SceneController.Instance.dialogueController.StartDialogue(dialogue);
        }
        //dialogueController.StartDialogue(dialogue);
        //FindObjectOfType<DialogueController>().StartDialogue(dialogue);
    }
}
