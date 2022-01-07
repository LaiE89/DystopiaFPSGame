using UnityEngine;

public class EndTrigger : MonoBehaviour {
    public SceneController controller;
    void OnTriggerEnter() {
        controller.NextLevel();
    }
}