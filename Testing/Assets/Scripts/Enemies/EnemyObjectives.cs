using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Enemies {
    public class EnemyObjectives : MonoBehaviour {
        public Dialogue[] dialogues;
        [SerializeField] GameObject objectiveBlock;
        [SerializeField] Movement enemyScript;
        Dialogue hurt;
        Dialogue attack;
        Dialogue target;
        Dialogue death;
        Dialogue mouse;

        void Start() {
            foreach (Dialogue dialogue in dialogues) {
                if (dialogue.onAttack) {
                    attack = dialogue;
                }
                if(dialogue.onDeath) {
                    death = dialogue;
                }
                if (dialogue.onTarget) {
                    target = dialogue;
                }
                if (dialogue.onMouse) {
                    mouse = dialogue;
                }
                if (dialogue.onHurt) {
                    hurt = dialogue;
                }
            }
            StartCoroutine(checkDialogueRoutine());
        }

        private IEnumerator checkDialogueRoutine(){
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true) {
                yield return wait;
                checkDialogue();
            }
        }

        void checkDialogue() {
            if (target != null) {
                if (enemyScript.targetLocked) {
                    if (!target.isTriggered) {
                        target.isTriggered = true;
                        if (target.isObjective) {
                            SceneController.Instance.objectivesController.StartDialogue(target);
                        }else {
                            SceneController.Instance.dialogueController.StartDialogue(target);
                        }
                    }
                }
            }
            if (death != null) {
                if (enemyScript.isDying) {
                    if (!death.isTriggered) {
                        death.isTriggered = true;
                        if (death.isObjective) {
                            SceneController.Instance.objectivesController.StartDialogue(death);
                        }else {
                            SceneController.Instance.dialogueController.StartDialogue(death);
                        }
                        if (objectiveBlock != null) {
                            objectiveBlock.SetActive(false);
                        }
                    }
                }
            }
            if (attack != null) {
                if (enemyScript.alreadyAttacked) {
                    if (!attack.isTriggered) {
                        attack.isTriggered = true;
                        if (attack.isObjective) {
                            SceneController.Instance.objectivesController.StartDialogue(attack);
                        }else {
                            SceneController.Instance.dialogueController.StartDialogue(attack);
                        }
                    }
                }
            }
            if (hurt != null) {
                if (enemyScript.isKnockedBack) {
                    if (!hurt.isTriggered) {
                        hurt.isTriggered = true;
                        if (hurt.isObjective) {
                            SceneController.Instance.objectivesController.StartDialogue(hurt);
                        }else {
                            SceneController.Instance.dialogueController.StartDialogue(hurt);
                        }
                    }
                }
            }
        }

        public virtual void OnMouseOver() {
            if (mouse != null) {
                Player.PlayerMovement player = SceneController.Instance.player;
                if (!mouse.isTriggered && player.interactableInRange(this.gameObject)) {
                    mouse.isTriggered = true;
                    if (mouse.isObjective) {
                        SceneController.Instance.objectivesController.StartDialogue(mouse);
                    }else {
                        SceneController.Instance.dialogueController.StartDialogue(mouse);
                    }
                }
            }
        }
    }
}
