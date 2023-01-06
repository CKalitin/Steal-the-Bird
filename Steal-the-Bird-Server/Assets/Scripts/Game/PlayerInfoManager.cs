using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be a struct, ugh
[Serializable]
public class PlayerInfo {
    [SerializeField] private string username;

    [SerializeField] private float damageDealt;
    [SerializeField] private float damageTaken;

    [SerializeField] private int playerKills;
    [SerializeField] private int playerDeaths;

    [SerializeField] private int enemyKills;
    [SerializeField] private int enemyDeaths;

    [SerializeField] private bool ready;

    public PlayerInfo() {
        username = "";
        damageDealt = 0f;
        damageTaken = 0f;
        playerKills = 0;
        playerDeaths = 0;
        enemyKills = 0;
        enemyDeaths = 0;
        ready = false;
    }

    public string Username { get => username; set => username = value; }

    public float DamageDealt { get => damageDealt; set => damageDealt = value; }
    public float DamageTaken { get => damageTaken; set => damageTaken = value; }

    public int PlayerKills { get => playerKills; set => playerKills = value; }
    public int PlayerDeaths { get => playerDeaths; set => playerDeaths = value; }

    public int EnemyKills { get => enemyKills; set => enemyKills = value; }
    public int EnemyDeaths { get => enemyDeaths; set => enemyDeaths = value; }

    public bool Ready { get => ready; set => ready = value; }
}

public class PlayerInfoManager : MonoBehaviour {
    #region Variables

    public static PlayerInfoManager instance;

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    public List<PlayerInfo> PlayerInfos { get => playerInfos; set => playerInfos = value; }

    #endregion

    #region Core

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Match Manager instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
        
        Initialize();
    }

    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerSetupInfoPacket += OnPlayerSetupInfoPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket += OnPlayerReadyPacket;
        USNL.CallbackEvents.OnClientDisconnected += OnClientDisconnected;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerSetupInfoPacket -= OnPlayerSetupInfoPacket;
        USNL.CallbackEvents.OnPlayerReadyPacket -= OnPlayerReadyPacket;
        USNL.CallbackEvents.OnClientDisconnected -= OnClientDisconnected;
    }

    #endregion
    
    #region Player Info Management

    private void Initialize() {
        playerInfos = new List<PlayerInfo>();
        for (int i = 0; i < USNL.ServerManager.instance.ServerConfig.MaxClients; i++)
            playerInfos.Add(new PlayerInfo());
    }

    public void SendPlayerInfo(int _id) {
        USNL.PacketSend.PlayerInfo(_id, playerInfos[_id].Username, playerInfos[_id].DamageDealt, playerInfos[_id].DamageTaken, playerInfos[_id].PlayerKills, playerInfos[_id].PlayerDeaths, playerInfos[_id].EnemyKills, playerInfos[_id].EnemyDeaths);
    }

    #endregion

    #region Callbacks

    private void OnPlayerSetupInfoPacket(object _packetObject) {
        USNL.PlayerSetupInfoPacket packet = (USNL.PlayerSetupInfoPacket)_packetObject;
        playerInfos[packet.FromClient].Username = packet.Username;
    }

    private void OnPlayerReadyPacket(object _packetObject) {
        USNL.PlayerReadyPacket packet = (USNL.PlayerReadyPacket)_packetObject;
        playerInfos[packet.FromClient].Ready = packet.Ready;
    }

    private void OnClientDisconnected(object _clientIdObject) {
        int clientId = (int)_clientIdObject;

        playerInfos[clientId] = new PlayerInfo();

        SendPlayerInfo(clientId);
    }

    #endregion
}
