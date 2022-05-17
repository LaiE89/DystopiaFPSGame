using Player;
using UnityEngine;

public class RPG : Weapons {
    public GameObject shellPrefab;
    public float upRecoil;
    public float force;
    
    public override void AttackDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, PlayerMovement player) {
        if (this.bullets > 0) {
            attackSound.Play();
            GameObject shell = Instantiate(shellPrefab, player.attackCam.ScreenToWorldPoint(Input.mousePosition), player.attackCam.transform.rotation);
            Destructable destructable = shell.GetComponent<Destructable>();
            Rigidbody rb = destructable.thisRb;
            destructable.thrower = player.gameObject;
            rb.AddForce(ToolMethods.SettingVector(player.attackCam.transform.forward.x * force, player.attackCam.transform.forward.y * force, player.attackCam.transform.forward.z * force), ForceMode.Impulse);
            player.AddRecoil(upRecoil);
        }else {
            base.AttackDamage(range, damage, knockback, attackSound, hurtSound, player);
        }
    }

    public override void RaycastDamage(float range, float damage, float knockback, AudioSource attackSound, AudioSource hurtSound, Enemies.Movement enemy) {
        enemy.isRotating = false;
        Vector3 direc = enemy.GetShootingDirection();
        attackSound.Play();
        GameObject shell = Instantiate(shellPrefab, ToolMethods.OffsetPosition(enemy.transform.position, 0, enemy.height - 0.25f, 0f) + enemy.transform.forward, Quaternion.LookRotation(direc));
        Destructable destructable = shell.GetComponent<Destructable>();
        Rigidbody rb = destructable.thisRb;
        destructable.thrower = enemy.gameObject;
        rb.AddForce(direc * force, ForceMode.Impulse);
        // rb.AddForce(ToolMethods.SettingVector(direc.x * force, direc.y, direc.z * force), ForceMode.Impulse);
    }
}
