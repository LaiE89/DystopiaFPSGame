using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

namespace Player {
    public class Health : MonoBehaviour {
        public float playerHealth;
        [SerializeField] public float maxPlayerHealth;
        [SerializeField] Slider healthSlider;
        [SerializeField] Image hurtScreen;
        [SerializeField] ingameMenus canvas;
        
        void Start() {
            playerHealth = maxPlayerHealth;
            healthSlider.maxValue = maxPlayerHealth;
        }

        void Update() {
            healthSlider.value = playerHealth;
        }

        public void TakeDamage(float amount) {
            playerHealth -= amount;
            CameraShaker.Instance.ShakeOnce(amount*2, amount, 0.1f, 0.5f);
            if (playerHealth <= 0) {
                StartCoroutine(DeathDelay());
            }
            StartCoroutine(FadeBlood(true));
        }

        IEnumerator DeathDelay() {
            yield return new WaitForSeconds(0.1f);
            canvas.ToggleDeathScreen();
        }

        IEnumerator FadeBlood(bool fadeAway) {
        // fade from opaque to transparent
            if (fadeAway) {
                // loop over 1 second backwards
                for (float i = 1; i >= 0; i -= Time.deltaTime) {
                    // set color with i as alpha
                    hurtScreen.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
            // fade from transparent to opaque
            else {
                // loop over 1 second
                for (float i = 0; i <= 1; i += Time.deltaTime) {
                    // set color with i as alpha
                    hurtScreen.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
        }
    }
}

