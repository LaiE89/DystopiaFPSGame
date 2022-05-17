using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Food : Interactable {
    float curPickUpTime;
    [SerializeField] public float nutritionValue;
    [SerializeField] public float pickUpTime;
    [SerializeField] public bool isAlive;
    bool isEating = false;
    Player.PlayerMovement player;

    void Start() {
        player = SceneController.Instance.player;
        if (isAlive) {
            this.enabled = false;
        }
        curPickUpTime = 0;
    }

    public override void Interact() {
        if (!player.isEating) {
            player.isEating = true;
            StartCoroutine(Consuming());
        }
    }

    IEnumerator Consuming() {
        while (player.isEating) {
            curPickUpTime += Time.deltaTime;
            player.holdInteractSlider.value = curPickUpTime;
            if (curPickUpTime < pickUpTime && !isEating) {
                StartCoroutine(EatingCooldown());
            }
            if (curPickUpTime >= pickUpTime) {
                player.weaponAnimator.ResetTrigger("isConsuming");
                SceneController.Instance.soundController.PlayOneShot("Death");
                player.playerHunger += nutritionValue;
                if (player.playerHunger > player.maxPlayerHunger) {
                    player.playerHunger = player.maxPlayerHunger;
                }
                player.hungerSlider.value = player.playerHunger;
                if (player.statusEffects.Contains("isHungry")) {
                    player.statusEffects.Remove("isHungry");
                    player.UpdatingStatus(player.statusEffects);
                }
                OnMouseExit();
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    IEnumerator EatingCooldown() {
        isEating = true;
        player.weaponAnimator.SetTrigger("isConsuming");
        yield return new WaitForSeconds(0.7f);
        // ParticleSystem eatParticles = Instantiate(SceneController.Instance.bloodParticles, ToolMethods.OffsetPosition(transform.position, 0, 0.1f, 0), transform.rotation);
        // eatParticles.Play();
        SceneController.Instance.bloodParticlePool.SpawnDecal(transform.up, ToolMethods.OffsetPosition(transform.position, 0, 0.1f, 0), ToolMethods.SettingVector(1f, 1f, 1f));
        yield return new WaitForEndOfFrame();
        player.weaponAnimator.ResetTrigger("isConsuming");
        isEating = false;
    }

    public override void OnMouseOver() {
        if (this.enabled) {
            base.OnMouseOver();
            if (!ingameMenus.pausedGame && player.interactableInRange(this.gameObject)){
                player.holdInteractSlider.gameObject.SetActive(true);
                player.holdInteractSlider.maxValue = pickUpTime;
                player.holdInteractSlider.value = curPickUpTime;
            }else {
                player.holdInteractSlider.gameObject.SetActive(false);
                StopCoroutine(Consuming());
                player.isEating = false;
            }
        }
    }

    public override void OnMouseExit() {
        if (this.enabled) {
            base.OnMouseExit();
            if (!ingameMenus.pausedGame) {
                player.holdInteractSlider.gameObject.SetActive(false);
                StopCoroutine(Consuming());
                player.isEating = false;
            }
        }
    }
}