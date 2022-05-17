using Player;
using UnityEngine;
using System.Collections;

public class Shotgun : Weapons {
    public float upRecoil;
    public int pellets;
    public float spread; // lower = smaller spread
    public TrailRenderer bulletTrail;

    public override void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, PlayerMovement player) {
        if (this.bullets > 0) {
            Ray[] rays = CreatingRays(player);
            foreach (Ray ray in rays) {
                BulletRaycast(range, damage, knockback, attackSound, hurtSound, ray, player);
            }
            player.AddRecoil(upRecoil);
        }else {
            base.AttackDamage(range, damage, knockback, attackSound, hurtSound, player);
        }
    }

    public override void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        if (this.bullets > 0) {
            Ray[] rays = CreatingRays(enemy);
            foreach (Ray ray in rays) {
                BulletRaycast(range, damage, knockback, attackSound, hurtSound, ray, enemy);
            }
        }else {
            base.RaycastDamage(range, damage, knockback, attackSound, hurtSound, enemy);
        }
    }
    
    private Ray[] CreatingRays(PlayerMovement player) {
        Ray[] rays = new Ray[pellets];
        Ray mouseRay = player.attackCam.ScreenPointToRay(Input.mousePosition);
        for(int i = 0; i < pellets; i++) {
            rays[i] = new Ray(mouseRay.origin, GetShootingDirection(mouseRay));  
        }
        return rays;
    }

    private Ray[] CreatingRays(Enemies.Movement enemy) {
        Ray[] rays = new Ray[pellets];
        Vector3 origin = ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0);
        Vector3 targetPos = enemy.GetShootingDirection();
        for(int i = 0; i < pellets; i++) {
            rays[i] = new Ray(origin, GetShootingDirection(targetPos));
        }
        return rays;
    }

    private void BulletRaycast(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Ray ray, Player.PlayerMovement player) {
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.green, 1f);
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
    }

    private void BulletRaycast(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Ray ray, Enemies.Movement enemy) {
        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.green, 1f);
        RaycastHit hit;
        attackSound.Play();
        TrailRenderer trail = Instantiate(bulletTrail, ray.origin, Quaternion.identity);
        if (Physics.Raycast(ray, out hit, range, enemy.enemyLayers)) {
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
            StartCoroutine(SpawnTrail(trail, ray.origin + ray.direction * range));
        }
    }

    private Vector3 GetShootingDirection(Ray mouseRay) {
        Vector3 targetPos = ToolMethods.OffsetPosition(mouseRay.direction * this.shootRange, Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        // targetPos.
        Vector3 direction = targetPos - mouseRay.direction;
        return direction;
    }

    private Vector3 GetShootingDirection(Vector3 direction) {
        Vector3 targetPos = ToolMethods.OffsetPosition(direction * this.shootRange, Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));
        Vector3 fDirection = targetPos - direction;
        return fDirection;
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
