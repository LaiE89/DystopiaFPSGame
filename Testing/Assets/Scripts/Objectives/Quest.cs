using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : Trigger {
    [SerializeField] DialogueTrigger tutorialTrigger;
    [SerializeField] GameObject objectiveBlock;

    public override void result() {
        if (!tutorialTrigger.enabled) {
            tutorialTrigger.enabled = true;
            Destroy(gameObject);
            if (objectiveBlock != null) {
                objectiveBlock.SetActive(false);
            }
        }
    }
}
