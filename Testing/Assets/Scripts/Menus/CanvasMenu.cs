using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMenu : MonoBehaviour {
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] public float fadeTime;

    void Start() {
        StartCoroutine(DoFadeIn(fadeTime));
    }

    private IEnumerator DoFadeIn(float fadeTime) {
        for (float i = 0; i <= 1 * fadeTime; i += Time.deltaTime) {
            canvasGroup.alpha = Mathf.Clamp01(i / fadeTime);
            yield return null;
        }
        yield return null;
    }
}
