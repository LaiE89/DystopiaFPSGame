using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class MoveCamera : MonoBehaviour {
        [SerializeField] Transform camPosition;
        float speed = 0.1f;
        Vector3 velocity = Vector3.zero;

        void LateUpdate() {
            //transform.position = Vector3.SmoothDamp(transform.position, camPosition.position, ref velocity, speed);
            transform.position = Vector3.Lerp(transform.position, camPosition.position, Time.deltaTime * 50);
        }
    }
}
