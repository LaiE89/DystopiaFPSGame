using UnityEngine;

public class Holdable : Interactable {

    Weapons weaponStats;
    Rigidbody rb;
    BoxCollider itemCollider;
    
    /*private void Start() {
        weaponStats = GetComponent<Weapons>();
        itemCollider = GetComponent<BoxCollider>();
    }*/

    private void Awake() {
        weaponStats = GetComponent<Weapons>();
        itemCollider = GetComponent<BoxCollider>();
    }

    public void DroppingWeapon(Transform holder) {
        if (!weaponStats.isDefaultItem) {
            transform.SetParent(null);
            transform.localScale = Vector3.one;

            if (!GetComponent<Rigidbody>()) {
                gameObject.AddComponent<Rigidbody>();
            }

            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            itemCollider.isTrigger = false;

            gameObject.SetActive(true);

            gameObject.layer = LayerMask.NameToLayer("Item");
            foreach (Transform child in transform) {
                child.gameObject.layer = LayerMask.NameToLayer("Item");
            }

            rb.AddForce(holder.forward, ForceMode.Impulse);
            rb.AddForce(holder.up * 2, ForceMode.Impulse);
            float random = Random.Range(-1f,1f);
            // new Vector3(random,random,random)
            rb.AddTorque(ToolMethods.SettingVector(random, random, random) * 10);
        }
    }

    public void CreatingWeapon(Transform location) {
        //transform.parent = null;
        itemCollider = GetComponent<BoxCollider>();
        itemCollider.isTrigger = false;
        if (!GetComponent<Rigidbody>()) {
            gameObject.AddComponent<Rigidbody>();
        }
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.AddForce(location.forward, ForceMode.Impulse);
        rb.AddForce(location.up * 2, ForceMode.Impulse);
        float random = Random.Range(-1f,1f);
        rb.AddTorque(ToolMethods.SettingVector(random, random, random) * 10);
    }
    
    public override void Interact(Transform parent) {
        SceneController.Instance.player.interactTextBox.text = ("");
        SceneController.Instance.soundController.PlayOneShot("Pick Up");
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<ParticleSystem>() == null){
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
        //this.GetComponent<BoxCollider>().isTrigger = true;
        itemCollider.isTrigger = true;
        if (GetComponent<Rigidbody>()) {
            Destroy(GetComponent<Rigidbody>());
        }
    }
}