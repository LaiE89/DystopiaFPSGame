using UnityEngine;
using EZCameraShake;

public class ActivateCameraShake : MonoBehaviour {
    [SerializeField] float magnitude = 1.5f;
    [SerializeField] float roughness = 3;
    [SerializeField] float fadeInTime = 0.1f;
    [SerializeField] float fadeOutTime = 0.5f;
    void OnEnable() {
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
    }
}
