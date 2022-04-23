using Player;
using UnityEngine;

public class Pistol : Weapons {

    public override void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, PlayerMovement player) {
        if (this.bullets > 0) {
            Ray ray = player.attackCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            attackSound.Play();
            if (Physics.Raycast(ray, out hit, range, player.playerLayers)) {
                if (hit.collider.tag == "Enemy") {
                    hurtSound.Play();
                    GameObject enemy = hit.collider.gameObject;
                    Enemies.Movement eMovement = enemy.GetComponent<Enemies.Movement>();
                    ParticleSystem blood = Instantiate(SceneController.Instance.bloodParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    blood.Play();
                    player.DealDamage(eMovement, enemy, damage, knockback);
                }else {
                    Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                    if (destructable != null) {
                        destructable.Interact(); 
                    }else {
                        PlaceBulletHole(hit);
                    }
                    ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                    ground.Play();
                }
            }
        }else {
            base.AttackDamage(range, damage, knockback, attackSound, hurtSound, player);
        }
    }

    public override void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0, height - 0.5f, 0), transform.TransformDirection(Vector3.forward) * 10, Color.green);
        //new Vector3(transform.TransformDirection(Vector3.forward).x, directionToTarget.y, transform.TransformDirection(Vector3.forward).z)
        // Debug.DrawRay(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height-0.5f, 0), ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.directionToTarget.y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z) * 20, Color.green, 3);
        enemy.isRotating = false;
        attackSound.Play();
        if (Physics.Raycast(ToolMethods.OffsetPosition(enemy.gameObject.transform.position, 0, enemy.height - 0.5f, 0), ToolMethods.SettingVector(enemy.gameObject.transform.TransformDirection(Vector3.forward).x, enemy.directionToTarget.y, enemy.gameObject.transform.TransformDirection(Vector3.forward).z), out hit, range, enemy.enemyLayers)) {
            if (hit.collider.tag == "Player") {
                enemy.CombatCalculation(damage, knockback, hurtSound);
            }else {
                Destructable destructable = hit.transform.gameObject.GetComponent<Destructable>();
                if (destructable != null) {
                    destructable.Interact(); 
                }else {
                    PlaceBulletHole(hit);
                }
                ParticleSystem ground = Instantiate(SceneController.Instance.groundParticles, hit.point, hit.transform.rotation) as ParticleSystem;
                ground.Play();
            }
        }
    }
    
    private void PlaceBulletHole(RaycastHit hit) {
        Vector3 collisionHitLoc = hit.point;
        Vector3 collisionHitRot = hit.normal;
        // Quaternion HitRot = Quaternion.LookRotation(Vector3.forward, collisionHitRot);
        SceneController.Instance.bulletHolePool.SpawnDecal(collisionHitRot * -1f, collisionHitLoc - collisionHitRot * -1f * 0.01f, ToolMethods.SettingVector(0.1f, 0.1f, 0.1f));
    }
}
