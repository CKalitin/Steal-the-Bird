using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {
    [SerializeField] private Transform birdModel;
    [Space]
    [SerializeField] private USNL.SyncedObject syncedObject;

    private void OnDestroy() {
        if (GameController.ApplicationQuitting) return; // Fixes bug

        birdModel.parent = null;

        birdModel.GetComponent<DeadBird>().enabled = true;
        birdModel.GetComponent<Animator>().enabled = true;

        birdModel.GetComponent<DeadBird>().StartRotation = transform.rotation;

        birdModel.GetComponent<DeadBird>().SetDead();

        birdModel.GetComponent<DeadBird>().SyncedObjectUUID = syncedObject.SyncedObjectUuid;
        birdModel.GetComponent<DeadBird>().StartPosition = transform.position;
    }
}
