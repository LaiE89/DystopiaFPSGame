using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueController : MonoBehaviour {

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;
    
    private Queue<string> sentences;

    void Start() {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue) {
        sentences = new Queue<string>();
        animator.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        if (dialogue.isCenter) {
            dialogueText.alignment = TextAlignmentOptions.Center;
            dialogueText.alignment = TextAlignmentOptions.Top;
        }else {
            dialogueText.alignment = TextAlignmentOptions.Left;
        }
        sentences.Clear();

        foreach(string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f);
        DisplayNextSentence();
    }

    void EndDialogue() {
        animator.SetBool("isOpen", false);
    }
}
