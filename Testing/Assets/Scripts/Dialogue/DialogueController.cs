using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour {

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public string textSound;
    public Animator animator;
    public bool isPlaying;
    private Queue<string> sentences;

    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        isPlaying = true;
        sentences = new Queue<string>();
        animator.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        if (dialogue.isCenter) {
            dialogueText.alignment = TextAlignmentOptions.Center;
            dialogueText.alignment = TextAlignmentOptions.Top;
        }else {
            dialogueText.alignment = TextAlignmentOptions.TopLeft;
        }
        sentences.Clear();

        foreach(string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence(dialogue);
    }

    public void DisplayNextSentence(Dialogue dialogue) {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, dialogue));
    }

    IEnumerator TypeSentence (string sentence, Dialogue dialogue) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            if (dialogue.audioSource != null) {
                dialogue.audioSource.PlayOneShot(dialogue.audioSource.clip);
            }else {
                if (textSound != null) {
                    SceneController.Instance.soundController.PlayOneShot(textSound);
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f);
        DisplayNextSentence(dialogue);
    }

    void EndDialogue() {
        isPlaying = false;
        animator.SetBool("isOpen", false);
    }
}
