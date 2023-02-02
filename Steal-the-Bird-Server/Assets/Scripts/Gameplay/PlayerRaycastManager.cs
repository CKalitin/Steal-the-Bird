using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastManager : MonoBehaviour {
    #region Variables

    public static PlayerRaycastManager instance;

    [SerializeField] private GameObject playerCameraPrefab;
    [Space]
    [SerializeField] private LayerMask raycastLayerMask;

    private Dictionary<int, Camera> playerCameras = new Dictionary<int, Camera>();

    private Dictionary<int, PlayerWeaponController> pwcs = new Dictionary<int, PlayerWeaponController>();

    public Dictionary<int, PlayerWeaponController> Pwcs { get => pwcs; set => pwcs = value; }

    #endregion

    #region Core

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Player Raycast Manager instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }

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

    #endregion

    #region Callbacks

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

        if (!playerCameras.ContainsKey(packet.FromClient)) return;

        playerCameras[packet.FromClient].fieldOfView = packet.FieldOfView;

        playerCameras[packet.FromClient].transform.position = packet.CameraPosition;
        playerCameras[packet.FromClient].transform.rotation = packet.CameraRotation;

        if (playerCameras[packet.FromClient].targetTexture) playerCameras[packet.FromClient].targetTexture.Release();
        if (playerCameras[packet.FromClient].targetTexture) playerCameras[packet.FromClient].targetTexture.DiscardContents();
        playerCameras[packet.FromClient].targetTexture = null;
        RenderTexture rt = new RenderTexture((int)packet.Resolution.x, (int)packet.Resolution.y, 24);
        rt.Create();
        playerCameras[packet.FromClient].targetTexture = rt;

        Ray ray = playerCameras[packet.FromClient].ViewportPointToRay(packet.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 9999, raycastLayerMask)) {
            if (pwcs.ContainsKey(packet.FromClient)) pwcs[packet.FromClient].AimWeapon(hit.collider.transform.position);
        }
    }

    #endregion
}
