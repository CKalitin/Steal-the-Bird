using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    #region Variables

    public static GameController instance;

    [Header("Gameplay")]
    [SerializeField] private float respawnTime = 5f;

    [Header("Management")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject[] playerPrefabs;

    private Dictionary<int, PlayerController> playerControllers = new Dictionary<int, PlayerController>();

    private MatchState previousMatchState = MatchState.Lobby;

    #endregion

    #region Core

    private void Awake() {
        Debug.LogError("Opened Console.");

        if (instance == null) instance = this;
        else {
            Debug.Log($"Game Controller instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }

    private void Update() {
        if (previousMatchState != MatchManager.instance.MatchState) OnMatchStateChange(MatchManager.instance.MatchState);
        previousMatchState = MatchManager.instance.MatchState;
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

    #region Match State

    private void OnMatchStateChange(MatchState _ms) {
        if (_ms == MatchState.Lobby) {
            USNL.ServerManager.instance.AllowNewConnections = true;
        } else if (_ms == MatchState.InGame) {
            USNL.ServerManager.instance.AllowNewConnections = false;
            StartGame();
        } else if (_ms == MatchState.Ended) {
            ResetGame();
        }
    }

    private void StartGame() {
        for (int i = 0; i < USNL.ServerManager.GetConnectedClientIds().Length; i++) {
            SpawnPlayer(i);
        }
    }

    private void ResetGame() {
        DestroyAllPlayers();
        BirdSpawner.instance.DestroyAllBirds();
    }

    #endregion

    #region Game Controller

    private void SpawnPlayer(int _clientId) {
        GameObject newPlayer = Instantiate(playerPrefabs[PlayerInfoManager.instance.PlayerInfos[_clientId].CharacterId], playerSpawnPoint.position, playerSpawnPoint.rotation);
        newPlayer.GetComponent<PlayerController>().ClientId = _clientId;
        newPlayer.GetComponent<PlayerController>().PlayerWeaponController.ClientId = _clientId;
        playerControllers.Add(_clientId, newPlayer.GetComponent<PlayerController>());

        int[] connectedClients = USNL.ServerManager.GetConnectedClientIds();
        for (int i = 0; i < connectedClients.Length; i++) {
            USNL.PacketSend.PlayerSpawned(connectedClients[i], _clientId, newPlayer.GetComponent<USNL.SyncedObject>().SyncedObjectUUID);
        }
    }

    private void DestroyAllPlayers() {
        for (int i = 0; i < playerControllers.Count; i++) {
            Destroy(playerControllers[i].gameObject);
        }
        playerControllers.Clear();
    }

    #endregion

    #region Public Functions

    public void OnPlayerDeath(int _clientId) {
        playerControllers.Remove(_clientId);
        StartCoroutine(RespawnPlayer(_clientId));
    }

    private IEnumerator RespawnPlayer(int _clientId) {
        yield return new WaitForSeconds(respawnTime);
        SpawnPlayer(_clientId);
    }

    #endregion

    #region Callbacks

    private void OnClientConnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;

        for (int i = 0; i < playerControllers.Count; i++) {
            USNL.PacketSend.PlayerSpawned(clientId, playerControllers[i].gameObject.GetComponent<PlayerController>().ClientId, playerControllers[i].gameObject.GetComponent<USNL.SyncedObject>().SyncedObjectUUID);
        }
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;
        
        if (playerControllers.ContainsKey(clientId)) {
            Destroy(playerControllers[clientId].gameObject);
            playerControllers.Remove(clientId);
        }
    }
    
    #endregion
}
