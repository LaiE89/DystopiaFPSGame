using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;

/*namespace Player {
    [System.Serializable]
    public class PlayerData {

        public float walkSpeed;
        public float sprintSpeed;
        public float jumpForce;
        public int playerDrugs;
        public int pickUpRange;
        public GameObject myWeapon;
        
        public PlayerData (Player.PlayerMovement player) {
            walkSpeed = player.walkSpeed;
            sprintSpeed = player.sprintSpeed;
            jumpForce = player.jumpForce;
            playerDrugs = player.playerDrugs;
            pickUpRange = player.pickUpRange;
            myWeapon = player.myWeapon;
        }
    }
}*/

namespace Player {
    [DataContract]
    public class PlayerData {
        [DataMember]
        public float playerHealth;
        [DataMember]
        public float playerHunger;
        [DataMember]
        public float walkSpeed;
        [DataMember]
        public float sprintSpeed;
        [DataMember]
        public float jumpForce;
        [DataMember]
        public int playerDrugs;
        [DataMember]
        public int playerAmmo;
        [DataMember]
        public int pickUpRange;
        [DataMember]
        public int sceneIndex;
        [DataMember]
        public List<string> statusEffects;
        [DataMember]
        public string myWeapon;

        
        public PlayerData (Player.PlayerMovement player) {
            playerHealth = player.playerHealth;
            playerHunger = player.playerHunger;
            walkSpeed = player.walkSpeed;
            sprintSpeed = player.sprintSpeed;
            jumpForce = player.jumpForce;
            playerDrugs = player.playerDrugs;
            playerAmmo = player.playerAmmo;
            pickUpRange = player.pickUpRange;
            sceneIndex = player.sceneIndex;
            statusEffects = player.statusEffects;
            myWeapon = player.myWeapon.name.Replace("(Clone)", "");
        }

        public PlayerData (float playerHealth, float playerHunger, float walkSpeed, float sprintSpeed, float jumpForce, int playerDrugs, int playerAmmo, int pickUpRange, int sceneIndex, List<string> statusEffects, string myWeapon) {
            this.playerHealth = playerHealth;
            this.playerHunger = playerHunger;
            this.walkSpeed = walkSpeed;
            this.sprintSpeed = sprintSpeed;
            this.jumpForce = jumpForce;
            this.playerDrugs = playerDrugs;
            this.pickUpRange = pickUpRange;
            this.sceneIndex = sceneIndex;
            this.statusEffects = statusEffects;
            this.myWeapon = myWeapon;
        }
    }
}