using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PiercingWeapon : Weapons {
    public override void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Player.PlayerMovement player) {
        Ray ray = player.attackCam.ScreenPointToRay(Input.mousePosition);
        attackSound.Play();
        RaycastHit[] hits = Physics.RaycastAll(ray, range, player.playerLayers);
        if (hits.Length > 0) {
            hits = sortByDistance(hits);
            foreach (RaycastHit hit in hits) {
                if (hit.collider.tag == "Enemy") {
                    hurtSound.Play();
                    GameObject enemy = hit.collider.gameObject;
                    Enemies.Movement eMovement = enemy.GetComponent<Enemies.Movement>();
                    SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                    player.DealDamage(eMovement, enemy, damage, knockback);
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                        destructable.Interact(); 
                    }
                    SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                    if (hit.transform.gameObject.layer == ToolMethods.LayerMaskToLayer(player.groundMask)) {
                        return;
                    }
                }
            }
        }
    }

    private RaycastHit[] sortByDistance(RaycastHit[] hits) {
        for (int i = 0; i < hits.Length - 1; i++) {
            for (int j = i + 1; j > 0; j--) {
                if (hits[j - 1].distance > hits[j].distance) {
                    RaycastHit temp = hits[j - 1];
                    hits[j - 1] = hits[j];
                    hits[j] = temp;
                }
            }
        }
        return hits;
    }

    private void PrintArray(RaycastHit[] array) {
        string result = "";
        foreach (RaycastHit i in array) {
            result += i.distance.ToString() + ", ";
        }
        Debug.Log(result);
    }
}
