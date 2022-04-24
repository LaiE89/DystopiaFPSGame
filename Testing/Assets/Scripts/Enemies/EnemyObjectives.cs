using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Enemies {
    public class EnemyObjectives : MonoBehaviour {
        public Dialogue[] dialogues;
        Boolean[] listOfConditions;
        [SerializeField] GameObject objectiveBlock;
        [SerializeField] Movement enemyScript;
        Dictionary<String, Queue<Dialogue>> dictOfDialogue;
        Queue<Dialogue> dialogueQueue;
        Queue<Dialogue> objectivesQueue;
        Dialogue prevDialogue;
        Dialogue prevObjective;

        void Start() {
            dialogueQueue = new Queue<Dialogue>();
            objectivesQueue = new Queue<Dialogue>();
            dictOfDialogue = new Dictionary<string, Queue<Dialogue>>();
            dictOfDialogue.Add("Attack", new Queue<Dialogue>());
            dictOfDialogue.Add("Death", new Queue<Dialogue>());
            dictOfDialogue.Add("Target", new Queue<Dialogue>());
            dictOfDialogue.Add("Mouse", new Queue<Dialogue>());
            dictOfDialogue.Add("Hurt", new Queue<Dialogue>());
            foreach (Dialogue dialogue in dialogues) {
                if (dialogue.onAttack) {
                    dictOfDialogue["Attack"].Enqueue(dialogue); 
                }
                if(dialogue.onDeath) {
                    dictOfDialogue["Death"].Enqueue(dialogue);
                }
                if (dialogue.onTarget) {
                    dictOfDialogue["Target"].Enqueue(dialogue);
                }
                if (dialogue.onMouse) {
                    dictOfDialogue["Mouse"].Enqueue(dialogue);
                }
                if (dialogue.onHurt) {
                    dictOfDialogue["Hurt"].Enqueue(dialogue);
                }
            }
            StartCoroutine(checkDialogueRoutine());
        }

        private IEnumerator checkDialogueRoutine(){
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true) {
                yield return wait;
                checkDialogue();
                if (dialogueQueue.Count != 0) {
                    if (prevDialogue != null && prevDialogue.isCancellable) {
                        prevDialogue = dialogueQueue.Dequeue();
                        SceneController.Instance.dialogueController.StartDialogue(prevDialogue);
                    }else {
                        if (!SceneController.Instance.dialogueController.isPlaying) {
                            prevDialogue = dialogueQueue.Dequeue();
                            SceneController.Instance.dialogueController.StartDialogue(prevDialogue);
                        }
                    }
                }
                if (objectivesQueue.Count != 0) {
                    if (prevObjective != null && prevObjective.isCancellable) {
                        prevObjective = objectivesQueue.Dequeue();
                        SceneController.Instance.objectivesController.StartDialogue(prevObjective);
                    }else {
                        if (!SceneController.Instance.objectivesController.isPlaying) {
                            prevObjective = objectivesQueue.Dequeue();
                            SceneController.Instance.objectivesController.StartDialogue(prevObjective);
                        }
                    }
                }
            }
        }

        void checkDialogue() {
            Dictionary<string, bool> currentConditions = new Dictionary<string, bool>{{"Target", enemyScript.targetLocked}, {"Death", enemyScript.isDying}, {"Attack", enemyScript.alreadyAttacked}, {"Hurt", enemyScript.isKnockedBack}};
            foreach (KeyValuePair<string, bool> condition in currentConditions) {
                if (condition.Value) {
                    if (dictOfDialogue[condition.Key].Count != 0) {
                        Dialogue curr = dictOfDialogue[condition.Key].Dequeue();
                        if (curr.isObjective) {
                            objectivesQueue.Enqueue(curr);
                        }else {
                            if (curr.onTarget || curr.onHurt) {
                                dictOfDialogue["Mouse"].Clear();
                            }
                            dialogueQueue.Enqueue(curr);
                        }
                        if (condition.Key == "Death" && objectiveBlock != null) {
                            objectiveBlock.SetActive(false);
                        }
                    }
                }
            }
        }

        public virtual void OnMouseOver() {
            if (this.isActiveAndEnabled && dictOfDialogue["Mouse"].Count != 0 && SceneController.Instance.player.interactableInRange(this.gameObject)) {
                Dialogue curr = dictOfDialogue["Mouse"].Dequeue();
                if (curr.isObjective) {
                    objectivesQueue.Enqueue(curr);
                }else {
                    dialogueQueue.Enqueue(curr);
                }
            }
        }
    }
}
