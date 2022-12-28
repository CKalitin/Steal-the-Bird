using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdKillCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Bird>()) {
            other.gameObject.GetComponent<Health>().ChangeHealth(-999999);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Bird>()) {
            other.gameObject.GetComponent<Health>().ChangeHealth(-999999);
        }
    }
}
