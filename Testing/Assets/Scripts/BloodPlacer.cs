using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPlacer : MonoBehaviour {
    
    public ParticleSystem p;

    private List<ParticleCollisionEvent> collisionEvents;
 
    /*void OnParticleCollision(GameObject other) {
        /*int safeLength = GetComponent<ParticleSystem>().GetSafeCollisionEventSize();
        // Debug.Log("Safe Length: " + safeLength);
        if (collisionEvents.Length < safeLength) {
            collisionEvents = new ParticleCollisionEvent[safeLength];
        }
        int numCollisionEvents = p.GetCollisionEvents(other, collisionEvents);
        // Debug.Log("Num Collision Events: " + numCollisionEvents + ", Collision Events: " + collisionEvents);
        for(int i = 0; i < numCollisionEvents; i++) {
            Vector3 collisionHitLoc = collisionEvents[i].intersection;
            Vector3 collisionHitRot = collisionEvents[i].normal;
            Quaternion HitRot = Quaternion.LookRotation(Vector3.forward, collisionHitRot);

            var decal = Instantiate(objectToInstantiate); 
            decal.transform.forward = collisionHitRot * -1f;
            decal.transform.position = collisionHitLoc - decal.transform.forward * 0.01f;
        }
    }*/

    private void Start() {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other) {
        p.GetCollisionEvents(other, collisionEvents);
        CreateExplosion(collisionEvents[0]);
    }

    private async void CreateExplosion(ParticleCollisionEvent particle) {
        collisionEvents.Clear();
        Vector3 collisionHitLoc = particle.intersection;
        Vector3 collisionHitRot = particle.normal;
        Quaternion HitRot = Quaternion.LookRotation(Vector3.forward, collisionHitRot);
        float randNum = Random.Range(0, 10);
        float randNum2 = Random.Range(1, 100);
        float randNum3 = Random.Range(1, 100);
        float randNum4 = Random.Range(1, 100);
        if (randNum >= 8f) {
            SceneController.Instance.bloodPool.SpawnDecal(collisionHitRot * -1f, collisionHitLoc - collisionHitRot * -1f * 0.01f, ToolMethods.SettingVector(randNum2 / 100, randNum3 / 100, randNum4 / 100));
        }
        /*var decal = Instantiate(objectToInstantiate); 
        decal.transform.forward = collisionHitRot * -1f;
        decal.transform.position = collisionHitLoc - decal.transform.forward * 0.01f;*/
    }

}
