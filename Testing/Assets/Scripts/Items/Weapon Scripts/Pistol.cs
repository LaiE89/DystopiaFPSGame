using Player;
using UnityEngine;
using System.Collections;

public class Pistol : Weapons {
    public float upRecoil;
    public TrailRenderer bulletTrail;
    
    public override void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, PlayerMovement player) {
        if (this.bullets > 0) {
            Ray ray = player.attackCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            attackSound.Play();
            TrailRenderer trail = Instantiate(bulletTrail, ray.origin, Quaternion.identity);
            if (Physics.Raycast(ray, out hit, range, player.playerLayers)) {
                StartCoroutine(SpawnTrail(trail, hit.point));
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
                    }else {
                        if (hit.collider.tag != "Appliance") {
                            PlaceBulletHole(hit);
                        }
                    }
                    SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                }
            }else {
                StartCoroutine(SpawnTrail(trail, ray.origin + ray.direction * range));
            }
            player.AddRecoil(upRecoil);
        }else {
            base.AttackDamage(range, damage, knockback, attackSound, hurtSound, player);
        }
    }

    public override void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        RaycastHit hit;
        Vector3 direc = enemy.GetShootingDirection();
        Debug.DrawRay(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height-0.5f, 0), direc * 20, Color.green, 1);
        //Debug.DrawLine (ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height-0.5f, 0), ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height-0.5f, 0) + ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.directionToTargetEyeLevel.y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z) * 10, Color.red, Mathf.Infinity);
        enemy.isRotating = false;
        attackSound.Play();
        TrailRenderer trail = Instantiate(bulletTrail, ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), Quaternion.identity);
        if (Physics.Raycast(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), direc, out hit, range, enemy.enemyLayers)) {
            StartCoroutine(SpawnTrail(trail, hit.point));
            if (hit.collider.tag == "Player") {
                bool hasDamaged = enemy.CombatCalculation(damage, knockback, hurtSound);
                if (hasDamaged) {
                    SceneController.Instance.bloodParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
                }
            }else {
                Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact(); 
                }else {
                    if (hit.collider.tag != "Appliance") {
                        PlaceBulletHole(hit);
                    }
                }
                SceneController.Instance.groundParticlePool.SpawnDecal(hit.transform.forward, hit.point, ToolMethods.SettingVector(1f, 1f, 1f));
            }
        }else {
            StartCoroutine(SpawnTrail(trail, ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0) + direc * range));
        }
    }
    
    private void PlaceBulletHole(RaycastHit hit) {
        Vector3 collisionHitLoc = hit.point;
        Vector3 collisionHitRot = hit.normal;
        // Quaternion HitRot = Quaternion.LookRotation(Vector3.forward, collisionHitRot);
        SceneController.Instance.bulletHolePool.SpawnDecal(collisionHitRot * -1f, collisionHitLoc - collisionHitRot * -1f * 0.01f, ToolMethods.SettingVector(0.1f, 0.1f, 0.1f));
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 destination) {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1) {
            trail.transform.position = Vector3.Lerp(startPosition, destination, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = destination;
        Destroy(trail.gameObject, trail.time);
    }
}
