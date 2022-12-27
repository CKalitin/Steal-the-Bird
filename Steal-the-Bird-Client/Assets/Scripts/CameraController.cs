using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Transform cam;
    [SerializeField] private Transform defaultCamPosition;
    [Space]
    [SerializeField] private float minCameraDist = 3f;
    [SerializeField] private float maxCameraDist = 8f;
    [Space]
    [SerializeField] private float currentZoom = 0.5f;
    [SerializeField] private float zoomStep = 0.05f;
    [Space]
    [SerializeField] private GameObject cameraPrefab;

    private void Update() {
        Zoom();
    }

    private void OnEnable() { USNL.CallbackEvents.OnPlayerSpawnedPacket += OnPlayerSpawnedPacket; }
    private void OnDisable() { USNL.CallbackEvents.OnPlayerSpawnedPacket -= OnPlayerSpawnedPacket; }

    private void Zoom() {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (scroll > 0) currentZoom = Mathf.Clamp(currentZoom + zoomStep, minCameraDist / maxCameraDist, 1);
        if (scroll < 0) currentZoom = Mathf.Clamp(currentZoom - zoomStep, minCameraDist / maxCameraDist, 1);

        Vector3 targetPos = Vector3.Lerp(defaultCamPosition.position, transform.position, currentZoom);

        if (Vector3.Distance(transform.position, targetPos) <= maxCameraDist && Vector3.Distance(transform.position, targetPos) >= minCameraDist) {
            cam.position = targetPos;
        }
    }

    private void OnPlayerSpawnedPacket(object _packetObject) {
        USNL.PlayerSpawnedPacket _packet = (USNL.PlayerSpawnedPacket)_packetObject;

        if (_packet.ClientId == USNL.ClientManager.instance.ClientId) {
            transform.position = USNL.SyncedObjectManager.instance.GetSyncedObject(_packet.PlayerSyncedObjectUUID).transform.position;
            transform.parent = USNL.SyncedObjectManager.instance.GetSyncedObject(_packet.PlayerSyncedObjectUUID).transform;
            transform.parent.GetComponent<PlayerController>().CameraTransform = transform;
        }
    }
}
