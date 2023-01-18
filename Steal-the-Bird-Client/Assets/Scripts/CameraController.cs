using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    #region Variables

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

    [Header("Match States")]
    [SerializeField] private Transform lobbyCameraPosition;
    [SerializeField] private float lobbyToGameLerpSpeed = 1f;

    private float lobbyLerpProgress;

    // If the game state is inGame and the camera should be at the in game camera position
    private bool gameCamera = false;
    private bool lerpToGameCamPos = false;

    #endregion

    #region Core

    private void Start() {
        if (MatchManager.instance.MatchState == MatchState.InGame | MatchManager.instance.MatchState == MatchState.Paused)
            gameCamera = true;
        else if (MatchManager.instance.MatchState == MatchState.Lobby) {
            cam.transform.position = lobbyCameraPosition.position;
            cam.transform.rotation = lobbyCameraPosition.rotation;

        }
    }

    private void Update() {
        if (gameCamera) {
            Zoom();
        }

        if (lerpToGameCamPos) {
            lobbyLerpProgress += Mathf.Clamp(0, 1, lobbyToGameLerpSpeed * Time.deltaTime);

            cam.transform.position = Vector3.Lerp(lobbyCameraPosition.position, Vector3.Lerp(defaultCamPosition.position, transform.position, currentZoom), lobbyLerpProgress);
            cam.transform.rotation = Quaternion.Lerp(lobbyCameraPosition.rotation, defaultCamPosition.rotation, lobbyLerpProgress);
            
            if (followTransform != null) {
                transform.position = Vector3.Lerp(lobbyCameraPosition.position, followTransform.position, lobbyLerpProgress);
            }

            if (lobbyLerpProgress >= 1) {
                lobbyLerpProgress = 0;
                lerpToGameCamPos = false;
                gameCamera = true;
            }
        }
    }

    private void LateUpdate() {
        if (gameCamera) {
            FollowPlayer();
        }
    }

    private void FixedUpdate() {
        if (gameCamera) {
            FollowPlayer();
            RaycastFromCamera();
        }
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerSpawnedPacket += OnPlayerSpawnedPacket;
        USNL.CallbackEvents.OnMatchUpdatePacket += OnMatchUpdatePacket;
    }
    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerSpawnedPacket -= OnPlayerSpawnedPacket;
        USNL.CallbackEvents.OnMatchUpdatePacket -= OnMatchUpdatePacket;
    }

    #endregion

    #region Camera Movement

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

    #endregion

    #region Management Functions

    public void LerpLobbyToGameCamPos() {
        lerpToGameCamPos = true;
    }

    public void ResetCamera() {
        gameCamera = false;
        lerpToGameCamPos = false;

        cam.transform.position = lobbyCameraPosition.position;
    }

    #endregion

    #region Callbacks

    private void OnPlayerSpawnedPacket(object _packetObject) {
        USNL.PlayerSpawnedPacket _packet = (USNL.PlayerSpawnedPacket)_packetObject;

        if (_packet.ClientId == USNL.ClientManager.instance.ClientId) {
            followTransform = USNL.SyncedObjectManager.instance.GetSyncedObject(_packet.PlayerSyncedObjectUUID).transform;
        }
    }

    private void OnMatchUpdatePacket(object _packetObject) {
        USNL.MatchUpdatePacket packet = (USNL.MatchUpdatePacket)_packetObject;

        if ((MatchState)packet.MatchState == MatchState.InGame | (MatchState)packet.MatchState == MatchState.Paused)
            LerpLobbyToGameCamPos();

        if ((MatchState)packet.MatchState == MatchState.Lobby)
            ResetCamera();
    }

    #endregion
}
