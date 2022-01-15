using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneFinisher : MonoBehaviour {
    public CutSceneMenu menu;

    void OnEnable() {
        menu.NextLevel();
    }
}
