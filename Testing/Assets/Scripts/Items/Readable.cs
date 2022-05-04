using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Readable : Interactable {
    [SerializeField] GameObject notesUI;
    [SerializeField] Image image;
    [SerializeField] GameObject pagePrefab;
    [TextArea(3, 10)]
    public string[] pages;

    public void Start() {
        notesUI.SetActive(false);
        int numPages;
        if (pages.Length % 2 == 0) {
            numPages = (pages.Length / 2);
        }else {
            numPages = (pages.Length / 2) + 1;
        }
        for (int i = 0; i < numPages; i++) {
            GameObject pageGameobject = Instantiate(pagePrefab, image.transform);
            if (i == 0) {
                pageGameobject.SetActive(true);
            }else {
                pageGameobject.SetActive(false);
            }
        }
        List<GameObject> listOfPages = new List<GameObject>();
        List<TextMeshProUGUI> textBoxes = new List<TextMeshProUGUI>();
        foreach (Transform pageTransform in image.transform) {
            listOfPages.Add(pageTransform.gameObject);
        }
        foreach (GameObject pageUI in listOfPages) {
            foreach (Transform child in pageUI.transform) {
                TextMeshProUGUI childText = child.GetComponent<TextMeshProUGUI>();
                Button childButton = child.GetComponent<Button>();
                if (childText) {
                    textBoxes.Add(childText);
                }else if (childButton) {
                    if (childButton.gameObject.name == "Next") {
                        int nextPageIndex = listOfPages.IndexOf(pageUI) + 1;
                        if (nextPageIndex <= listOfPages.Count - 1) {
                            childButton.onClick.AddListener(delegate { FlipPage(listOfPages[nextPageIndex]); });
                        }else {
                            Destroy(childButton.gameObject);
                        }
                    }else {
                        int prevPageIndex = listOfPages.IndexOf(pageUI) - 1;
                        if (prevPageIndex >= 0) {
                            childButton.onClick.AddListener(delegate { FlipPage(listOfPages[prevPageIndex]); });
                        }else {
                            Destroy(childButton.gameObject);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < pages.Length; i++) {
            textBoxes[i].text = pages[i];
        }
    }

    void FlipPage (GameObject ok) {
        SceneController.Instance.soundController.Play("Page Turn");
        ok.SetActive(true);
    }

    public void ExitReading() {
        SceneController.Instance.soundController.Play("UI Click");
        notesUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        ingameMenus.pausedGame = false;
    }

    public override void Interact(Transform user) {
        base.Interact();
        notesUI.SetActive(true);
        SceneController.Instance.soundController.PauseAll();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        ingameMenus.pausedGame = true;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }
}