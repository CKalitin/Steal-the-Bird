using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    #region Variables

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;

    private Dictionary<int, PlayerController> playerControllers = new Dictionary<int, PlayerController>();

    #endregion

    #region Core

    private void Awake() {
        Debug.LogError("Opened Console.");
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnClientConnected += OnClientConnected;
        USNL.CallbackEvents.OnClientDisconnected += OnClientDisconnected;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnClientConnected -= OnClientConnected;
        USNL.CallbackEvents.OnClientDisconnected -= OnClientDisconnected;
    }
    
    #endregion

    #region Game Controller

    private void SpawnPlayer(int _clientId) {
        GameObject newPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        newPlayer.GetComponent<PlayerController>().ClientId = _clientId;
        playerControllers.Add(_clientId, newPlayer.GetComponent<PlayerController>());

        int[] connectedClients = USNL.ServerManager.GetConnectedClientIds();
        for (int i = 0; i < connectedClients.Length; i++)
            USNL.PacketSend.PlayerSpawned(connectedClients[i], connectedClients[i], newPlayer.GetComponent<USNL.SyncedObject>().SyncedObjectUUID);
    }

    #endregion

    #region Callbacks

    private void OnClientConnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;
        SpawnPlayer(clientId);

        for (int i = 0; i < playerControllers.Count; i++)
            USNL.PacketSend.PlayerSpawned(clientId, playerControllers[i].gameObject.GetComponent<PlayerController>().ClientId, playerControllers[i].gameObject.GetComponent<USNL.SyncedObject>().SyncedObjectUUID);
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;
        Destroy(playerControllers[clientId].gameObject);
        playerControllers.Remove(clientId);
    }
    
    #endregion
}
