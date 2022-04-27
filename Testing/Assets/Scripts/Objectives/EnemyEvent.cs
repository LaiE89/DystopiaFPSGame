using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvent : Trigger {
    [SerializeField] DialogueTrigger dialogueTrigger;
    [SerializeField] Enemies.Movement[] aggressiveEnemies;
    [SerializeField] Enemies.Movement[] passiveEnemies;
    [SerializeField] Enemies.Movement[] enemiesToKill;

    public override void result() {
        if (dialogueTrigger && !dialogueTrigger.enabled) {
            dialogueTrigger.enabled = true;
        }
        Destroy(gameObject);
        foreach (Enemies.Movement enemy in aggressiveEnemies) {
            if (enemy) {
                if (enemy.isPassive) {
                    enemy.isPassive = false;
                }
                if (enemy.isInvicible) {
                    enemy.isInvicible = false;
                }
                enemy.turnNonKinematic();
            }
        }
        foreach (Enemies.Movement enemy in passiveEnemies) {
            if (enemy) {
                if (enemy.isInvicible) {
                    enemy.isInvicible = false;
                }
            }
        }
        foreach (Enemies.Movement enemyToKill in enemiesToKill) {
            if (enemyToKill) {
                if (enemyToKill.isInvicible) {
                    enemyToKill.isInvicible = false;
                }
                enemyToKill.gameObject.transform.SetParent(null);
                enemyToKill.gameObject.transform.localScale = Vector3.one;
                enemyToKill.gameObject.transform.localRotation = Quaternion.identity;
                enemyToKill.TakeDamage(enemyToKill.enemyHealth);
            }
        }
    }
}
