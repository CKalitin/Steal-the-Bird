using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastHitManager : MonoBehaviour {
    [SerializeField] private GameObject playerCameraPrefab;
    [Space]
    [SerializeField] private LayerMask raycastLayerMask;

    private Dictionary<int, Camera> playerCameras = new Dictionary<int, Camera>();

    private void OnEnable() {
        USNL.CallbackEvents.OnClientConnected += OnClientConnected;
        USNL.CallbackEvents.OnClientDisconnected += OnClientDisconnected;
        USNL.CallbackEvents.OnRaycastFromCameraPacket += OnRaycastFromCameraPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnClientConnected -= OnClientConnected;
        USNL.CallbackEvents.OnClientDisconnected -= OnClientDisconnected;
        USNL.CallbackEvents.OnRaycastFromCameraPacket -= OnRaycastFromCameraPacket;
    }

    private void OnClientConnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;

        GameObject newPlayerCamera = Instantiate(playerCameraPrefab, Vector3.zero, Quaternion.identity);
        playerCameras.Add(clientId, newPlayerCamera.GetComponent<Camera>());
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;

        Destroy(playerCameras[clientId].gameObject);
        playerCameras.Remove(clientId);
    }

    private void OnRaycastFromCameraPacket(object _packetObject) {
        USNL.RaycastFromCameraPacket packet = (USNL.RaycastFromCameraPacket)_packetObject;

        playerCameras[packet.FromClient].fieldOfView = packet.FieldOfView;

        playerCameras[packet.FromClient].transform.position = packet.CameraPosition;
        playerCameras[packet.FromClient].transform.rotation = packet.CameraRotation;

        playerCameras[packet.FromClient].targetTexture = null;
        RenderTexture rt = new RenderTexture((int)packet.Resolution.x, (int)packet.Resolution.y, 24);
        playerCameras[packet.FromClient].targetTexture = rt;

        Ray ray = playerCameras[packet.FromClient].ViewportPointToRay(packet.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 9999, raycastLayerMask)) {
            Debug.DrawRay(playerCameras[packet.FromClient].transform.position, (hit.point - playerCameras[packet.FromClient].transform.position) * 5, Color.red, 1f);
            //Debug.Log(hit.transform.name);
        }
    }
}
