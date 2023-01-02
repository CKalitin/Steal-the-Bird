using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {
    [SerializeField] private GameObject deadBird;
    [Space]
    [SerializeField] private USNL.SyncedObject syncedObject;

    private void OnDestroy() {
        if (GameController.ApplicationQuitting) return; // Fixes bug

        DeadBird newDeadBird = Instantiate(deadBird, transform.position, transform.rotation).GetComponent<DeadBird>();

        newDeadBird.SyncedObjectUUID = syncedObject.SyncedObjectUuid;
        newDeadBird.StartPosition = transform.position;
    }
}
