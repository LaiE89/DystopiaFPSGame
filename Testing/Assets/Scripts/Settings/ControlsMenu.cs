using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;

public class ControlsMenu : MonoBehaviour {
    public static Dictionary<string, KeyCode> keybinds;
    Event keyEvent;
    bool waitingForKey;
    KeyCode newKey;
    SoundController soundController;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text blockText;
    [SerializeField] TMP_Text sprintText;
    [SerializeField] TMP_Text jumpText;
    [SerializeField] TMP_Text interactText;
    [SerializeField] TMP_Text reloadText;
    [SerializeField] TMP_Text consumeText;
    [SerializeField] TMP_Text skills1Text;
    [SerializeField] TMP_Text skills2Text;
    [SerializeField] TMP_Text skills3Text;

    public void Awake() {
        waitingForKey = false;
        if (SceneController.Instance) {
            soundController = SceneController.Instance.soundController;
        }else {
            soundController = MainMenu.soundController;
        }
    }

    public void Start() {
        FixingText();
    }

    void OnGUI() {
        if (waitingForKey) {
            keyEvent = Event.current;
            if (keyEvent.isKey || keyEvent.isMouse && waitingForKey) {
                if (keyEvent.isKey) {
                    newKey = keyEvent.keyCode;
                }else {
                    if (keyEvent.button == 0)
                        newKey = KeyCode.Mouse0;
                    else if (keyEvent.button == 1)
                        newKey = KeyCode.Mouse1;
                    else if (keyEvent.button == 2)
                        newKey = KeyCode.Mouse2;
                    else if (keyEvent.button == 3)
                        newKey = KeyCode.Mouse3;
                    else if (keyEvent.button == 4)
                        newKey = KeyCode.Mouse4;
                    else if (keyEvent.button == 5)
                        newKey = KeyCode.Mouse5;
                    else if (keyEvent.button == 6)
                        newKey = KeyCode.Mouse6;
                }
                waitingForKey = false;
            }
        }
    }

    public void StartGetButtonDown(TMP_Text text) {
        text.text = "PRESS A KEY";
        soundController.Play("UI Click");
        StartCoroutine(AssignKey(text));
    }

    IEnumerator WaitForKey() {
        while (waitingForKey) {
            yield return null;
        }
    }

    IEnumerator AssignKey(TMP_Text button) {
        yield return new WaitForEndOfFrame();
        waitingForKey = true;
        yield return new WaitForEndOfFrame();
        yield return WaitForKey();
        bool inKeybinds = false;
        foreach (KeyValuePair<string, KeyCode> entry in keybinds) {
            if (newKey == entry.Value) {
                inKeybinds = true;
            }
        }
        if (!inKeybinds) {
            keybinds[button.name] = newKey;
            button.text = newKey.ToString().ToUpper();
        }else {
            button.text = keybinds[button.name].ToString().ToUpper();
        }
    }

    public void ResetKeybinds() {
        soundController.Play("UI Click");
        keybinds.Clear();
        DefaultKeybinds();
        FixingText();
    }

    public void FixingText() {
        attackText.text = keybinds[attackText.name].ToString().ToUpper();
        blockText.text = keybinds[blockText.name].ToString().ToUpper();
        sprintText.text = keybinds[sprintText.name].ToString().ToUpper();
        jumpText.text = keybinds[jumpText.name].ToString().ToUpper();
        interactText.text = keybinds[interactText.name].ToString().ToUpper();
        consumeText.text = keybinds[consumeText.name].ToString().ToUpper();
        reloadText.text = keybinds[reloadText.name].ToString().ToUpper();
        skills1Text.text = keybinds[skills1Text.name].ToString().ToUpper();
        skills2Text.text = keybinds[skills2Text.name].ToString().ToUpper();
        skills3Text.text = keybinds[skills3Text.name].ToString().ToUpper();
    }

    public static void DefaultKeybinds() {
        keybinds.Add("ForwardKey", KeyCode.W);
        keybinds.Add("BackwardKey", KeyCode.S);
        keybinds.Add("RightKey", KeyCode.D);
        keybinds.Add("LeftKey", KeyCode.A);
        keybinds.Add("skill1Key", KeyCode.Alpha1);
        keybinds.Add("skill2Key", KeyCode.Alpha2);
        keybinds.Add("skill3Key", KeyCode.Alpha3);
        keybinds.Add("pickUpKey", KeyCode.Q);
        keybinds.Add("reloadKey", KeyCode.R);
        keybinds.Add("jumpKey", KeyCode.Space);
        keybinds.Add("consumeKey", KeyCode.E);
        keybinds.Add("sprintKey", KeyCode.LeftShift);
        keybinds.Add("attackKey", KeyCode.Mouse0);
        keybinds.Add("blockKey", KeyCode.Mouse1);
    }

    public void ApplyingKeybinds() {
        soundController.Play("UI Click");
        if (SceneController.Instance) {
            Player.PlayerMovement player = SceneController.Instance.player;
            if (player) {
                player.ApplyKeybinds();
                Debug.Log(keybinds["skill1Key"]);
            }
        }
    }
}

