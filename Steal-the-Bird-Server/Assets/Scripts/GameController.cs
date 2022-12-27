using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    #region Variables

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

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
        players.Add(_clientId, newPlayer);
    }

    #endregion

    #region Callbacks

    private void OnClientConnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;
        SpawnPlayer(clientId);
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;
        Destroy(players[clientId]);
        players.Remove(clientId);
    }
    
    #endregion
}
