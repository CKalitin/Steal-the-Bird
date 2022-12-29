using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Zoom")]
    [SerializeField] private Transform defaultCamPosition;
    [Space]
    [SerializeField] private float minCameraDist = 3f;
    [SerializeField] private float maxCameraDist = 8f;
    [Space]
    [SerializeField] private float currentZoom = 0.5f;
    [SerializeField] private float zoomStep = 0.05f;
    [Space]
    [SerializeField] private GameObject cameraPrefab;

    [Header("Follow")]
    [SerializeField] private Transform followTransform;

    [Header("Raycast")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask raycastLayerMask;

    private void Update() {
        Zoom();
        FollowPlayer();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastFromCamera();
        }
    }

    private void LateUpdate() {
        FollowPlayer();
    }

    private void FixedUpdate() {
        FollowPlayer();
    }

    private void OnEnable() { USNL.CallbackEvents.OnPlayerSpawnedPacket += OnPlayerSpawnedPacket; }
    private void OnDisable() { USNL.CallbackEvents.OnPlayerSpawnedPacket -= OnPlayerSpawnedPacket; }
    
    private void FollowPlayer() {
        if (followTransform == null) return;
        transform.position = followTransform.position;
    }
    
    private void Zoom() {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        if (scroll > 0) currentZoom = Mathf.Clamp(currentZoom + zoomStep, minCameraDist / maxCameraDist, 1);
        if (scroll < 0) currentZoom = Mathf.Clamp(currentZoom - zoomStep, minCameraDist / maxCameraDist, 1);

        Vector3 targetPos = Vector3.Lerp(defaultCamPosition.position, transform.position, currentZoom);

        if (Vector3.Distance(transform.position, targetPos) <= maxCameraDist && Vector3.Distance(transform.position, targetPos) >= minCameraDist) {
            cam.transform.position = targetPos;
        }
    }

    private void RaycastFromCamera() {
        USNL.PacketSend.RaycastFromCamera(cam.transform.position, cam.transform.rotation, new Vector2(Screen.width, Screen.height), cam.fieldOfView, cam.ScreenToViewportPoint(Input.mousePosition));
    }

    private void OnPlayerSpawnedPacket(object _packetObject) {
        USNL.PlayerSpawnedPacket _packet = (USNL.PlayerSpawnedPacket)_packetObject;

        if (_packet.ClientId == USNL.ClientManager.instance.ClientId) {
            followTransform = USNL.SyncedObjectManager.instance.GetSyncedObject(_packet.PlayerSyncedObjectUUID).transform;
        }
    }
}
