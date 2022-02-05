using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : Trigger {
    public override void result() {
        SceneController.Instance.NextLevel();
    }
}
